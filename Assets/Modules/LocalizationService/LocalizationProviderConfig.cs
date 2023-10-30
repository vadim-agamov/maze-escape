using System.Collections.Generic;
using System.Linq;
using Modules.PlatformService;
using UnityEngine;

namespace Modules.LocalizationService
{
    public class LocalizationProviderConfig : ScriptableObject
    {
        [SerializeField] 
        private List<LocalizationConfig> _configs = new List<LocalizationConfig>();
        
        // public string Localize(Language language, string key, params object[] args)
        // {
        //     var config = _configs.First(config => config.Language == language);
        //     var localizationKey = config.Keys.First(localizationKey => localizationKey.Key == key);
        //     return string.Format(localizationKey.Value, args);
        // }
        
        public IReadOnlyList<LocalizationKey> GetKeys(Language language)
        {
            var config = _configs.First(config => config.Language == language);
            return config.Keys;
        }
        
#if UNITY_EDITOR
        public void Add(LocalizationConfig config)
        {
            _configs.Add(config);
        }
#endif
    }
}