using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SuchByte.MacroDeck.Logging;

namespace airxiti.Euroscope
{
    internal class Stats
    {
        public string callsign { get; private set; } = "Offline";
        public TimeSpan elapsed { get; private set; } = TimeSpan.Zero;

        public async Task GetVatsimStatusAsync()
        {
            try
            {
                var vatsim_id = Configurator.Instance.VatsimId;
                MacroDeckLogger.Info(Main.Instance,$"Vatsim-ID: {vatsim_id}");
                if (string.IsNullOrWhiteSpace(vatsim_id))
                {
                    this.callsign = "No ID";
                    this.elapsed = TimeSpan.Zero;
                    return;
                }

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, "https://api.vatsim.net/v2/atc/online");
                request.Headers.Add("Accept", "application/json");
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();

                var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                
                if (root.ValueKind == JsonValueKind.Array)
                {
                    foreach (var element in root.EnumerateArray())
                    {
                        if (element.TryGetProperty("id", out var idProp) && idProp.GetInt32().ToString() == vatsim_id)
                        {
                            if (element.TryGetProperty("callsign", out var callsignProp))
                                this.callsign = callsignProp.GetString() ?? string.Empty;
                            else
                                this.callsign = "Empty";
                            MacroDeckLogger.Info(Main.Instance, $"VATSIM Status: {this.callsign}");
                            if (element.TryGetProperty("start", out var startProp) && startProp.GetString() is string start && !string.IsNullOrEmpty(start))
                            {
                                if (DateTime.TryParse(start, out var startDateTime))
                                    this.elapsed = DateTime.UtcNow - startDateTime.ToUniversalTime();
                                else
                                    this.elapsed = TimeSpan.Zero;
                            }
                            else
                            {
                                this.elapsed = TimeSpan.Zero;
                            }
                        }

                    }
                }

                
            }
            catch (Exception ex)
            {
                MacroDeckLogger.Error(Main.Instance, $"Error fetching VATSIM status: {ex.Message}");
            }
        }
    }
}
