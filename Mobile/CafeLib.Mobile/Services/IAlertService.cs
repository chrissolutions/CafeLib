using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace CafeLib.Mobile.Services
{
    public interface IAlertService
    {
        /// <summary>
        /// Displays an alert on the page.
        /// </summary>
        /// <param name="title">title</param>
        /// <param name="message">message</param>
        /// <param name="ok">OK</param>
        [UsedImplicitly]
        void Alert(string title, string message, string ok = "OK");

        /// <summary>
        /// Displays an alert (simple question) on the page.
        /// </summary>
        /// <param name="title">title</param>
        /// <param name="message">message</param>
        /// <param name="ok">OK</param>
        /// <param name="cancel">cancel</param>
        // ReSharper disable once MethodOverloadWithOptionalParameter
        [UsedImplicitly]
        Task<bool> Confirm(string title, string message, string ok = "OK", string cancel = "Cancel");

        /// <summary>
        /// Displays an action sheet (list of buttons) on the page, asking for user input.
        /// </summary>
        /// <param name="title">dialog title</param>
        /// <param name="cancel">cancellation string</param>
        /// <param name="destroy">destroy string</param>
        /// <param name="options">option list</param>
        /// <returns></returns>
        [UsedImplicitly]
        Task<string> Popup(string title, string cancel, string destroy, IEnumerable<string> options);
    }
}
