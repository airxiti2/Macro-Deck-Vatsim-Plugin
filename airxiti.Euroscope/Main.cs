using System;
using SuchByte.MacroDeck.Plugins;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using SuchByte.MacroDeck.Variables;
using System.Xml.Linq;
using SuchByte.MacroDeck.Logging;
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

        public override void Enable()
        {
            while (true)
            {
                SetVariable(new VariableState { Name = "time_sec", Value = GetTimeInSeconds(), Type = VariableType.String, Save = false });
                System.Threading.Thread.Sleep(1000);
            }
            
        }
    }

}
