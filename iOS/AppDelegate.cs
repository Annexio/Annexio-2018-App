using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using AppsFlyerXamarinBinding;
using UserNotifications;
using Newtonsoft.Json;

namespace AnnexioWebApp.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        private AppsFlyerTracker tracker = AppsFlyerTracker.SharedTracker();
        public static string AppsflyerUID;
     
        public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
        {
            #region Constructors
            public UserNotificationCenterDelegate()
            {
            }
            #endregion

            #region Override Methods
            public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
            {
                // Do something with the notification
                Console.WriteLine("Active Notification: {0}", notification);
                XPush.XPush.UserNotificationCenterWillPresentNotification(notification);
                // Tell system to display the notification anyway or use
                // `None` to say we have handled the display locally.
                completionHandler(UNNotificationPresentationOptions.Alert);
            }
            #endregion
        }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            LoadApplication(new App());

            tracker.AppsFlyerDevKey = ConfigSettings.AppsFlyerDevKey;
            tracker.AppleAppID = ConfigSettings.AppsFlyerAppId;

            int types;
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                types = (int)(UIUserNotificationType.Badge | UIUserNotificationType.Alert | UIUserNotificationType.Sound);
            }
            else
            {
                types = (int)(UIRemoteNotificationType.Alert | UIRemoteNotificationType.Sound | UIRemoteNotificationType.Badge);
            }
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                UNUserNotificationCenter center = UNUserNotificationCenter.Current;
                center.Delegate = new UserNotificationCenterDelegate();
            }

            XPush.XPush.SetInAppMessageEnabled(true);
            XPush.XPush.RegisterForRemoteNotificationTypes(types);
            XPush.XPush.SetSandboxModeEnabled(false); //Required if doing a debug/dev build of your app
            XPush.XPush.SetLocationEnabled(true); // Required for location or beacon functionality
            NSNotificationCenter.DefaultCenter.AddObserver(XPush.Constants.XPushMessageResponseReceiveNotification, MessageResponseReceived);
            NSNotificationCenter.DefaultCenter.AddObserver(XPush.Constants.XPushInboxBadgeChangeNotification, InboxBadgeUpdated);
            XPush.XPush.ApplicationDidFinishLaunchingWithOptions((options != null) ? options : new NSDictionary());

            return base.FinishedLaunching(app, options);
        }

        public void InboxBadgeUpdated(NSNotification nu)
        {
            Console.WriteLine("Inbox Badge Changed : " + XPush.XPush.InboxBadge);
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            XPush.XPush.ApplicationDidRegisterForRemoteNotificationsWithDeviceToken(deviceToken);
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            XPush.XPush.ApplicationDidFailToRegisterForRemoteNotificationsWithError(error);
        }

        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            Console.WriteLine("Debug: ApplicationDidReceiveLocalNotification");
            Console.WriteLine("Debug: ApplicationDidReceiveLocalNotification " + notification.AlertTitle);
            XPush.XPush.ApplicationDidReceiveLocalNotification(notification);
        }

        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            XPush.XPush.ApplicationDidReceiveRemoteNotification(userInfo);
        }

        void MessageResponseReceived(NSNotification notification)
        {
            Console.WriteLine("Debug: MessageResponseReceived" + notification.Name);
            XPush.XPMessageResponseNotification response = ((XPush.XPMessageResponseNotification)notification.Object);
            if(response.MessageType == "push" && response.MessagePayload["deeplink"] != null)
            {
                Console.WriteLine("Debug: MessageResponseReceived Deeplinking with " + response.MessagePayload["deeplink"].ToString());
                ((AnnexioWebAppPage)App.Current.MainPage).ConfigureDeeplink(response.MessagePayload["deeplink"].ToString());
            }
        }

        public override void OnActivated(UIApplication uiApplication)
        {
            tracker.TrackAppLaunch();
            AppsflyerUID = tracker.GetAppsFlyerUID();
            base.OnActivated(uiApplication);
        }
    }
}
