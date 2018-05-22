using System;
using System.Collections.Generic;


namespace AnnexioWebApp
{
    public interface IXPushDelegate
    {
        void HitEvent(String eventName);
        void HitTag(String tagName);
        void HitTag(String tagName, String tagValue);
        void OpenInbox();
        void SetSubscription(Boolean sub);
        void SetExternalId(String ext);

        IDictionary<String, String> GetDeviceInfo();
    }
}