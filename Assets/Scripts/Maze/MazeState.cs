using System.Threading;
using Cysharp.Threading.Tasks;
using Maze.Cheats;
using Maze.Service;
using Maze.UI;
using Modules.CheatService;
using Modules.LocalizationService;
using Modules.ServiceLocator;
using Modules.SoundService;
using Modules.UIService;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using IState = Modules.FSM.IState;

namespace Maze
{
    public class MazeState : IState
    {
        private ISoundService SoundService { get; } = ServiceLocator.Get<ISoundService>();
        private const string AMBIENT_SOUND_ID = "ambient";
        private MazeCheatsProvider _cheatsProvider;
        private MazeHUDModel _hud;
        private LocalizationProviderConfig _localizationProvider;

        async UniTask IState.Enter(CancellationToken cancellationToken)
        {
            await SceneManager.LoadSceneAsync("Scenes/CoreMaze");
            await ServiceLocator.Register<IMazeService>(new GameObject("MazeService").AddComponent<MazeService>(), cancellationToken);
            SoundService.PlayLoop(AMBIENT_SOUND_ID);
            _hud = new MazeHUDModel();
            await _hud.OpenAndShow(MazeHUD.KEY, cancellationToken);
            _localizationProvider = await Addressables.LoadAssetAsync<LocalizationProviderConfig>("MazeLocalizationProvider").ToUniTask(cancellationToken: cancellationToken);
            ServiceLocator.Get<ILocalizationService>().Register(_localizationProvider);
#if DEV
            var cheatService = ServiceLocator.Get<ICheatService>();
            _cheatsProvider = new MazeCheatsProvider(cheatService, ServiceLocator.Get<IMazeService>());
            cheatService.RegisterCheatProvider(_cheatsProvider);
#endif
        }

        async UniTask IState.Exit(CancellationToken cancellationToken)
        {
            await SoundService.Stop(AMBIENT_SOUND_ID);
            ServiceLocator.UnRegister<IMazeService>();
            await SceneManager.LoadSceneAsync("Scenes/Empty");
            await _hud.HideAndClose(cancellationToken);
            ServiceLocator.Get<ILocalizationService>().Unregister(_localizationProvider);
#if DEV
            ServiceLocator.Get<ICheatService>().UnRegisterCheatProvider(_cheatsProvider);
#endif
        }
    }
}