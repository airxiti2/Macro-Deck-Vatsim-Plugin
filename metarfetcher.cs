using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace airxiti.Vatsim
{
    public class Metarfetcher
    {
        public static async Task<string> fetch_metar(string icao)
        {
            var client = new HttpClient();
            var url = $"https://metar.vatsim.net/{icao}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "text/plain");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
