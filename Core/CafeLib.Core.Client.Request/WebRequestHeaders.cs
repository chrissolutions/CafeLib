using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace CafeLib.Core.Client.Request
{
    /// <summary>
    /// Abstract class to handle Http requests.
    /// </summary>
    public class WebRequestHeaders : IDictionary<string, string>
    {
        protected ConcurrentDictionary<string, string> Headers;

        /// <inheritdoc />
        public WebRequestHeaders()
        {
            Headers = new ConcurrentDictionary<string, string>();
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return Headers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, string> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            Headers.Clear();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return Headers.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            Headers.ToArray().CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            return Remove(item.Key);
        }

        public int Count => Headers.Count;

        public bool IsReadOnly => false;

        public void Add(string key, string value)
        {
            Headers.GetOrAdd(key, value);
        }

        public bool ContainsKey(string key)
        {
            return Headers.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return Headers.TryRemove(key, out var _);
        }

        public bool TryGetValue(string key, out string value)
        {
            return Headers.TryGetValue(key, out value);
        }

        public string this[string key]
        {
            get => Headers[key];
            set => Headers.AddOrUpdate(key, value, (k, v) => value);
        }

        public ICollection<string> Keys => Headers.Keys;

        public ICollection<string> Values => Headers.Values;
    }
}
