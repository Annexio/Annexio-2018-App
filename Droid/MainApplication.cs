using System;

using Android.App;
using Android.OS;
using Android.Runtime;
using Plugin.CurrentActivity;
using IE.Imobile.Extremepush.Api.Model;
using IE.Imobile.Extremepush;
using Android.Util;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AnnexioWebApp.Droid
{
	//You can specify additional application information in this attribute
    [Application]
    public class MainApplication : Application, Application.IActivityLifecycleCallbacks, IMessageResponseListener, IInboxBadgeUpdateListener
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer)
          :base(handle, transer)
        {
        
        }

        public void MessageResponseReceived(string messageType, PushMessage messagePayload,
                                 string responseType, IDictionary<string, string> responsePayload,
                                 Java.Lang.Ref.WeakReference activityHolder)
        {
            // Handle a message response
            Log.Debug("MessageResponseReceived", messageType);
            if (messagePayload != null)
                Log.Debug("MessageResponseReceived", messagePayload.ToJson());
            Log.Debug("MessageResponseReceived", responseType);
            if (responsePayload != null)
            {
                foreach (KeyValuePair<string, string> keypair in responsePayload)
                    Log.Debug("MessageResponseReceived", keypair.Key + " - " + keypair.Value);
            }
            if (messageType == "push" && !string.IsNullOrEmpty(messagePayload.Url))
            {
                Log.Debug("Debug: MessageResponseReceived Deeplinking with ", messagePayload.Url);
                ((AnnexioWebAppPage)App.Current.MainPage).ConfigureDeeplink(messagePayload.Url);
            }
            Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(messagePayload.ToJson());
            if (messageType == "push" && dict.ContainsKey("deeplink"))
            {
                ((AnnexioWebAppPage)App.Current.MainPage).ConfigureDeeplink(dict["deeplink"]);
            }
            
        }

        public void InboxBadgeUpdated(int i, Java.Lang.Ref.WeakReference activityHolder)
        {
            //print out inbox badge 
            Log.Debug("Inbox Badge is : ", i + "");
        }
        public override void OnCreate()
        {
            base.OnCreate();
            RegisterActivityLifecycleCallbacks(this);
            string XPushAppKey = ConfigSettings.XPushAppKey;
            string GoogleProjectNumber = ConfigSettings.GoogleProjectNumber;

            new PushConnector.Builder(XPushAppKey, GoogleProjectNumber)
                             .SetMessageResponseListener(this)
                             .SetEnableInApp(true)
                             .SetEnableStartSession(true)
                             .TurnOnDebugMode(false)
                             .SetInboxBadgeUpdateListener(this)
                             .Create(this);
        }

        public override void OnTerminate()
        {
            base.OnTerminate();
            UnregisterActivityLifecycleCallbacks(this);
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityDestroyed(Activity activity)
        {
        }

        public void OnActivityPaused(Activity activity)
        {
        }

        public void OnActivityResumed(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
        }

        public void OnActivityStarted(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityStopped(Activity activity)
        {
        }
    }
}