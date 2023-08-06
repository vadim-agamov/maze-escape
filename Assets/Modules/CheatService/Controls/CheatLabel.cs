using System;
using UnityEngine;

namespace Modules.CheatService.Controls
{
    public class CheatLabel
    {
        private readonly Func<string> _name;

        public CheatLabel(Func<string> name)
        {
            _name = name;
        }
        
        public void OnGUI()
        {
            GUILayout.Label(_name.Invoke());
        }
    }
}