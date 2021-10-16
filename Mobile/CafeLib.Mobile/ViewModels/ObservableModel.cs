using System;
using System.ComponentModel;
using System.Windows.Input;
// ReSharper disable UnusedMember.Global

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
            PropertyChanged += OnPropertyChanged;
        }

        /// <summary>
        /// Determines whether the observable model is dirty.
        /// </summary>
        private bool _isDirty;
        public bool IsDirty
        {
            get => _isDirty;
            protected set => SetValue(ref _isDirty, value);
        }

        /// <summary>
        /// Appearing command.
        /// </summary>
        protected ICommand PropertyChangedCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">PropertyChangedEventArgs</param>
        protected void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsDirty = true;
            PropertyChangedCommand.Execute(e.PropertyName);
        }

        /// <summary>
        /// Deserialize the data of type T into an observable model.
        /// </summary>
        /// <param name="data"></param>
        public virtual void Deserialize(T data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Serialize the model
        /// </summary>
        /// <returns>serialized object into type of T</returns>
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
