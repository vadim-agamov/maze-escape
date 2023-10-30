using System;
using TMPro;
using UnityEngine;

namespace Modules.LocalizationService
{
    public class Localization: MonoBehaviour
    {
        [HideInInspector]
        [SerializeField]
        private TMP_Text _text;

        [SerializeField] 
        private string _key;
        
        private ILocalizationService LocalizationService { get; set; }
        private object[] _args = Array.Empty<object>();

        private void OnEnable()
        {
            LocalizationService = ServiceLocator.ServiceLocator.Get<ILocalizationService>();
            Localize();
        }

        private void Localize()
        {
            _text.text = LocalizationService.Localize(_key, _args);
        }
        
        public void SetParameters(params object[] args)
        {
            _args = args;
        }
        
        private void OnValidate()
        {
            _text ??= GetComponent<TMP_Text>();
        }
    }
}