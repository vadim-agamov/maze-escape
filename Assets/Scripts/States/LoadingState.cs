using System;
using System.Threading;
using Cheats;
using Cysharp.Threading.Tasks;
using Maze;
using Modules.AnalyticsService;
using Modules.CheatService;
using Modules.Extensions;
using Modules.FlyItemsService;
using Modules.FSM;
using Modules.InputService;
using Modules.PlatformService;
using Modules.ServiceLocator;
using Modules.SoundService;
using Modules.UIService;
using Modules.Utils;
using Services.AdsService;
using Services.GamePlayerDataService;
using Services.JumpScreenService;
using UnityEngine;

#if UNITY_EDITOR
    using Modules.PlatformService.EditorPlatformService;
#elif FB
    using Modules.PlatformService.FbPlatformService;
#elif YANDEX
    using Modules.PlatformService.YandexPlatformService;
#elif DUMMY_WEBGL
    using Modules.PlatformService.DummyPlatformService;
#endif

namespace States
{
    public class LoadingState : IState
    {
        async UniTask IState.Enter(CancellationToken cancellationToken)
        {
            SetupEventSystem();

            IJumpScreenService jumpScreenService = GameObject.Find("JumpScreen").GetComponent<JumpScreen>();
            await ServiceLocator.Register(jumpScreenService, cancellationToken: cancellationToken);
            await jumpScreenService.Show(cancellationToken);
            
#if UNITY_EDITOR
            IPlatformService platformService = new GameObject("EditorSN").AddComponent<EditorPlatformService>();
#elif FB
            IPlatformService platformService = new GameObject("FbBridge").AddComponent<FbPlatformService>();
#elif YANDEX
            IPlatformService platformService = new GameObject("Yandex").AddComponent<YandexPlatformService>();
#elif DUMMY_WEBGL
            IPlatformService platformService = new GameObject("DummySN").AddComponent<DummyPlatformService>();
#endif

            var playerDataService = new GameObject().AddComponent<GamePlayerDataService>();
            var soundService = new GameObject().AddComponent<SoundService>()
                .BindProperty(playerDataService.MuteSoundProperty);
            
            var tasks = new[]
            {
                ServiceLocator.Register(platformService, cancellationToken: cancellationToken),
                ServiceLocator.Register(playerDataService, cancellationToken: cancellationToken),
                ServiceLocator.Register<IAnalyticsService>(new AnalyticsService(), cancellationToken: cancellationToken),
                RegisterUI(cancellationToken),
                ServiceLocator.Register<ISoundService>(soundService, cancellationToken: cancellationToken),
                ServiceLocator.Register<IInputService>(new InputService(), cancellationToken: cancellationToken),
                ServiceLocator.Register<IAdsService>(new AdsService(), cancellationToken: cancellationToken),
#if DEV
                RegisterCheats(cancellationToken)
#endif
            };

            await tasks.WhenAll(jumpScreenService);
            
            Debug.Log($"[{nameof(LoadingState)}] all services registered");

            ServiceLocator.Get<GamePlayerDataService>().PlayerData.LastSessionDate = DateTime.Now;
            ServiceLocator.Get<IAnalyticsService>().Start();
            
            await Fsm.Enter(new MazeState(), cancellationToken);
            
            await jumpScreenService.Hide(cancellationToken);
            
            platformService.GameReady();
            ServiceLocator.Get<IAnalyticsService>().TrackEvent($"Loaded");
        }

        private async UniTask RegisterUI(CancellationToken token)
        {
            IUIService uiService = new UIService(new Vector2(1080, 1920));
            await ServiceLocator.Register(uiService, cancellationToken: token);
            await ServiceLocator.Register<IFlyItemsService>(new FlyItemsService(uiService.Canvas), cancellationToken: token);
        }
        
        private static void SetupEventSystem()
        {
            var eventSystemPrefab = Resources.Load("EventSystem");
            var eventSystem = GameObject.Instantiate(eventSystemPrefab);
            eventSystem.name = "[EventSystem]";
            GameObject.DontDestroyOnLoad(eventSystem);
        }

#if DEV
        private async UniTask RegisterCheats(CancellationToken token)
        {
            ICheatService cheatService = new GameObject().AddComponent<CheatService>();
            await ServiceLocator.Register(cheatService, cancellationToken: token);
            var playerDataService = await ServiceLocator.GetAsync<GamePlayerDataService>(token);
            cheatService.RegisterCheatProvider(new GeneralCheatsProvider(cheatService, playerDataService));
            cheatService.RegisterCheatProvider(new AdCheatsProvider(cheatService));
        }  
#endif
        
        UniTask IState.Exit(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}