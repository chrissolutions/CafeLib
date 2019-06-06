using System;
using System.ComponentModel;

namespace CafeLib.Mobile.ViewModels
{
    /// <summary>
    /// ObservableModel implementation class.
    /// </summary>
    /// <typeparam name="T">serialization type</typeparam>
    public abstract class ObservableModel<T> : ObservableBase, ISerializableModel<T>
    {
        protected ObservableModel()
        {
            PropertyChanged += ObservableModel_PropertyChanged;
        }

        public bool IsDirty
        {
            get;
            protected set;
        }

        protected virtual void ObservableModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsDirty = true;
        }

        public virtual void Deserialize(T data)
        {
            throw new NotImplementedException();
        }

        public abstract T Serialize();

        /// <summary>
        /// Reset model data to default.
        /// </summary>
        public virtual void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
