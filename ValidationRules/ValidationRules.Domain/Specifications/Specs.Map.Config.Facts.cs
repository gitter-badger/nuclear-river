using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Domain.Dto;
using NuClear.ValidationRules.Domain.Model.Facts;

namespace NuClear.ValidationRules.Domain.Specifications
{
    public static partial class Specs
    {
        public static partial class Map
        {
            public static partial class Config
            {
                public static class ToFacts
                {
                    public static readonly MapSpecification<OrderValidationConfig, IReadOnlyCollection<GlobalAssociatedPosition>> GlobalAssociatedPosition =
                        new MapSpecification<OrderValidationConfig, IReadOnlyCollection<GlobalAssociatedPosition>>(
                            config => config.Positions.SelectMany(position => position.MasterPositions.Select(master => new GlobalAssociatedPosition
                                {
                                    MasterPositionId = master.Id,
                                    AssociatedPositionId = position.Id,
                                    ObjectBindingType = Parce(master.BindingType),
                                })).ToArray());

                    // todo: в denied positions должна быть избыточность, симметричные пары тоже должны импортироваться
                    public static readonly MapSpecification<OrderValidationConfig, IReadOnlyCollection<GlobalDeniedPosition>> GlobalDeniedPosition =
                        new MapSpecification<OrderValidationConfig, IReadOnlyCollection<GlobalDeniedPosition>>(
                            config => config.Positions.SelectMany(position => position.DeniedPositions.Select(denied => new GlobalDeniedPosition
                            {
                                MasterPositionId = position.Id,
                                DeniedPositionId = denied.Id,
                                ObjectBindingType = Parce(denied.BindingType),
                            })).ToArray());

                    private static int Parce(string bindingType)
                    {
                        switch (bindingType.ToLowerInvariant())
                        {
                            case "match":
                                return 1;
                            case "nodependency":
                                return 2;
                            case "different":
                                return 3;
                            default:
                                throw new ArgumentException($"Unknown binding type {bindingType}", nameof(bindingType));
                        }
                    }
                }
            }
        }
    }
}