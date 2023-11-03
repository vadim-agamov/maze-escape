using System;
using UnityEngine;

namespace Modules.CheatService.Controls
{
    public class CheatButton: CheatControl
    {
        private readonly string _name;
        private readonly Action _action;
        private readonly bool _hideCheats;
        
        public CheatButton(ICheatService cheatService, string name, Action action, bool hideCheats = true): base(cheatService)
        {
            _name = name;
            _action = action;
            _hideCheats = hideCheats;
        }
        
        public void OnGUI()
        {
            if (GUILayout.Button(_name))
            {
                _action?.Invoke();
                if(_hideCheats)
                {
                    HideCheats();
                }
            }
        }
    }
}