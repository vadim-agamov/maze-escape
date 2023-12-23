using System;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<AudioClip> GetAllAudioClips() => _sounds.Select(s => s.Clip);
    }
}