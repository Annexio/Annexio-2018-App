using System.Threading.Tasks;
using Android.Locations;
using System.Linq;
using Xamarin.Forms;

[assembly: Dependency(typeof(AnnexioWebApp.Droid.GeoLookup))]
namespace AnnexioWebApp.Droid
{
    public class GeoLookup : IGeoLookup
    {
        public async Task<string> GetCountryFromPosition(double latitude, double longitude)
        {
            var geo = new Geocoder(Android.App.Application.Context);
            var addresses = await geo.GetFromLocationAsync(latitude, longitude, 1);
            if (addresses.Any())
                return addresses[0].CountryCode;
            return null;
        }

        public GeoLookup()
        {
        }
    }
}
