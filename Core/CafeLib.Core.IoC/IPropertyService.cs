using System;

namespace CafeLib.Core.IoC
{
    public interface IPropertyService : IServiceProvider
    {
        /// <summary>
        /// Get property from property bag
        /// </summary>
        /// <typeparam name="T">property type</typeparam>
        /// <returns>property value</returns>
        T GetProperty<T>();

        /// <summary>
        /// Set a property in the property bag
        /// </summary>
        /// <typeparam name="T">property type</typeparam>
        /// <param name="value">property value</param>
        void SetProperty<T>(T value);

        /// <summary>
        /// Get property from property bag based on its key.
        /// </summary>
        /// <typeparam name="T">property type</typeparam>
        /// <param name="key">property key</param>
        /// <returns>property value</returns>
        T GetProperty<T>(string key);

        /// <summary>
        /// Set a property in the property bag using a key.
        /// </summary>
        /// <typeparam name="T">property type</typeparam>
        /// <param name="key">property key</param>
        /// <param name="value">property value</param>
        void SetProperty<T>(string key, T value);

        /// <summary>
        /// Get property from property bag based on its key.
        /// </summary>
        /// <typeparam name="T">property type</typeparam>
        /// <param name="guid">property guid</param>
        /// <returns>property value</returns>
        T GetProperty<T>(Guid guid);

        /// <summary>
        /// Set a property in the property bag using a guid.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="guid">property guid</param>
        /// <param name="value">property value</param>
        void SetProperty<T>(Guid guid, T value);

        /// <summary>
        /// Set a property in the property bag using a guid.
        /// </summary>
        T ToObject<T>();
    }
}
