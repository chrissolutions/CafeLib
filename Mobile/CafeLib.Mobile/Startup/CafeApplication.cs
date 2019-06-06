using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CafeLib.Core.IoC;
using CafeLib.Mobile.Services;
using Xamarin.Forms;

namespace CafeLib.Mobile.Startup
{
    /// <summary>
    /// Base class of CafeLib mobile application.
    /// </summary>
    public abstract class CafeApplication : Application, IAlertService, IDisposable
    {
        protected IServiceRegistry Registry { get; }

        /// <summary>
        /// Cafe mobile application constructor.
        /// </summary>
        /// <param name="serviceRegistry"></param>
        protected CafeApplication(IServiceRegistry serviceRegistry)
        {
            Registry = serviceRegistry ?? throw new ArgumentNullException(nameof(serviceRegistry));
        }

        public virtual void Configure()
        {
            Registry.AddScoped<IAlertService>(x => this);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Alert(string title, string message, string ok = "OK")
        {
            throw new NotImplementedException();
        }

        public Task<bool> Confirm(string title, string message, string ok = "OK", string cancel = "Cancel")
        {
            throw new NotImplementedException();
        }

        public Task<string> Popup(string title, string cancel, string destroy, IEnumerable<string> options)
        {
            throw new NotImplementedException();
        }
    }
}
