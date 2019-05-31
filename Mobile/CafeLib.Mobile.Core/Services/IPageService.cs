using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CafeLib.Mobile.Core.ViewModels;
using Xamarin.Forms;

namespace CafeLib.Mobile.Core.Services
{
    public interface IPageService : IServiceProvider
    {
        /// <summary>
        /// Displays an alert on the page.
        /// </summary>
        /// <param name="title">title</param>
        /// <param name="message">message</param>
        /// <param name="ok">OK</param>
        void DisplayAlert(string title, string message, string ok = "OK");

        /// <summary>
        /// Displays an alert (simple question) on the page.
        /// </summary>
        /// <param name="title">title</param>
        /// <param name="message">message</param>
        /// <param name="ok">OK</param>
        /// <param name="cancel">cancel</param>
        // ReSharper disable once MethodOverloadWithOptionalParameter
        Task<bool> DisplayConfirm(string title, string message, string ok = "OK", string cancel = "Cancel");

        /// <summary>
        /// Displays an action sheet (list of buttons) on the page, asking for user input.
        /// </summary>
        /// <param name="title">dialog title</param>
        /// <param name="cancel">cancellation string</param>
        /// <param name="destroy">destroy string</param>
        /// <param name="options">option list</param>
        /// <returns></returns>
        Task<string> DisplayPopup(string title, string cancel, string destroy, IEnumerable<string> options);

        /// <summary>
        /// Resolves the page for the view model
        /// </summary>
        /// <typeparam name="T">BaseViewModel</typeparam>
        /// <typeparam name="TPage"></typeparam>
        /// <returns>page</returns>
        TPage ResolvePage<T, TPage>() where T : BaseViewModel<TPage> where TPage : Page;
    }
}
