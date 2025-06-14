using System;
using SuchByte.MacroDeck.Plugins;
using System.Collections.Generic;
using SuchByte.MacroDeck.Variables;
using SuchByte.MacroDeck.Logging;
using System.Threading.Tasks;
using System.Threading;
using SuchByte.MacroDeck.ActionButton;


namespace airxiti.Vatsim
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
            VariableManager.SetValue(string.Format("vatsim_{0}", variableState.Name), variableState.Value, variableState.Type, this, variableState.Save);
        }
        public static string GetTimeInSeconds()
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
                SetVariable(new VariableState { Name = "station", Value = $"{stats.Callsign}", Type = VariableType.String, Save = true });
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
                            foreach (var airport in Configurator.Instance.MetarAirports)
                            {
                                var metarData = await Metarfetcher.FetchMetar(airport);
                                SetVariable(new VariableState { Name = $"metar_{airport.ToLower()}", Value = metarData, Type = VariableType.String, Save = true });
                            }
                        }
                        catch (Exception ex)
                        {
                            MacroDeckLogger.Error(this, $"Error fetching METAR data: {ex.Message}");
                        }
                        try
                        {
                            foreach (var airport in Configurator.Instance.AtisAirports)
                            {
                                var atisData = await VatisFetcher.GetAtis(airport);
                                var activeRunways = VatisFetcher.GetRunways.ExtractRunways(atisData[1]);
                                SetVariable(new VariableState { Name = $"atis_{airport.ToLower()}_letter", Value = atisData[0], Type = VariableType.String, Save = true });
                                SetVariable(new VariableState { Name = $"atis_{airport.ToLower()}_text", Value = atisData[1], Type = VariableType.String, Save = true });
                                SetVariable(new VariableState { Name = $"atis_{airport.ToLower()}_runways", Value = string.Join(",", activeRunways), Type = VariableType.String, Save = true });
                            }
                        }
                        catch (Exception ex)
                        {
                            MacroDeckLogger.Error(this, $"Error fetching ATIS data: {ex.Message}");
                        }
                        try { SetVariable(new VariableState { Name = "elapsed", Value = $"{stats.Elapsed:hh\\:mm}", Type = VariableType.String, Save = true }); }
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
                new ReloadPlugin()
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
            using var configuratorForm = new VatsimConfiguratorForm();
            configuratorForm.ShowDialog();
        }

        public class ReloadPlugin : PluginAction
        {
            public override string Name => "Reload Plugin";
            public override string Description => "Reloads the Vatsim Plugin";
            public override void Trigger(string clientId, ActionButton actionButton)
            {
                MacroDeckLogger.Info(Instance, "Reloading Vatsim Plugin");
                Instance?.Enable();
            }
        }
    }
}
