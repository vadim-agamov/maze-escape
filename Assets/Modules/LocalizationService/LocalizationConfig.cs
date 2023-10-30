using System;
using Modules.PlatformService;
using UnityEngine;

namespace Modules.LocalizationService
{
    [Serializable]
    public class LocalizationKey
    {
        [SerializeField] 
        private string _key;

        [SerializeField] 
        private string _value;
        
        public LocalizationKey(string key, string value)
        {
            _key = key;
            _value = value;
        }
        public string Key => _key;
        public string Value => _value;
    }
    
    public class LocalizationConfig : ScriptableObject
    {
        [SerializeField]
        private Language _language;
        
        [SerializeField]
        private LocalizationKey[] _keys;

        public Language Language
        {
            get => _language;
#if UNITY_EDITOR
            set => _language = value;
#endif
        }

        public LocalizationKey[] Keys
        {
            get => _keys;
#if UNITY_EDITOR
            set => _keys = value;
#endif
        }
    }
}