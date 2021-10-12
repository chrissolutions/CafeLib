using System.Threading.Tasks;
using CafeLib.Core.Support;
using DroneLander.ViewModels;
using Xamarin.Forms;

namespace DroneLander
{
    public class Bootstrapper : SingletonBase<Bootstrapper>
    {
        /// <summary>
        /// Initialize the Navigation Page here.
        /// </summary>
        /// <param name="args">application arguments</param>
        /// <returns>navigation page</returns>
        public static async Task InitApplication(params object[] args)
        {
            // Initialize the root page.
            Application.Current.MainPage = new MainViewModel().AsNavigationPage();
            await Task.CompletedTask;
        }
    }
}
