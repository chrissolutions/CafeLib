using System;
using System.Collections.Concurrent;
using CafeLib.Core.Extensions;

namespace CafeLib.Core.IoC
{
    internal class PropertyService : IPropertyService
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
        public bool HasProperty<T>()
        {
            return HasProperty(typeof(T).FullName ?? throw new ArgumentNullException(typeof(T).Name));
        }

        /// <inheritdoc />
        public T GetProperty<T>()
        {
            return GetProperty<T>(typeof(T).FullName ?? throw new ArgumentNullException(typeof(T).Name));
        }

        /// <inheritdoc />
        public void SetProperty<T>(T value)
        {
            SetProperty(typeof(T).FullName ?? throw new ArgumentNullException(typeof(T).Name), value);
        }

        /// <inheritdoc />
        public bool RemoveProperty<T>()
        {
            return RemoveProperty(typeof(T).FullName);
        }

        /// <inheritdoc />
        public bool HasProperty(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        /// <inheritdoc />
        public T GetProperty<T>(string key)
        {
            return _dictionary.TryGetValue(key, out var value) ? (T)value : default;
        }

        /// <inheritdoc />
        public void SetProperty<T>(string key, T value)
        {
            _dictionary.AddOrUpdate(key, value, (k, v) => value);
        }

        /// <inheritdoc />
        public bool RemoveProperty(string key)
        {
            return _dictionary.TryRemove(key, out _);
        }

        /// <inheritdoc />
        public bool HasProperty(Guid guid)
        {
            return HasProperty(guid.ToString("B"));
        }

        /// <inheritdoc />
        public T GetProperty<T>(Guid guid)
        {
            return GetProperty<T>(guid.ToString("B"));
        }

        /// <inheritdoc />
        public void SetProperty<T>(Guid guid, T value)
        {
            SetProperty(guid.ToString("B"), value);
        }

        /// <inheritdoc />
        public bool RemoveProperty(Guid guid)
        {
            return RemoveProperty(guid.ToString("B"));
        }

        /// <inheritdoc />
        public T ToObject<T>()
        {
            return (T)_dictionary.ToObject();
        }
    }
}
