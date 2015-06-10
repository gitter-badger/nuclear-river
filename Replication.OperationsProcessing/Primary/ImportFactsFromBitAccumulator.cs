using System;
using System.Linq;
using System.Xml.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Messaging.Transports.CorporateBus.API;
using NuClear.Replication.OperationsProcessing.Metadata.Flows;
using NuClear.Replication.OperationsProcessing.Transports.CorporateBus;

namespace NuClear.Replication.OperationsProcessing.Primary
{
    public sealed class ImportFactsFromBitAccumulator : MessageProcessingContextAccumulatorBase<ImportFactsFromBitFlow, CorporateBusPerformedOperationsMessage, CorporateBusDtoMessage>
    {
        protected override CorporateBusDtoMessage Process(CorporateBusPerformedOperationsMessage message)
        {
            var xmls = message.Packages.SelectMany(x => x.ConvertToXElements());
            var dtos = xmls.Select(ParseXml);

            return new CorporateBusDtoMessage
            {
                Id = message.Id,
                TargetFlow = MessageFlow,
                Dtos = dtos,
            };
        }

        private static object ParseXml(XElement xml)
        {
            switch (xml.Name.LocalName.ToLowerInvariant())
            {
                case "firmpopularity":
                    return ParseFirmPopularity(xml);
                case "rubricpopularity":
                    return ParseRubricPopularity(xml);
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Unsupported type of corporate bus message '{0}'", xml.Name.LocalName));
            }
        }

        private static object ParseFirmPopularity(XElement xml)
        {
            var dto = new FirmStatisticsDto();
            dto.ProjectId = (long)xml.Attribute("BranchCode");
            dto.Firms = xml.Descendants("Firm").Select(x =>
            {
                var firmDto = new FirmStatisticsDto.FirmDto();
                firmDto.FirmId = (long)x.Attribute("Code");
                firmDto.Categories = x.Descendants("Rubric").Select(y =>
                {
                    var rubricDto = new FirmStatisticsDto.FirmDto.CategoryDto();
                    rubricDto.CategoryId = (long)y.Attribute("Code");
                    rubricDto.Hits = (long)y.Attribute("ClickCount");
                    rubricDto.Shows = (long)y.Attribute("ImpressionCount");

                    return rubricDto;
                }).ToList();

                return firmDto;
            }).ToList();

            return dto;
        }

        private static object ParseRubricPopularity(XElement xml)
        {
            var dto = new CategoryStatisticsDto();

            var branchElement = xml.Element("Branch");
            if (branchElement == null)
            {
                // FIXME {y.baranihin, 09.06.2015}: надо решить настраивать фильтрацию на шине или не настраивать и фильтровать самим здесь
                throw new ArgumentException("Element 'Branch' is required");
            }
            dto.ProjectId = (long)branchElement.Attribute("Code");

            dto.Categories = xml.Descendants("Rubric").Select(x =>
            {
                var rubricDto = new CategoryStatisticsDto.CategoryDto();
                rubricDto.CategoryId = (long)x.Attribute("Code");
                rubricDto.AdvertisersCount = (long)x.Attribute("AdvFirmCount");

                return rubricDto;
            }).ToList();

            return dto;
        }
    }
}