using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace AnnexioWebApp
{
    public static class ConfigSettings
    {
        public static string AppUrl => GetSetting<string>("APP_URL");
        public static string AppsFlyerDevKey => GetSetting<string>("APPSFLYER_DEV_KEY");
        public static string AppsFlyerGcmProjectNumber => GetSetting<string>("APPSFLYER_GCM_PROJECT_NUMBER");
        public static string AppsFlyerAppId => GetSetting<string>("APPSFLYER_APP_ID");
        public static string XPushAppKey => GetSetting<string>("XPUSH_APP_KEY");
        public static string GoogleProjectNumber => GetSetting<string>("GOOGLE_PROJECT_NUMBER");
        public static List<string> ExternalExceptions => GetSetting<List<string>>("EXTERNAL_EXCEPTIONS");
        public static List<string> PermittedCountries => GetSetting<List<string>>("PERMITTEDJURISDICTIONS");
        public static bool AppsFlyerWebParamsEnabled => GetSetting<bool>("APPSFLYER_WEB_PARAMS_ENABLED");

        public static T GetSetting<T>(string settingType)
        {
            using (var stream = IntrospectionExtensions
                .GetTypeInfo(typeof(ConfigSettings))
                .Assembly
                .GetManifestResourceStream("AnnexioWebApp.config.xml"))
            using (var reader = new StreamReader(stream))
            {
                var doc = XDocument.Parse(reader.ReadToEnd());
                var element = doc.Element("config").Element(settingType);
                if (element == null) { return default(T); }
                if(settingType == "EXTERNAL_EXCEPTIONS" || settingType == "PERMITTEDJURISDICTIONS")
                    return (T)Convert.ChangeType(new List<string>(element.Value.Split(',')), typeof(T));
                else 
                    return (T)Convert.ChangeType(element.Value, typeof(T));
            }
        }
    }
}
