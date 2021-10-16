using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using CafeLib.Core.Extensions;

namespace CafeLib.Authorization.Tokens
{
    public class ClaimCollection : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly IDictionary<string, string> _dictionary;

        public int Count => _dictionary.Count;
        public bool IsReadOnly => _dictionary.IsReadOnly;

        /// <summary>
        /// Claims default constructor.
        /// </summary>
        public ClaimCollection()
        {
            _dictionary = new ConcurrentDictionary<string, string>();
        }

        /// <summary>
        /// Claims constructor.
        /// </summary>
        /// <param name="claims">collection of claims</param>
        public ClaimCollection(IDictionary<string, string> claims)
        {
            _dictionary = new ConcurrentDictionary<string, string>(claims);
        }


        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public void Add(string key, string value)
        {
            _dictionary.AddOrUpdate(key, value, (k, v) => value);
        }

        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return _dictionary.Remove(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public string this[string key]
        {
            get => _dictionary.TryGetValue(key, out var claim) ? claim : null;
            set => _dictionary[key] = value;
        }

        public ICollection<string> Keys => _dictionary.Keys;

        public ICollection<string> Values => _dictionary.Values;
    }
}
