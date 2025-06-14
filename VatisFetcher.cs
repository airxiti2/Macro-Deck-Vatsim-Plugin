using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace airxiti.Vatsim
{
    public class VatisFetcher
    {
        public static async Task<string[]> GetAtis(string airport)
        {
            using var ws = new ClientWebSocket();
            var uri = new Uri("ws://127.0.0.1:49082");
            await ws.ConnectAsync(uri, CancellationToken.None);

            var request = new
            {
                type = "getAtis",
                value = new
                {
                    station = airport
                }
            };
            var json = JsonSerializer.Serialize(request);
            var buffer = Encoding.UTF8.GetBytes(json);
            await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

            var receiveBuffer = new byte[4096];
            var result = await ws.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
            var response = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
            using JsonDocument document = JsonDocument.Parse(response);
            var root = document.RootElement;
            var atisLetter = root.GetProperty("value").GetProperty("atisLetter").GetString();
            var textAtis = root.GetProperty("value").GetProperty("textAtis").GetString();
            return [atisLetter, textAtis];
        }
    }
}
