using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using CafeLib.Core.Extensions;

// ReSharper disable UnusedMember.Global

namespace CafeLib.Web.Request
{
    /// <summary>
    /// Abstract class to handle Http requests.
    /// </summary>
    public class WebHeaders : IDictionary<string, IEnumerable<string>>
    {
        protected ConcurrentDictionary<string, IEnumerable<string>> Headers;

        public int Count => Headers.Count;

        public bool IsReadOnly => false;

        /// <summary>
        /// WebRequestHeaders constructor.
        /// </summary>
        public WebHeaders()
        {
            Headers = new ConcurrentDictionary<string, IEnumerable<string>>();
        }

        internal WebHeaders(HttpHeaders headers)
            : this()
        {
            headers.ForEach(x => Add(x.Key, x.Value));
        }

        public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator()
        {
            return Headers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, string> item)
        {
            var (key, value) = item;
            Add(key, value);
        }

        public void Add(KeyValuePair<string, IEnumerable<string>> item)
        {
            var (key, value) = item;
            Add(key, value);
        }

        public void Clear()
        {
            Headers.Clear();
        }

        public bool Contains(KeyValuePair<string, IEnumerable<string>> item)
        {
            return Headers.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, IEnumerable<string>>[] array, int arrayIndex)
        {
            Headers.ToArray().CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, IEnumerable<string>> item)
        {
            return Remove(item.Key);
        }

        public void Add(string key, string value)
        {
            var items = (IList)Headers.GetOrAdd(key, x => new List<string>());
            items.Add(value);
        }

        public void Add(string key, IEnumerable<string> value)
        {
            var items = Headers.GetOrAdd(key, x => new List<string>());
            ((List<string>)items).AddRange(value);
        }

        public bool ContainsKey(string key) => Headers.ContainsKey(key);

        public bool Remove(string key) => Headers.Remove(key, out _);

        public bool TryGetValue(string key, out IEnumerable<string> value) => Headers.TryGetValue(key, out value);

        public IEnumerable<string> this[string key]
        {
            get => Headers[key];
            set => Headers[key] = value;
        }

        public ICollection<string> Keys => Headers.Keys;

        public ICollection<IEnumerable<string>> Values => Headers.Values;
    }
}
