using System;
using System.Collections.Concurrent;

namespace CafeLib.Core.IoC
{
    internal class PropertyService : ServiceBase, IPropertyService
    {
        private readonly ConcurrentDictionary<string, object> _dictionary;

        /// <summary>
        /// PropertyService constructor.
        /// </summary>
        public PropertyService()
        {
            _dictionary = new ConcurrentDictionary<string, object>();
        }

        /// <inheritdoc />
        public T GetProperty<T>()
        {
            return _dictionary.TryGetValue(typeof(T).FullName ?? throw new ArgumentNullException(typeof(T).Name), out var value) ? (T)value : default(T);
        }

        /// <inheritdoc />
        public void SetProperty<T>(T value)
        {
            _dictionary.AddOrUpdate(typeof(T).FullName ?? throw new ArgumentNullException(typeof(T).Name), value, (k, v) => value);
        }

        /// <inheritdoc />
        public T GetProperty<T>(string key)
        {
            return _dictionary.TryGetValue(key, out var value) ? (T)value : default(T);
        }

        /// <inheritdoc />
        public void SetProperty<T>(string key, T value)
        {
            _dictionary.AddOrUpdate(key, value, (k, v) => value);
        }

        /// <inheritdoc />
        public T GetProperty<T>(Guid guid)
        {
            return _dictionary.TryGetValue(guid.ToString("B"), out var value) ? (T)value : default(T);
        }

        /// <inheritdoc />
        public void SetProperty<T>(Guid guid, T value)
        {
            _dictionary.AddOrUpdate(guid.ToString("B"), value, (k, v) => value);
        }
    }
}
