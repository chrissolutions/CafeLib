
using JetBrains.Annotations;

namespace CafeLib.Mobile.Core.ViewModels
{
    /// <summary>
    /// Observable model interface
    /// </summary>
    /// <typeparam name="T">Serialization type</typeparam>
    public interface ISerializableModel<T>
    {
        /// <summary>
        /// Deserializes the observable model from the local storage.
        /// </summary>
        [UsedImplicitly]
        void Deserialize(T data);

        /// <summary>
        /// Serializes the observable model in the local storage.
        /// Returns the serialized model as a JSON string.
        /// </summary>
        [UsedImplicitly]
        T Serialize();

        /// <summary>
        /// Reset model data to default.
        /// </summary>
        [UsedImplicitly]
        void Reset();
    }
}
