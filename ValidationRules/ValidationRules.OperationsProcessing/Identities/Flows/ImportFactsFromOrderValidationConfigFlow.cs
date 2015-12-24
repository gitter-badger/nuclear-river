using System;

using NuClear.Messaging.API.Flows;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.Flows
{
    public sealed class ImportFactsFromOrderValidationConfigFlow : MessageFlowBase<ImportFactsFromOrderValidationConfigFlow>
    {
        public override Guid Id { get; }
            = new Guid("F63AAF46-1DF0-4BB2-B388-CFC2557D3457");

        public override string Description { get; }
            = "Маркер для потока импорта конфига orderValidation.config в домен Validation Rules";
    }
}