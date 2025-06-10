using System;
using SuchByte.MacroDeck.Plugins;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using SuchByte.MacroDeck.Variables;
using System.Xml.Linq;
using SuchByte.MacroDeck.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;

namespace airxiti.Euroscope
{
    public class Main : MacroDeckPlugin
    {
        public class VariableState
        {
            public string Name { get; set; }
            protected VariableType _type = VariableType.Bool;
            public VariableType Type { get { return _type; } set { _type = value; } }
            protected object _value = false;
            public object Value { get { return _value; } set { _value = value; } }
            protected bool _save = true;
            public bool Save { get { return _save; } set { _save = value; } }
        }

        public void SetVariable(VariableState variableState)
        {
            VariableManager.SetValue(string.Format("Euroscope_{0}", variableState.Name), variableState.Value, variableState.Type, this, variableState.Save);
        }
        public string GetTimeInSeconds()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }

        private CancellationTokenSource _cts;

        private async Task UpdateLoopAsync(CancellationToken token)
        {
            string lastMinute = null;
            while (!token.IsCancellationRequested)
            {
                // Jede Sekunde: Zeitvariable aktualisieren
                SetVariable(new VariableState { Name = "time_sec", Value = GetTimeInSeconds(), Type = VariableType.String, Save = false });

                // Jede Minute: METAR abrufen und speichern
                string currentMinute = DateTime.Now.ToString("yyyyMMddHHmm");
                if (currentMinute != lastMinute)
                {
                    lastMinute = currentMinute;
                    var metar_eddb = await EDDB_metar();
                    SetVariable(new VariableState { Name = "metar_eddb", Value = metar_eddb, Type = VariableType.String, Save = true });
                }

                await Task.Delay(1000, token);
            }
        }

        public override void Enable()
        {
            _cts = new CancellationTokenSource();
            _ = UpdateLoopAsync(_cts.Token);
        }

        static async Task<string> EDDB_metar()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://metar.vatsim.net/EDDB");
            request.Headers.Add("Accept", "text/plain");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();

        }
    }

}
