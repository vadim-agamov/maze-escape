using System;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.SoundService
{
    [CreateAssetMenu(fileName = "SoundsConfig", menuName = "Configs/SoundsConfig", order = 0)]
    public class SoundsConfig : ScriptableObject
    {
        [Serializable]
        public class Sound
        {
            public string Id;
            public AudioClip Clip;
        }

        [SerializeField]
        private List<Sound> _sounds;
        
        public AudioClip GetSound(string id) => _sounds.Find(s => s.Id == id).Clip;
    }
}