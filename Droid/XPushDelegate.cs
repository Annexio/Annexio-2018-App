using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using IE.Imobile.Extremepush;
using Android.App;
using Plugin.CurrentActivity;
using Java.Util;
using System.Collections.Generic;
using Org.Json;

[assembly: Xamarin.Forms.Dependency(typeof(AnnexioWebApp.Droid.XPushDelegate))]
namespace AnnexioWebApp.Droid
{
    public class XPushDelegate : Java.Lang.Object, IXPushDelegate
    {
        public void HitEvent(String eventName){
            PushConnector.MPushConnector.HitEvent(eventName, "");
        }

        public void HitTag(String tagName)
        {
            PushConnector.MPushConnector.HitTag(tagName, "");
        }

        public void HitTag(String tagName, String tagValue)
        {
            PushConnector.MPushConnector.HitTag(tagName, tagValue);
        }

        public void OpenInbox(){
            Activity ac = CrossCurrentActivity.Current.Activity;
            PushConnector.MPushConnector.OpenInbox(CrossCurrentActivity.Current.Activity);
        }

        public void SetSubscription(Boolean sub){
            PushConnector.MPushConnector.SetSubscription(sub);
        }

        public void SetExternalId(String ext){
            PushConnector.MPushConnector.SetExternalId(CrossCurrentActivity.Current.Activity, ext);
        }

        public IDictionary<String,String> GetDeviceInfo(){
            IDictionary<String, String> mDict = PushConnector.MPushConnector.GetDeviceInfo(CrossCurrentActivity.Current.Activity);
            return mDict;
        }
    }
}

