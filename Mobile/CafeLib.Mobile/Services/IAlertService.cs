using System.Collections.Generic;
using System.Threading.Tasks;
// ReSharper disable UnusedMemberInSuper.Global

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
        Task DisplayAlert(string title, string message, string ok = "OK");

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
        /// <param name="cancel">cancel button display</param>
        /// <param name="delete">delete button display</param>
        /// <param name="options">option list</param>
        /// <returns></returns>
        Task<string> DisplayOptions(string title, string cancel, string delete, IEnumerable<string> options);
    }
}
