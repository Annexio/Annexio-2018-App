using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using Foundation;
using UIKit;
using AnnexioWebApp.IOS.Renderers;
using System.Linq;
using AnnexioWebApp.iOS;

[assembly: ExportRenderer(typeof(WebView), typeof(IOSWebViewRender))]
namespace AnnexioWebApp.IOS.Renderers
{    
    public class IOSWebViewRender : ViewRenderer<WebView, UIWebView>, INSUrlConnectionDelegate
    {        
        private static UIWebView webViewStore;

        private void Control_LoadError(object sender, UIWebErrorArgs e)
        {
            string errmsg = "Request to server timed out";

            ((AnnexioWebAppPage)App.Current.MainPage).DisplayErrorMessage(errmsg);
        }


        public void LoadFinished(object sender, System.EventArgs e)
        {
            if (firstLoad && !Control.IsLoading)
            {
                firstLoad = false;
                ((AnnexioWebAppPage)App.Current.MainPage).FirstLoadComplete();
            }
        }


        private bool firstLoad = true;

        private bool shouldStartLoad(UIWebView wv, NSUrlRequest req, UIWebViewNavigationType wnav)
        {
            if (System.Uri.TryCreate(req.Url.AbsoluteString, System.UriKind.RelativeOrAbsolute, out System.Uri result))
            {
                foreach(string item in ConfigSettings.ExternalExceptions)
                {
                    if(result.Host.Contains(item))
                    {
                        Device.OpenUri(result);
                        return false;       
                    }
                }
            }
            if (req.Url.AbsoluteString.Contains("about:blank"))
                return false;
            var headerKey = new NSString("X-App-Type");
            if (!req.Headers.ContainsKey(headerKey))
            {
                if (((AnnexioWebAppPage)App.Current.MainPage).IsHeaderRequired(req.Url.AbsoluteString))
                {
                    var copy = req.MutableCopy() as NSMutableUrlRequest;
                    NSMutableDictionary dic = new NSMutableDictionary();
                    dic.Add(headerKey, new NSString("IOS"));
                    copy.Headers = dic;
                    string finalUrl = ((AnnexioWebAppPage)App.Current.MainPage).AppendAppsflyerParam(copy.Url.ToString(), AppDelegate.AppsflyerUID);
                    copy.Url = new NSUrl(finalUrl);


                    ((UIWebView)wv).LoadRequest(copy);
                    return false;
                }
            }
            return true;
        }

        private void NewElement_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Source")
            {
                UrlWebViewSource source = (Xamarin.Forms.UrlWebViewSource)Element.Source;
                var webRequest = new NSMutableUrlRequest(new NSUrl(source.Url));
                webRequest.TimeoutInterval = 20;
                Control.LoadError += Control_LoadError;
                Control.LoadFinished += LoadFinished;
                this.Control.LoadRequest(webRequest);
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            var webView = Control as UIWebView;

            if (webView == null)
            {
                if(webViewStore != null)
                {
                    webView = webViewStore;
                }
                else{
                    webView = new UIWebView();
                }
                SetNativeControl(webView);
                webViewStore = webView;
            }

            if (e.OldElement != null)
            {
                webView.ShouldStartLoad -= shouldStartLoad;
            }

            if (e.NewElement != null)
            {
                webView.ShouldStartLoad += shouldStartLoad;
                e.NewElement.PropertyChanged += NewElement_PropertyChanged;
            }
                 
        }
    }
}