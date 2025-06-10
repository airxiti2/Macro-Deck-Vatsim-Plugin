using System;
using SuchByte.MacroDeck.Plugins;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using SuchByte.MacroDeck.Variables;
using System.Xml.Linq;
using SuchByte.MacroDeck.Logging;
using RestSharp;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public override async void Enable()
        {
            System.Threading.Timer timer = null;
            timer = new System.Threading.Timer(_ =>
            {
                SetVariable(new VariableState { Name = "time_sec", Value = GetTimeInSeconds(), Type = VariableType.String, Save = false });
            }, null, 0, 1000);

            var options = new RestClientOptions("")
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("https://metar.vatsim.net/:EDDB", Method.Get);
            request.AddHeader("Accept", "text/plain");
            RestResponse response = await client.ExecuteAsync(request);
            var metar_eddb = response.Content;
            SetVariable(new VariableState { Name = "metar_eddb", Value = metar_eddb, Type = VariableType.String, Save = true });
        }
    }

}
