using System;
using System.Linq;
using System.Xml.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Messaging.Transports.CorporateBus.API;
using NuClear.Replication.OperationsProcessing.Metadata.Flows;
using NuClear.Replication.OperationsProcessing.Transports.CorporateBus;
using NuClear.Tracing.API;

namespace NuClear.Replication.OperationsProcessing.Primary
{
    public sealed class ImportFactsFromBitAccumulator : MessageProcessingContextAccumulatorBase<ImportFactsFromBitFlow, CorporateBusPerformedOperationsMessage, CorporateBusAggregatableMessage>
    {
        private readonly ITracer _tracer;

        public ImportFactsFromBitAccumulator(ITracer tracer)
        {
            _tracer = tracer;
        }

        protected override CorporateBusAggregatableMessage Process(CorporateBusPerformedOperationsMessage message)
        {
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
            .Select(x => x.Dto)
            .ToList();

            return new CorporateBusAggregatableMessage
            {
                TargetFlow = MessageFlow,
                Dtos = dtos,
            };
        }

        private bool TryParseXml(XElement xml, out ICorporateBusDto dto)
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

        private bool TryParseFirmPopularity(XElement xml, out ICorporateBusDto dto)
        {
            try
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
                                var clickCountAttr = y.Attribute("ClickCount");
                                var impressionCountAttr = y.Attribute("ImpressionCount");
                                if (clickCountAttr == null || impressionCountAttr == null)
                                {
                                    throw new ArgumentException();
                                }

                                var rubricDto = new FirmStatisticsDto.FirmDto.CategoryDto
                                {
                                    CategoryId = (long)y.Attribute("Code"),
                                    Hits = (long)clickCountAttr,
                                    Shows = (long)impressionCountAttr
                                };

                                return rubricDto;
                            }).ToList()
                        };

                        return firmDto;
                    }).ToList(),
                };

                return true;
            }
            catch (ArgumentException)
            {
                _tracer.Warn("Skip FirmPopularity message due to unsupported format");
                dto = null;
                return false;
            }
        }

        private bool TryParseRubricPopularity(XElement xml, out ICorporateBusDto dto)
        {
            var branchElement = xml.Element("Branch");
            if (branchElement == null)
            {
                dto = null;
                return false;
            }

            try
            {
                dto = new CategoryStatisticsDto
                {
                    ProjectId = (long)branchElement.Attribute("Code"),
                    Categories = xml.Descendants("Rubric").Select(x =>
                    {
                        var advFirmCountAttr = x.Attribute("AdvFirmCount");
                        if (advFirmCountAttr == null)
                        {
                            throw new ArgumentException();
                        }

                        var rubricDto = new CategoryStatisticsDto.CategoryDto
                        {
                            CategoryId = (long)x.Attribute("Code"),
                            AdvertisersCount = (long)advFirmCountAttr
                        };

                        return rubricDto;
                    }).ToList()
                };

                return true;
            }
            catch (ArgumentException)
            {
                _tracer.Warn("Skip RubricPopularity message due to unsupported format");
                dto = null;
                return false;
            }
        }
    }
}