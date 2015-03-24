using System;

using LinqToDB;

namespace NuClear.AdvancedSearch.Replication.Data
{
    internal sealed class DataMapper
    {
        private readonly IDataContext _context;

        public DataMapper(IDataContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            _context = context;
        }

        public void Insert<T>(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            _context.Insert(item);
        }

        public void Update<T>(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            _context.Update(item);
        }

        public void Delete<T>(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            _context.Delete(item);
        }
    }
}
