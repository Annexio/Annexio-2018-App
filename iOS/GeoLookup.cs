using System.Linq;
using System.Threading.Tasks;
using CoreLocation;
using Xamarin.Forms;

[assembly: Dependency(typeof(AnnexioWebApp.iOS.GeoLookup))]
namespace AnnexioWebApp.iOS
{
    public class GeoLookup : IGeoLookup
    {
        public async Task<string> GetCountryFromPosition(double latitude, double longitude)
        {
            var geoCoder = new CLGeocoder();
            var location = new CLLocation(latitude, longitude);
            var placemarks = await geoCoder.ReverseGeocodeLocationAsync(location);
            if (placemarks.Any())
            {
                var pm = placemarks[0];
                return pm.IsoCountryCode;
            }
            return null;   
        }

        public GeoLookup()
        {
        }
    }
}
