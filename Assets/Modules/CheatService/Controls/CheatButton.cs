using System;
using UnityEngine;

namespace Modules.CheatService.Controls
{
    public class CheatButton: CheatControl
    {
        private readonly string _name;
        private readonly Action _action;
        
        public CheatButton(ICheatService cheatService, string name, Action action): base(cheatService)
        {
            _name = name;
            _action = action;
        }
        
        public void OnGUI()
        {
            if (GUILayout.Button(_name))
            {
                _action?.Invoke();
                HideCheats();
            }
        }
    }
}