using CafeLib.Core.IoC;
using CafeLib.Mobile.Extensions;
using CafeLib.Mobile.Startup;
using DroneLander.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace DroneLander
{
    // ReSharper disable once RedundantExtendsListEntry
    public partial class App : CafeApplication
    {
        public App(IServiceRegistry registry)
            : base(registry)
        {
            InitializeComponent();
        }

        /// <summary>
        /// Configure the application.
        /// </summary>
        public override void Configure()
        {
            Registry
                .AddViewModel<MainViewModel>();
        }

        protected override void OnStart()
        {
            // Handles where your app starts
            Application.Current.StartOnViewModel<MainViewModel>();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
