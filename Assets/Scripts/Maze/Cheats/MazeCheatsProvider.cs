using Actions;
using Cysharp.Threading.Tasks;
using Maze.Service;
using Modules.CheatService;
using Modules.CheatService.Controls;
using Modules.ServiceLocator;
using Services.GamePlayerDataService;
using UnityEngine;

namespace Maze.Cheats
{
    public class MazeCheatsProvider: ICheatsProvider
    {
        private readonly CheatButton _restart;
        private readonly CheatIntInput _level;
        private readonly CheatLabel _levelInfo;
        private readonly CheatButton _winLevel;

        public MazeCheatsProvider(ICheatService cheatService, IMazeService mazeService)
        {
            _restart = new CheatButton(
                cheatService,
                "Restart Level",
                () =>
                {
                    new GotoStateAction(new MazeState(), true).Execute(Bootstrapper.SessionToken).Forget();
                });

            var levelConfig = mazeService.Context.Level;
            _levelInfo = new CheatLabel(() => $"Level Info: #{levelConfig.LevelId}, Complexity :{levelConfig.Complexity}");
            
            var playerDataService = ServiceLocator.Get<GamePlayerDataService>();
            _level = new CheatIntInput(
                cheatService,
                "Level",
                () => playerDataService.PlayerData.Level,
                level =>
                {
                    Debug.Log("Set level to " + level);
                    playerDataService.PlayerData.Level = level;
                    new GotoStateAction(new MazeState(), true).Execute(Bootstrapper.SessionToken).Forget();
                });
            
            _winLevel = new CheatButton(cheatService,
                "Win", 
                () => new WinLevelAction().Execute(Bootstrapper.SessionToken).Forget());
        }

        void ICheatsProvider.OnGUI()
        {
            _levelInfo.OnGUI();
            _restart.OnGUI();
            _winLevel.OnGUI();
            _level.OnGUI();
        }

        string ICheatsProvider.Id => "Maze";
    }
}