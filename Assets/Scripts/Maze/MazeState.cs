using System.Threading;
using Cysharp.Threading.Tasks;
using Maze.Cheats;
using Maze.MazeService;
using Modules.CheatService;
using Modules.ServiceLocator;
using Modules.SoundService;
using Unity.Services.Analytics;
using UnityEngine.SceneManagement;
using IAnalyticsService = Modules.AnalyticsService.IAnalyticsService;
using IState = Modules.FSM.IState;

namespace Maze
{
    public class MazeState : IState
    {
        private readonly string AMBIENT_SOUND_ID = "welcome-to-coconut-island-153182";
        private MazeCheatsProvider _cheatsProvider;
        
        async UniTask IState.Enter(CancellationToken cancellationToken)
        {
            await SceneManager.LoadSceneAsync("Scenes/CoreMaze");
            await ServiceLocator.RegisterAndInitialize<IMazeService>(new MazeService.MazeService(), cancellationToken: cancellationToken);
            ServiceLocator.Get<ISoundService>().PlayLoop(AMBIENT_SOUND_ID);

#if DEV
            var cheatService = ServiceLocator.Get<ICheatService>();
            _cheatsProvider = new MazeCheatsProvider(cheatService);
            cheatService.RegisterCheatProvider(_cheatsProvider);
#endif
        }

        async UniTask IState.Exit(CancellationToken cancellationToken)
        {
            await ServiceLocator.Get<ISoundService>().Stop(AMBIENT_SOUND_ID);
            ServiceLocator.UnRegister<IMazeService>();
            await SceneManager.LoadSceneAsync("Scenes/Empty");
            
#if DEV
            ServiceLocator.Get<ICheatService>().UnRegisterCheatProvider(_cheatsProvider);
#endif
        }
    }
}