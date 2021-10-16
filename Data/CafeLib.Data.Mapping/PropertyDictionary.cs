using CafeLib.Core.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CafeLib.Data.Mapping
{
    internal class PropertyDictionary<TEntity> : IEnumerable<KeyValuePair<string, PropertyInfo>> where TEntity : IEntity
    {
        private readonly IDictionary<string, PropertyInfo> _dictionary;

        public PropertyDictionary()
        {
            _dictionary = typeof(TEntity).GetProperties().ToDictionary(p => p.Name);
        }

        public ICollection<string> Keys => _dictionary.Keys;

        public ICollection<PropertyInfo> Values => _dictionary.Values;

        public IEnumerator<KeyValuePair<string, PropertyInfo>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(string key, PropertyInfo value)
        {
            _dictionary.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool TryGetValue(string key, out PropertyInfo value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public PropertyInfo this[string key]
        {
            get => _dictionary[key];
            set => _dictionary[key] = value;
        }
    }
}
