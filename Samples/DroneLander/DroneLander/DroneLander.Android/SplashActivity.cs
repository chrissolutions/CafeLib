using Android.App;
using Android.Content;
using System.Threading.Tasks;
using Android.OS;

namespace DroneLander.Droid
{
    [Activity(Label = "Drone Lander", Icon="@drawable/icon", Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : Activity
    {
        protected override void OnResume()
        {
            base.OnResume();

            var startupWork = new Task(async () =>
            {
                await Task.Delay(300);
                RunOnUiThread(() => StartActivity(new Intent(Application.Context, typeof(MainActivity))));
            });

            startupWork.Start();
        }
    }
}