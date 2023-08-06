using System;
using UnityEngine;

namespace Modules.CheatService.Controls
{
    public class CheatIntInput: CheatControl
    {
        private readonly string _name;
        private int _value;
        private bool _isDirty;
        private readonly Action<int> _action;
        private readonly Func<int> _getValue;

        public CheatIntInput(ICheatService cheatService, string name, Func<int> getValue, Action<int> action) : base(cheatService)
        {
            _name = name;
            _action = action;
            _getValue = getValue;
        }
        
        public void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(_name);
            if (GUILayout.Button("-10"))
            {
                _isDirty = true;
                _value-=10;
            }
            if (GUILayout.Button("-1"))
            {
                _isDirty = true;
                _value--;
            }
            int.TryParse(GUILayout.TextField(_isDirty ? _value.ToString() : _getValue.Invoke().ToString()), out _value);
            if (GUILayout.Button("+1"))
            {
                _isDirty = true;
                _value++;
            }
            if (GUILayout.Button("+10"))
            {
                _isDirty = true;
                _value+=10;
            }
            if (GUILayout.Button(">"))
            {
                _isDirty = false;
                _action?.Invoke(_value);
                HideCheats();
            }
            GUILayout.EndHorizontal();
        }
    }
}