﻿namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class Account : IErmFactObject
    {
        public long Id { get; set; }

        public decimal Balance { get; set; }

        public long BranchOfficeOrganizationUnitId { get; set; }

        public long LegalPersonId { get; set; }
    }
}