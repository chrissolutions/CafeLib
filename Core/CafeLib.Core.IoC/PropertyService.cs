﻿using System;
using System.Collections.Concurrent;
using CafeLib.Core.Extensions;

namespace CafeLib.Core.IoC
{
    internal class DictionaryService : IDictionaryService
    {
        private readonly ConcurrentDictionary<string, object> _dictionary;

        /// <summary>
        /// PropertyService constructor.
        /// </summary>
        public DictionaryService()
        {
            _dictionary = new ConcurrentDictionary<string, object>();
        }

        /// <inheritdoc />
        public bool HasEntry<T>()
        {
            return HasEntry(typeof(T).FullName ?? throw new ArgumentNullException(typeof(T).Name));
        }

        /// <inheritdoc />
        public T GetEntry<T>()
        {
            return GetEntry<T>(typeof(T).FullName ?? throw new ArgumentNullException(typeof(T).Name));
        }

        /// <inheritdoc />
        public void SetEntry<T>(T value)
        {
            SetEntry(typeof(T).FullName ?? throw new ArgumentNullException(typeof(T).Name), value);
        }

        /// <inheritdoc />
        public bool RemoveEntry<T>()
        {
            return RemoveEntry(typeof(T).FullName);
        }

        /// <inheritdoc />
        public bool HasEntry(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        /// <inheritdoc />
        public T GetEntry<T>(string key)
        {
            return _dictionary.TryGetValue(key, out var value) ? (T)value : default;
        }

        /// <inheritdoc />
        public void SetEntry<T>(string key, T value)
        {
            _dictionary.AddOrUpdate(key, value, (k, v) => value);
        }

        /// <inheritdoc />
        public bool RemoveEntry(string key)
        {
            return _dictionary.TryRemove(key, out _);
        }

        /// <inheritdoc />
        public bool HasEntry(Guid guid)
        {
            return HasEntry(guid.ToString("B"));
        }

        /// <inheritdoc />
        public T GetEntry<T>(Guid guid)
        {
            return GetEntry<T>(guid.ToString("B"));
        }

        /// <inheritdoc />
        public void SetEntry<T>(Guid guid, T value)
        {
            SetEntry(guid.ToString("B"), value);
        }

        /// <inheritdoc />
        public bool RemoveEntry(Guid guid)
        {
            return RemoveEntry(guid.ToString("B"));
        }

        /// <inheritdoc />
        public T ToObject<T>()
        {
            return (T)_dictionary.ToObject();
        }
    }
}
