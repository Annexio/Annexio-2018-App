using Xamarin.Forms;
using System.Linq;
using System.Collections.Generic;
using System;
using Plugin.Geolocator;
using System.Threading.Tasks;

namespace AnnexioWebApp
{
    public partial class AnnexioWebAppPage : ContentPage
    {
        private WebView localWebView;
        private LoaderView preloaderView;
        private List<string> webHistory;
        private string deepLinkUrl = string.Empty;
        private bool siteWasLoadedInitially = false;

        public void ConfigureDeeplink(string deepLinkURL)
        {
            deepLinkUrl = deepLinkURL;
            AddToHistory(deepLinkURL);
            LoadURL(deepLinkURL);
            siteWasLoadedInitially = true;
        }

        public AnnexioWebAppPage()
        {
            InitializeComponent();
            preloaderView = new LoaderView();
            preloaderView.ReloadLastPageEvent += this.ReLoadURL;
            localWebView = preloaderView.FindByName<WebView>("annexioView");
            localWebView.Navigated += this.OnNavigated;
            Content = preloaderView;
            webHistory = new List<string>();
        }

        public async void FirstLoadComplete()
        {
            bool wasBlocked = await CheckGeo();
            if (wasBlocked) return;
            preloaderView.FindByName<ContentView>("overlayView").IsVisible = false;       
        }

        public void OnNavigated(object sender, WebNavigatedEventArgs e)
        {
            FirstLoadComplete();
        }
       
        public bool CanGoBack
        {
            get
            {
                return webHistory.Count > 1;
            }
        }

        public void GoBack()
        {
            if (webHistory.Count > 1)
            {
                webHistory.RemoveAt(webHistory.Count - 1);
                LoadURL(webHistory[webHistory.Count - 1]);
            }
        }

        public void LoadURL(string url)
        {
            preloaderView.FindByName<WebView>("annexioView").Source = url;
        }

        public bool IsHeaderRequired(string url)
        {
            if (System.Uri.TryCreate(url, System.UriKind.RelativeOrAbsolute, out System.Uri result))
            {
                return result.OriginalString.StartsWith(ConfigSettings.AppUrl, System.StringComparison.CurrentCultureIgnoreCase);
            }
            return true;    
        }

        public async Task<bool> CheckGeo()
        {
            if (CrossGeolocator.Current.IsGeolocationAvailable
      && CrossGeolocator.Current.IsGeolocationEnabled)
            {
                var position = await CrossGeolocator.Current.GetLastKnownLocationAsync();
                if (position != null)
                {
                    var isoCountry =
                        await DependencyService.Get<IGeoLookup>().GetCountryFromPosition(
                            position.Latitude, position.Longitude);
                    if (isoCountry != null && !ConfigSettings.PermittedCountries.Contains(isoCountry.ToUpper())) {
                        preloaderView.FindByName<Button>("retryButton").IsVisible = false;
                        DisplayErrorMessage("Sorry but this application cannot be accessed from your location.");
                        return true;
                    }
                }
            }
            return false;
        }

        public void LoadDesignatedSite()
        {                                        
            if (String.IsNullOrEmpty(deepLinkUrl))
            {
                preloaderView.FindByName<WebView>("annexioView").Source = ConfigSettings.AppUrl;
            }
            else
            {                
                preloaderView.FindByName<WebView>("annexioView").Source = deepLinkUrl;
            }
            siteWasLoadedInitially = true;
        }

        public string AppendAppsflyerParam(string url, string AppsflyerUID)
        {
            return ConfigSettings.AppsFlyerWebParamsEnabled ? AppendQueryStringParam(url, "appsFlyerId", AppsflyerUID) : url;
        }
        public string AppendQueryStringParam(string url, string paramKey, string paramValue)
        {
            return url + (url.Contains("?") ?  "&" : "?") + $"{paramKey}={paramValue}";
        }

        public void AddToHistory(string url)
        {
            if (!url.Contains("Launch") && 
                url.StartsWith(ConfigSettings.AppUrl, StringComparison.CurrentCultureIgnoreCase)) {
                webHistory.Add(url);
            }
        }

        public void DisplayErrorMessage(string errorMessage = "Unknown")
        {
            preloaderView.FindByName<Label>("errorMessageReason").Text = errorMessage;
            preloaderView.FindByName<ContentView>("overlayView").IsVisible = false;
            preloaderView.FindByName<ContentView>("errorView").IsVisible = true;
        }

        public void ReLoadURL(object sender, EventArgs args)
        {
            preloaderView.FindByName<ContentView>("errorView").IsVisible = false;
            preloaderView.FindByName<ContentView>("overlayView").IsVisible = true;

            LoadURL(webHistory.Count > 0 ? webHistory[webHistory.Count - 1] : ConfigSettings.AppUrl);
        }

    }
}
