using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Web.Request
{
    /// <summary>
    /// Abstract class to handle Http requests.
    /// </summary>
    public class WebRequestHeaders : IDictionary<string, IList<string>>
    {
        protected ConcurrentDictionary<string, IList<string>> Headers;

        public int Count => Headers.Count;

        public bool IsReadOnly => false;

        /// <summary>
        /// WebRequestHeaders constructor.
        /// </summary>
        public WebRequestHeaders()
        {
            Headers = new ConcurrentDictionary<string, IList<string>>();
        }

        public IEnumerator<KeyValuePair<string, IList<string>>> GetEnumerator()
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

        public void Add(KeyValuePair<string, IList<string>> item)
        {
            var (key, value) = item;
            Add(key, value);
        }

        public void Clear()
        {
            Headers.Clear();
        }

        public bool Contains(KeyValuePair<string, IList<string>> item)
        {
            return Headers.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, IList<string>>[] array, int arrayIndex)
        {
            Headers.ToArray().CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, IList<string>> item)
        {
            return Remove(item.Key);
        }

        public void Add(string key, string value)
        {
            var items = Headers.GetOrAdd(key, x => new List<string>());
            items.Add(value);
        }

        public void Add(string key, IList<string> value)
        {
            var items = Headers.GetOrAdd(key, x => new List<string>());
            ((List<string>)items).AddRange(value);
        }

        public bool ContainsKey(string key) => Headers.ContainsKey(key);

        public bool Remove(string key) => Headers.Remove(key, out _);

        public bool TryGetValue(string key, out IList<string> value) => Headers.TryGetValue(key, out value);

        public IList<string> this[string key]
        {
            get => Headers[key];
            set => Headers[key] = value;
        }

        public ICollection<string> Keys => Headers.Keys;

        public ICollection<IList<string>> Values => Headers.Values;
    }
}
