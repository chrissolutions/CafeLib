using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CafeLib.Mobile.ViewModels
{
    public abstract class ObservableBase : INotifyPropertyChanged
    {
        #region Event Delegate 

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        /// <summary>
        /// Property changed handler.
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Refresh all properties.
        /// </summary>
        protected void RefreshProperties()
        {
            OnPropertyChanged(string.Empty);
        }

        /// <summary>
        /// Set a property value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingField"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        protected bool SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;

            // ReSharper disable once ExplicitCallerInfoArgument
            OnPropertyChanged(propertyName);

            return true;
        }

        #endregion
    }
}
