using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CafeLib.Core.Extensions;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Collections.Stores
{
    public class KeyValueStore : IDictionary<string, string>
    {
        private readonly IDictionary<string, string> _storage;
        private readonly string _filePath;

        public int Count => _storage.Count;
        public bool IsReadOnly => _storage.IsReadOnly;

        public KeyValueStore(string filePath)
        {
            _storage = new ConcurrentDictionary<string, string>();
            _filePath = filePath;
            Read();
        }

        protected void Read()
        {
            try
            {
                var data = File.ReadAllLines(_filePath);
                var kv = data.Select(x => x.Split('=')).Select(y => new KeyValuePair<string, string>(y[0], y[1]));
                kv.ForEach(x => _storage.Add(x));
            }
            catch (FileNotFoundException)
            {
            }
        }

        public void Write()
        {
            var data = _storage.Select(x => $"{x.Key}={x.Value}");
            File.WriteAllLines(_filePath, data, Encoding.UTF8);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _storage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, string> item)
        {
            _storage.Add(item);
        }

        public void Clear()
        {
            _storage.Clear();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return _storage.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            _storage.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            return _storage.Remove(item);
        }

        public bool ContainsKey(string key)
        {
            return _storage.ContainsKey(key);
        }

        public void Add(string key, string value)
        {
            _storage.Add(key, value);
        }

        public bool Remove(string key)
        {
            return _storage.Remove(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            return _storage.TryGetValue(key, out value);
        }

        public string this[string key]
        {
            get => _storage[key];
            set => _storage[key] = value;
        }

        public ICollection<string> Keys => _storage.Keys;
        public ICollection<string> Values => _storage.Values;
    }
}
