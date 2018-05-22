using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Android.Content;
using Android.Views;
using Android.Webkit;
using AnnexioWebApp.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Xamarin.Forms.WebView), typeof(DroidWebViewRender))]
namespace AnnexioWebApp.Droid.Renderers
{
    public class DroidWebViewRender : ViewRenderer<Xamarin.Forms.WebView, Android.Webkit.WebView>
    {
        private Dictionary<string, string> customHeaders;
        void NewElement_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Source")
            {
                UrlWebViewSource source = Element.Source as UrlWebViewSource;
                if (source != null)
                {
                    Control.LoadUrl(source.Url, customHeaders);
                }

            }
        }

        public DroidWebViewRender(Context context) : base(context) {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.WebView> e)
        {
            base.OnElementChanged(e);
            
            Android.Webkit.WebView webView = Control as Android.Webkit.WebView;
            if (Control == null)
            {
                webView = new Android.Webkit.WebView(base.Context);
                SetNativeControl(webView);
            }

            customHeaders = new Dictionary<string, string>
            {
                ["X-App-Type"] = "Android"
            };

            webView.Settings.JavaScriptEnabled = true;

            webView.Settings.BuiltInZoomControls = true;
            webView.Settings.SetSupportZoom(true);

            webView.ScrollBarStyle = ScrollbarStyles.OutsideOverlay;
            webView.ScrollbarFadingEnabled = false;

            webView.SetLayerType(LayerType.Software, null);
            webView.Settings.SetPluginState(state: Android.Webkit.WebSettings.PluginState.On);
            webView.LayoutParameters = new Android.Widget.RelativeLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);

            webView.SetWebViewClient(new DroidWebViewClient(customHeaders));

            if (e.NewElement != null)
            {
                e.NewElement.PropertyChanged += NewElement_PropertyChanged;
            }
        }
    }

    public class DroidWebViewClient : Android.Webkit.WebViewClient
    {
        private bool firstLoad = true;
        private Dictionary<string, string> customHeaders;

        public DroidWebViewClient(Dictionary<string, string> requestHeaders)
        {
            customHeaders = requestHeaders;
        }


        /* For older versions of Android */
        public override bool ShouldOverrideUrlLoading(Android.Webkit.WebView View, string url)
        {
            if(System.Uri.TryCreate(url, System.UriKind.RelativeOrAbsolute, out System.Uri result))
            {
                if (ConfigSettings.ExternalExceptions.Any(url.Contains))
                {
                    Device.OpenUri(result);
                    return true;
                }
            }
            var pageInstance = ((AnnexioWebAppPage)App.Current.MainPage);
            string finalUrl = pageInstance.AppendAppsflyerParam(url, MainActivity.AppsflyerUID);
            pageInstance.AddToHistory(finalUrl);

            if (pageInstance.IsHeaderRequired(url))
            {
                View.LoadUrl(finalUrl, customHeaders);
            }
            else
            {
                View.LoadUrl(finalUrl);
            }
            return true;
        }

        public override bool ShouldOverrideUrlLoading(Android.Webkit.WebView View, Android.Webkit.IWebResourceRequest request)
        {
            if (System.Uri.TryCreate(request.Url.ToString(), System.UriKind.RelativeOrAbsolute, out System.Uri result))
            {
                if (ConfigSettings.ExternalExceptions.Any(request.Url.ToString().Contains))
                {
                    Device.OpenUri(result);
                    return true;
                }
            }
            var pageInstance = ((AnnexioWebAppPage)App.Current.MainPage);
            string finalUrl = pageInstance.AppendAppsflyerParam(request.Url.ToString(), MainActivity.AppsflyerUID);
            pageInstance.AddToHistory(finalUrl);
            if (pageInstance.IsHeaderRequired(request.Url.ToString()))
            {
                View.LoadUrl(finalUrl, customHeaders);
            }
            else
            {
                View.LoadUrl(finalUrl);
            }
            return true;
        }

        public override void OnPageStarted(Android.Webkit.WebView view, string url, Android.Graphics.Bitmap favicon)
        {
            base.OnPageStarted(view, url, favicon);
        }

        public override void OnPageFinished(Android.Webkit.WebView view, string url)
        {
            base.OnPageFinished(view, url);

            if (firstLoad)
            {
                firstLoad = false;
                ((AnnexioWebAppPage)App.Current.MainPage).FirstLoadComplete();
            }
        }

        public override void OnReceivedError(Android.Webkit.WebView view, IWebResourceRequest request, WebResourceError error)
        {
            base.OnReceivedError(view, request, error);
            var pageInstance = ((AnnexioWebAppPage)App.Current.MainPage);

            //Set firstLoad to true to enable loader again
            firstLoad = true;

            //Switch the view to display the error (message override optional)
            if (pageInstance.IsHeaderRequired(request.Url.ToString()))
            {
                ((AnnexioWebAppPage)App.Current.MainPage).DisplayErrorMessage(error.ErrorCode == ClientError.Timeout
                                                                                                    ? "Request to server timed out"
                                                                                                                : null);
            }
        }

        public override void OnReceivedHttpError(Android.Webkit.WebView view, IWebResourceRequest request, WebResourceResponse errorResponse)
        {
            base.OnReceivedHttpError(view, request, errorResponse);
            var pageInstance = ((AnnexioWebAppPage)App.Current.MainPage);

            //Set firstLoad to true to enable loader again
            firstLoad = true;

            //Switch the view to display the error (message override optional)
            if (pageInstance.IsHeaderRequired(request.Url.ToString()))
            {
                ((AnnexioWebAppPage)App.Current.MainPage).DisplayErrorMessage(errorResponse.StatusCode == (int)HttpStatusCode.RequestTimeout
                                                                              ? "Request to server timed out"
                                                                                                                                                                       : null);
            }
        }
    }
}