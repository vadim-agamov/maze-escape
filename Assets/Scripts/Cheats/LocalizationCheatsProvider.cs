using Modules.CheatService;
using Modules.CheatService.Controls;
using Modules.LocalizationService;
using Modules.PlatformService;
using Modules.ServiceLocator;

namespace Cheats
{
    public class LocalizationCheatsProvider: ICheatsProvider
    {
        private readonly CheatButton _langRussian;
        private readonly CheatButton _langEnglish;
        private readonly CheatButton _langTurkish;
        private readonly CheatLabel _currentLang;
        private ILocalizationService LocalizationService { get; } = ServiceLocator.Get<ILocalizationService>();

        public LocalizationCheatsProvider(ICheatService cheatService)
        {
            _langRussian = new CheatButton(cheatService, Language.Russian.ToString(), () =>
            {
                LocalizationService.SetLanguage(Language.Russian);
            });
            _langEnglish = new CheatButton(cheatService, Language.English.ToString(), () =>
            {
                LocalizationService.SetLanguage(Language.English);
            });
            _langTurkish = new CheatButton(cheatService, Language.Turkish.ToString(), () =>
            {
                LocalizationService.SetLanguage(Language.Turkish);
            });
            
            _currentLang = new CheatLabel(()=> $"Current language: {LocalizationService.CurrentLanguage}");
        }

        public void OnGUI()
        {
            _langRussian.OnGUI();
            _langEnglish.OnGUI();
            _langTurkish.OnGUI();
            _currentLang.OnGUI();
        }

        public string Id => "Localization";
    }
}