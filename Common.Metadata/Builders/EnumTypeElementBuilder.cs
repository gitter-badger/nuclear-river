﻿using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Common.Metadata.Elements;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.AdvancedSearch.Common.Metadata.Builders
{
    public sealed class EnumTypeElementBuilder : MetadataElementBuilder<EnumTypeElementBuilder, EnumTypeElement>
    {
        private readonly Dictionary<string, long> _enumMembers = new Dictionary<string, long>();
        private string _name;

        public EnumTypeElementBuilder Name(string name)
        {
            _name = name;
            return this;
        }

        public EnumTypeElementBuilder Member(string name, long value)
        {
            _enumMembers.Add(name, value);
            return this;
        }

        protected override EnumTypeElement Create()
        {
            if (string.IsNullOrEmpty(_name))
            {
                throw new InvalidOperationException("The entity name was not specified.");
            }

            return new EnumTypeElement(UriExtensions.AsUri(_name).AsIdentity(), _enumMembers);
        }
    }
}