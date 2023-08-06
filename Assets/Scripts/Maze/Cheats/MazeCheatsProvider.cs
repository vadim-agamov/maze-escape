using Actions;
using Cysharp.Threading.Tasks;
using Maze.MazeService;
using Modules.CheatService;
using Modules.CheatService.Controls;
using Modules.ServiceLocator;
using Services.PlayerDataService;
using UnityEngine;

namespace Maze.Cheats
{
    public class MazeCheatsProvider: ICheatsProvider
    {
        private readonly CheatButton _restart;
        private readonly CheatIntInput _level;
        private readonly CheatLabel _levelInfo;
        private readonly CheatButton _winLevel;

        public MazeCheatsProvider(ICheatService cheatService)
        {
            _restart = new CheatButton(
                cheatService,
                "Restart Level",
                () =>
                {
                    new GotoStateAction(new MazeState(), true).Execute(Bootstrapper.SessionToken).Forget();
                });

            var levelConfig = ServiceLocator.Get<IMazeService>().Context.Level;
            _levelInfo = new CheatLabel(() => $"Level Info: #{levelConfig.LevelId}, min path:{levelConfig.MinPath}");
            
            var playerDataService = ServiceLocator.Get<IPlayerDataService>();
            _level = new CheatIntInput(
                cheatService,
                "Level",
                () => playerDataService.Data.Level,
                level =>
                {
                    Debug.Log("Set level to " + level);
                    playerDataService.Data.Level = level;
                    playerDataService.Commit();
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