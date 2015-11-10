using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.SqlServer.Management.Smo;

namespace NuClear.DataTest.Runner.Smo
{
    internal sealed class DatabaseSchemaComparer
    {
        private readonly Database _sourceDatabase;
        private readonly Database _targetDatabase;
        private readonly IEnumerable<string> _schemas;

        public DatabaseSchemaComparer(Database sourceDatabase, Database targetDatabase, IEnumerable<string> schemas)
        {
            _sourceDatabase = sourceDatabase.PrefetchTablesAndViews();
            _targetDatabase = targetDatabase.PrefetchTablesAndViews();
            _schemas = schemas;
        }

        public bool Equals()
        {
            var differences = GetDifferences();
            return !differences.Any();
        }

        public IEnumerable<TableViewTableTypeBase> GetDifferences()
        {
            var differences = _schemas.SelectMany(schema =>
            {
                var sourceCompareObjects = _sourceDatabase.GetTablesAndViewsFor(schema);
                var targetCompareObjects = _targetDatabase.GetTablesAndViewsFor(schema);

                var schemaDifferences = GetDifferences(sourceCompareObjects, targetCompareObjects);
                return schemaDifferences;
            });

            return differences;
        }

        private static IEnumerable<TableViewTableTypeBase> GetDifferences(IEnumerable<TableViewTableTypeBase> sources, IEnumerable<TableViewTableTypeBase> targets)
        {
            var targetsDictionary = targets.ToDictionary(x => x.Name);

            var differences = sources.Select(source =>
            {
                TableViewTableTypeBase target;
                if (!targetsDictionary.TryGetValue(source.Name, out target))
                {
                    return source;
                }

                var columnDifferences = GetDifferences(source.Columns.Cast<Column>(), target.Columns.Cast<Column>());
                if (columnDifferences.Any())
                {
                    return source;
                }

                return null;
            })
            .Where(x => x != null);

            return differences;
        }

        private static IEnumerable<Column> GetDifferences(IEnumerable<Column> sources, IEnumerable<Column> targets)
        {
            var targetsDictionary = targets.ToDictionary(x => x.Name);

            var differences = sources.Select(source =>
            {
                Column target;
                if (!targetsDictionary.TryGetValue(source.Name, out target))
                {
                    return source;
                }

                if (DataTypeComparer.Instance.Compare(source.DataType, target.DataType) < 0)
                {
                    return source;
                }

                return null;
            })
            .Where(x => x != null);

            return differences;
        }

        private sealed class DataTypeComparer : IComparer<DataType>
        {
            private DataTypeComparer() { }

            public static readonly DataTypeComparer Instance = new DataTypeComparer();

            private static readonly Type[][] TyperOrders =
            {
                new [] { typeof(byte), typeof(short), typeof(int), typeof(long) },
                new [] { typeof(float), typeof(double), typeof(decimal) },
                new [] { typeof(DateTime), typeof(DateTimeOffset) },
            };

            public int Compare(DataType x, DataType y)
            {
                var xClrType = GetClrType(x.SqlDataType);
                var yClrType = GetClrType(y.SqlDataType);

                if (xClrType == yClrType)
                {
                    if (x.NumericPrecision < y.NumericPrecision ||
                        x.NumericScale < y.NumericScale ||
                        (x.MaximumLength >=0 && y.MaximumLength >= 0 && x.MaximumLength < y.MaximumLength))
                    {
                        return -1;
                    }

                    return 0;
                }

                foreach (var typerOrder in TyperOrders)
                {
                    var xIndex = Array.IndexOf(typerOrder, xClrType);
                    var yIndex = Array.IndexOf(typerOrder, yClrType);

                    if (xIndex == -1 && yIndex == -1)
                    {
                        continue;
                    }

                    if (xIndex == -1 || yIndex == -1)
                    {
                        return -1;
                    }

                    return xIndex.CompareTo(yIndex);
                }

                return -1;
            }

            private static Type GetClrType(SqlDataType sqlDataType)
            {
                switch (sqlDataType)
                {
                    case SqlDataType.BigInt:
                        return typeof(long);
                    case SqlDataType.Int:
                        return typeof(int);
                    case SqlDataType.SmallInt:
                        return typeof(short);
                    case SqlDataType.TinyInt:
                        return typeof(byte);

                    case SqlDataType.Decimal:
                    case SqlDataType.Numeric:
                    case SqlDataType.Money:
                    case SqlDataType.SmallMoney:
                        return typeof(decimal);
                    case SqlDataType.Float:
                        return typeof(double);
                    case SqlDataType.Real:
                        return typeof(float);

                    case SqlDataType.DateTimeOffset:
                        return typeof(DateTimeOffset);
                    case SqlDataType.SmallDateTime:
                    case SqlDataType.Date:
                    case SqlDataType.Time:
                    case SqlDataType.DateTime:
                    case SqlDataType.DateTime2:
                        return typeof(DateTime);

                    case SqlDataType.Bit:
                        return typeof(bool);

                    case SqlDataType.Char:
                    case SqlDataType.NChar:
                    case SqlDataType.NText:
                    case SqlDataType.NVarChar:
                    case SqlDataType.NVarCharMax:
                    case SqlDataType.Text:
                    case SqlDataType.VarChar:
                    case SqlDataType.VarCharMax:
                    case SqlDataType.Xml:
                    case SqlDataType.SysName:
                        return typeof(string);

                    case SqlDataType.UniqueIdentifier:
                        return typeof(Guid);

                    case SqlDataType.Binary:
                    case SqlDataType.Image:
                    case SqlDataType.Timestamp:
                    case SqlDataType.VarBinary:
                    case SqlDataType.VarBinaryMax:
                        return typeof(byte[]);

                    case SqlDataType.UserDefinedDataType:
                    case SqlDataType.UserDefinedType:
                    case SqlDataType.Variant:
                    case SqlDataType.UserDefinedTableType:
                    case SqlDataType.HierarchyId:
                    case SqlDataType.Geometry:
                    case SqlDataType.Geography:
                    case SqlDataType.None:
                        return typeof(object);

                    default:
                        throw new ArgumentOutOfRangeException(nameof(sqlDataType), sqlDataType, null);
                }
            }

        }
    }
}