using Modules.CheatService;
using Modules.CheatService.Controls;
using Modules.ServiceLocator;
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

        public GeneralCheatsProvider(ICheatService cheatService)
        {
            _playerDataService = ServiceLocator.Get<IPlayerDataService>();

            _reset = new CheatButton(
                cheatService,
                "Reset", () =>
            {
                Debug.Log("Reset");
                _playerDataService.Reset();
            });
            
            _installDate = new CheatLabel(() => $"Install date: {_playerDataService.Data?.InstallDate}");
            _lastSessionDate = new CheatLabel(() => $"Last Session date: {_playerDataService.Data?.LastSessionDate}");
        }

        void ICheatsProvider.OnGUI()
        {
            _installDate.OnGUI();
            _lastSessionDate.OnGUI();
            _reset.OnGUI();
        }

        string ICheatsProvider.Id => "General";
    }
}