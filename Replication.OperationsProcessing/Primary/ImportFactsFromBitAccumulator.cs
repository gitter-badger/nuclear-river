using System.Linq;
using System.Xml.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Messaging.Transports.CorporateBus.API;
using NuClear.Replication.OperationsProcessing.Metadata.Flows;
using NuClear.Replication.OperationsProcessing.Transports.CorporateBus;
using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Primary
{
    public sealed class ImportFactsFromBitAccumulator : MessageProcessingContextAccumulatorBase<ImportFactsFromBitFlow, CorporateBusPerformedOperationsMessage, CorporateBusDtoMessage>
    {
        private readonly ITelemetryPublisher _telemetryPublisher;

        public ImportFactsFromBitAccumulator(ITelemetryPublisher telemetryPublisher)
        {
            _telemetryPublisher = telemetryPublisher;
        }

        protected override CorporateBusDtoMessage Process(CorporateBusPerformedOperationsMessage message)
        {
            _telemetryPublisher.Trace("Process");

            var xmls = message.Packages.SelectMany(x => x.ConvertToXElements());

            var dtos = xmls.Select(x =>
            {
                ICorporateBusDto dto;
                return new
                {
                    Parsed = TryParseXml(x, out dto),
                    Dto = dto,
                };
            })
            .Where(x => x.Parsed)
            .Select(x => x.Dto);

            return new CorporateBusDtoMessage
            {
                Id = message.Id,
                TargetFlow = MessageFlow,
                Dtos = dtos,
            };
        }

        private static bool TryParseXml(XElement xml, out ICorporateBusDto dto)
        {
            switch (xml.Name.LocalName.ToLowerInvariant())
            {
                case "firmpopularity":
                    return TryParseFirmPopularity(xml, out dto);
                case "rubricpopularity":
                    return TryParseRubricPopularity(xml, out dto);
                default:
                    dto = null;
                    return false;
            }
        }

        private static bool TryParseFirmPopularity(XElement xml, out ICorporateBusDto dto)
        {
            dto = new FirmStatisticsDto
            {
                ProjectId = (long)xml.Attribute("BranchCode"),
                Firms = xml.Descendants("Firm").Select(x =>
                {
                    var firmDto = new FirmStatisticsDto.FirmDto
                    {
                        FirmId = (long)x.Attribute("Code"),
                        Categories = x.Descendants("Rubric").Select(y =>
                        {
                            var rubricDto = new FirmStatisticsDto.FirmDto.CategoryDto
                            {
                                CategoryId = (long)y.Attribute("Code"),
                                Hits = (long)y.Attribute("ClickCount"),
                                Shows = (long)y.Attribute("ImpressionCount")
                            };

                            return rubricDto;
                        }).ToList()
                    };

                    return firmDto;
                }).ToList(),
            };

            return true;
        }

        private static bool TryParseRubricPopularity(XElement xml, out ICorporateBusDto dto)
        {
            var branchElement = xml.Element("Branch");
            if (branchElement == null)
            {
                dto = null;
                return false;
            }

            dto = new CategoryStatisticsDto
            {
                ProjectId = (long)branchElement.Attribute("Code"),
                Categories = xml.Descendants("Rubric").Select(x =>
                {
                    var rubricDto = new CategoryStatisticsDto.CategoryDto
                    {
                        CategoryId = (long)x.Attribute("Code"),
                        AdvertisersCount = (long)x.Attribute("AdvFirmCount")
                    };

                    return rubricDto;
                }).ToList()
            };

            return true;
        }
    }
}