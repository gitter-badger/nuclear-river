using System.Collections.Generic;
using System.Linq;

namespace NuClear.DataTest.Metamodel
{
    public sealed class CommandlineParameters
    {
        private readonly IReadOnlyDictionary<string, string> _dictionary;

        public CommandlineParameters(IReadOnlyDictionary<string, string> dictionary)
        {
            _dictionary = dictionary.ToDictionary(x => x.Key.ToLower(), x => x.Value.ToLower());
        }

        public string Get(string key)
        {
            return _dictionary[key];
        }

        public bool TryGet(string key, out string value)
        {
            return _dictionary.TryGetValue(key.ToLower(), out value);
        }
    }
}
