using System;

// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Collections.Dictionaries
{
    public interface IDictionaryService
    {
        /// <summary>
        /// Determine whether dictionary contains the entry.
        /// </summary>
        /// <typeparam name="T">entry type</typeparam>
        /// <returns>true if the dictionary contains the entry; false otherwise</returns>
        bool HasEntry<T>();

        /// <summary>
        /// Get entry from the dictionary.
        /// </summary>
        /// <typeparam name="T">entry type</typeparam>
        /// <returns>entry value</returns>
        T GetEntry<T>();

        /// <summary>
        /// Set an entry in the dictionary.
        /// </summary>
        /// <typeparam name="T">entry type</typeparam>
        /// <param name="value">entry value</param>
        void SetEntry<T>(T value);

        /// <summary>
        /// Remove entry keyed by its type from the dictionary.
        /// </summary>
        /// <typeparam name="T">entry type</typeparam>
        /// <returns>true if the entry has been removed; false otherwise</returns>
        bool RemoveEntry<T>();

        /// <summary>
        /// Determine whether the dictionary contains the entry.
        /// </summary>
        /// <param name="key">entry key</param>
        /// <returns>true if the dictionary contains the property; false otherwise</returns>
        bool HasEntry(string key);

        /// <summary>
        /// Get entry from the dictionary based on its key.
        /// </summary>
        /// <typeparam name="T">entry type</typeparam>
        /// <param name="key">entry key</param>
        /// <returns>entry value</returns>
        T GetEntry<T>(string key);

        /// <summary>
        /// Set an entry in the dictionary using a key.
        /// </summary>
        /// <typeparam name="T">entry type</typeparam>
        /// <param name="key">entry key</param>
        /// <param name="value">entry value</param>
        void SetEntry<T>(string key, T value);

        /// <summary>
        /// Remove entry from the dictionary based on its key.
        /// </summary>
        /// <param name="key">entry key</param>
        /// <returns>true if the entry has been removed; false otherwise</returns>
        bool RemoveEntry(string key);

        /// <summary>
        /// Determine whether the dictionary contains the entry.
        /// </summary>
        /// <param name="guid">entry guid</param>
        /// <returns>true if the dictionary contains the entry; false otherwise</returns>
        bool HasEntry(Guid guid);

        /// <summary>
        /// Get entry from the dictionary based on its key.
        /// </summary>
        /// <typeparam name="T">entry type</typeparam>
        /// <param name="guid">entry guid</param>
        /// <returns>entry value</returns>
        T GetEntry<T>(Guid guid);

        /// <summary>
        /// Set an entry in the dictionary using a guid.
        /// </summary>
        /// <typeparam name="T">entry type</typeparam>
        /// <param name="guid">entry guid</param>
        /// <param name="value">entry value</param>
        void SetEntry<T>(Guid guid, T value);

        /// <summary>
        /// Remove entry from the dictionary keyed by a guid.
        /// </summary>
        /// <param name="guid">entry guid</param>
        /// <returns>true if the entry has been removed; false otherwise</returns>
        bool RemoveEntry(Guid guid);

        /// <summary>
        /// Convert the property bag to an object.
        /// </summary>
        T ToObject<T>();
    }
}
