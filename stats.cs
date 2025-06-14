using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SuchByte.MacroDeck.Logging;

namespace airxiti.Vatsim
{
    internal class Stats
    {
        public string Callsign { get; private set; } = "Offline";
        public TimeSpan Elapsed { get; private set; } = TimeSpan.Zero;

        public async Task GetVatsimStatusAsync()
        {
            try
            {
                var vatsimId = Configurator.Instance.VatsimId;
                MacroDeckLogger.Info(Main.Instance,$"Vatsim-ID: {vatsimId}");
                if (string.IsNullOrWhiteSpace(vatsimId))
                {
                    this.Callsign = "No ID";
                    this.Elapsed = TimeSpan.Zero;
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
                        if (element.TryGetProperty("id", out var idProp) && idProp.GetInt32().ToString() == vatsimId)
                        {
                            if (element.TryGetProperty("callsign", out var callsignProp))
                                this.Callsign = callsignProp.GetString() ?? string.Empty;
                            else
                                this.Callsign = "Empty";
                            MacroDeckLogger.Info(Main.Instance, $"VATSIM Status: {this.Callsign}");
                            if (element.TryGetProperty("start", out var startProp) && startProp.GetString() is { } start && !string.IsNullOrEmpty(start))
                            {
                                if (DateTime.TryParse(start, out var startDateTime))
                                    this.Elapsed = DateTime.UtcNow - startDateTime.ToUniversalTime();
                                else
                                    this.Elapsed = TimeSpan.Zero;
                            }
                            else
                            {
                                this.Elapsed = TimeSpan.Zero;
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
