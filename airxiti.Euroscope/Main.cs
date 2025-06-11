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
using System.Collections;
using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.GUI;
#pragma warning disable CA1416

namespace airxiti.Euroscope
{
    public class Main : MacroDeckPlugin
    {
        public static Main Instance { get; private set; }

        public Main()
        {
            Instance = this;
        }
        public override bool CanConfigure => true;



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

        private Stats stats;
        private Metarfetcher metar;
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
            try
            {
                string lastMinute = null;
                await stats.GetVatsimStatusAsync();
                SetVariable(new VariableState { Name = "station", Value = $"{stats.callsign}", Type = VariableType.String, Save = true });
                while (!token.IsCancellationRequested)
                {
                    SetVariable(new VariableState { Name = "time_sec", Value = GetTimeInSeconds(), Type = VariableType.String, Save = false });

                    string currentMinute = DateTime.Now.ToString("yyyyMMddHHmm");
                    if (currentMinute != lastMinute)
                    {
                        lastMinute = currentMinute;
                        await stats.GetVatsimStatusAsync();
                        
                        try
                        {
                            foreach (var airport in Configurator.Instance.Airports)
                            {
                                var metarData = await Metarfetcher.fetch_metar(airport);
                                SetVariable(new VariableState { Name = $"metar_{airport.ToLower()}", Value = metarData, Type = VariableType.String, Save = true });
                            }
                        }
                        catch (Exception ex)
                        {
                            MacroDeckLogger.Error(this, $"Error fetching METAR data: {ex.Message}");
                        }
                        try { SetVariable(new VariableState { Name = "elapsed", Value = $"{stats.elapsed:hh\\:mm}", Type = VariableType.String, Save = true }); }
                        catch (Exception ex)
                        {
                            MacroDeckLogger.Error(this, $"Error setting elapsed time variable: {ex.Message}");
                        }
                    }
                    await Task.Delay(1000, token);
                }
            }
            catch (Exception ex)
            {
                MacroDeckLogger.Info(this, $"UpdateLoopAsync Exception: {ex}");     
            }
        }


        public override void Enable()
        {

            this.Actions = new List<PluginAction>
            {
                new ResetPlugin()
            };

            if (_cts != null)
            {
                _cts.Cancel();
                try
                {
                    Task.Delay(200).Wait();
                }
                catch { }
                _cts.Dispose();
                _cts = null;
            }

            stats = new Stats();
            
            
            _cts = new CancellationTokenSource();
            _ = UpdateLoopAsync(_cts.Token);
        }

        public override void OpenConfigurator()
        {
            using (var configuratorForm = new EuroscopeConfiguratorForm())
            {
                configuratorForm.ShowDialog();
            }
        }

        public class ResetPlugin : PluginAction
        {
            public override string Name => "Reset";
            public override string Description => "Reset Plugin";
            public override void Trigger(string clientId, ActionButton actionButton)
            {
                MacroDeckLogger.Info(Instance, "Resetting Euroscope Plugin");
                Instance?.Enable();
            }
        }
    }
}
