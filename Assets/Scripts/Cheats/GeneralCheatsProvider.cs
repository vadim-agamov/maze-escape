using Modules.CheatService;
using Modules.CheatService.Controls;
using Services.PlayerDataService;
using UnityEngine;

namespace Cheats
{
    public class GeneralCheatsProvider : ICheatsProvider
    {
        private string _id;
        private readonly IPlayerDataService _playerDataService;
        private readonly CheatButton _reset;
        private readonly CheatLabel _installDate;
        private readonly CheatLabel _lastSessionDate;
        private readonly CheatLabel _adsLastShownDate;

        public GeneralCheatsProvider(ICheatService cheatService, IPlayerDataService playerDataService)
        {
            _playerDataService = playerDataService;

            _reset = new CheatButton(cheatService, "Reset", () =>
            {
                Debug.Log("Reset");
                _playerDataService.Reset();
            });
            
            _installDate = new CheatLabel(() => $"Install date: {_playerDataService.Data?.InstallDate}");
            _lastSessionDate = new CheatLabel(() => $"Last Session date: {_playerDataService.Data?.LastSessionDate}");
            _adsLastShownDate = new CheatLabel(() => $"Ads Shown date: {_playerDataService.Data?.AdsLastShownDate}");
        }

        void ICheatsProvider.OnGUI()
        {
            _installDate.OnGUI();
            _lastSessionDate.OnGUI();
            _adsLastShownDate.OnGUI();
            _reset.OnGUI();
        }

        string ICheatsProvider.Id => "General";
    }
}