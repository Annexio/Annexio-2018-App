using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using System.Threading;

namespace AnnexioWebApp.Droid
{
    [Activity(Label = "@string/AppName", 
        Icon = "@mipmap/ic_launcher",
        RoundIcon = "@mipmap/ic_launcher_round",
        Theme = "@style/App.Splash",
        MainLauncher = true, 
        NoHistory = true,
        LaunchMode = LaunchMode.SingleTask,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RunOnUiThread(() =>
            {
                SetContentView(Resource.Layout.splash_layout);
            });
        }

        protected override void OnResume()
        {
            base.OnResume();

            new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(1000);
                RunOnUiThread(() =>
                {
                    StartActivity(typeof(MainActivity));
                    Finish();
                });
            })).Start();
        }
    }
}