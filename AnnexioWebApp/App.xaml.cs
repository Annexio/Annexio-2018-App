using Xamarin.Forms;

namespace AnnexioWebApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AnnexioWebAppPage();
        }

        protected override void OnStart()
        {
            ((AnnexioWebAppPage)MainPage).LoadDesignatedSite();
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
