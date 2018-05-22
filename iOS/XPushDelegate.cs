using System;
using System.Collections;
using System.Collections.Generic;
using AVFoundation;
using Xamarin.Forms.Platform.iOS;
using Foundation;

[assembly: Xamarin.Forms.Dependency(typeof(AnnexioWebApp.iOS.XPushDelegate))]
namespace AnnexioWebApp.iOS
{
    public class XPushDelegate : IXPushDelegate
    {
        public XPushDelegate()
        {
        }

        public void HitEvent(String eventName)
        {
            XPush.XPush.HitEvent(eventName);
        }

        public void HitTag(String tagName)
        {
            XPush.XPush.HitTag(tagName);
        }

        public void HitTag(String tagName, String tagValue){
            XPush.XPush.HitTag(tagName, tagValue);
        }

        public void OpenInbox()
        {
            XPush.XPush.OpenInbox();
        }

        public void SetSubscription(Boolean sub)
        {
            XPush.XPush.SetSubscription(sub);
        }

        public void SetExternalId(String ext)
        {
            XPush.XPush.SetExternalId(ext);
        }



        public IDictionary<String, String> GetDeviceInfo()
        {
            IDictionary<String, String> genDict = new Dictionary<String, String>();
            NSDictionary mDict = XPush.XPush.DeviceInfo;
            foreach(object key in mDict.Keys){
                String s = mDict[key as NSString].ToString();
                genDict.Add(key.ToString(), s);
            }
            return genDict;
        }
    }  
}
