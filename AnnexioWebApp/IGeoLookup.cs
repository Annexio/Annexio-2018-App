using System;
using System.Threading.Tasks;

namespace AnnexioWebApp
{
    public interface IGeoLookup
    {
        Task<String> GetCountryFromPosition(double latitude, double longitude);
    }
}
