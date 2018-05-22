using Android.App;
using Android.Content.PM;
using Android.Views;
using Android.OS;
using Com.Appsflyer;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Android.Net;
using Android.Content;
using Android.Runtime;

namespace AnnexioWebApp.Droid
{
    [Activity(Label = "@string/AppName",
        Icon = "@mipmap/ic_launcher",
        RoundIcon = "@mipmap/ic_launcher_round",
        Theme = "@style/App.Default.Theme", 
        MainLauncher = false,
        LaunchMode = LaunchMode.SingleTask, 
        AlwaysRetainTaskState = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private Xamarin.Forms.Application appInstance;
        public static string AppsflyerUID;

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
		{
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            appInstance = new App();
            LoadApplication(appInstance);
            AppsFlyerLib.Instance.StartTracking(Application, ConfigSettings.AppsFlyerDevKey);
            AppsflyerUID = AppsFlyerLib.Instance.GetAppsFlyerUID(Application.Context);
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity = this;
        }

        protected override void OnResume()
        {
            base.OnResume();

            CheckNetworkConnection();
        }

        public override bool OnKeyDown(Android.Views.Keycode keyCode, Android.Views.KeyEvent e)
        {
            var pageInstance = ((AnnexioWebAppPage)appInstance.MainPage);
            if (keyCode == Keycode.Back && pageInstance.CanGoBack)
            {
                pageInstance.GoBack();
                return true; //Prevent the default key down closing the app
            }
            return base.OnKeyDown(keyCode, e);
        }

        protected void CheckNetworkConnection()
        {
            var con_manager = (ConnectivityManager)GetSystemService(Context.ConnectivityService);
            if (!(con_manager.ActiveNetworkInfo != null
                && con_manager.ActiveNetworkInfo.IsAvailable
                && con_manager.ActiveNetworkInfo.IsConnected)) {
                ((AnnexioWebAppPage)appInstance.MainPage).DisplayErrorMessage("No network connection available");
            }
        }
    }
}
