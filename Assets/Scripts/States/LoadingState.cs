using System;
using System.Collections.Generic;
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
using Modules.LocalizationService;
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
        async UniTask IState.Enter(CancellationToken token)
        {
            SetupEventSystem();

            IJumpScreenService jumpScreenService = GameObject.Find("JumpScreen").GetComponent<JumpScreen>();
            await ServiceLocator.Register(jumpScreenService, token);
            await jumpScreenService.Show(token);
            
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
                ServiceLocator.Register(platformService, token),
                ServiceLocator.Register(playerDataService, token,typeof(IPlatformService)),
                ServiceLocator.Register<IAnalyticsService>(new AnalyticsService(), token,typeof(IPlatformService)),
                ServiceLocator.Register<IUIService>(new UIService(new Vector2(1080, 1920)), token),
                ServiceLocator.Register<IFlyItemsService>(new FlyItemsService(), token, typeof(IUIService)),
                ServiceLocator.Register<ISoundService>(soundService, token),
                ServiceLocator.Register<IInputService>(new InputService(), token),
                ServiceLocator.Register<IAdsService>(new AdsService(), token, typeof(GamePlayerDataService), typeof(IPlatformService), typeof(IAnalyticsService)),
                ServiceLocator.Register<ILocalizationService>(new LocalizationService(), token, typeof(IPlatformService)),
#if DEV
                RegisterCheats(token)
#endif
            };

            await tasks.WhenAll(jumpScreenService);
            
            Debug.Log($"[{nameof(LoadingState)}] all services registered");

            var newInstall = playerDataService.PlayerData.InstallDate == playerDataService.PlayerData.LastSessionDate;
            playerDataService.PlayerData.LastSessionDate = DateTime.Now;
            playerDataService.Commit();
            
            ServiceLocator.Get<IAnalyticsService>().Start();
            
            await Fsm.Enter(new MazeState(), token);
            
            await jumpScreenService.Hide(token);
            
            platformService.GameReady();
            ServiceLocator.Get<IAnalyticsService>().TrackEvent($"Loaded", new Dictionary<string, object>
            {
                { "new_install", newInstall }
            });
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
            await ServiceLocator.Register(cheatService, token, typeof(GamePlayerDataService));
            cheatService.RegisterCheatProvider(new GeneralCheatsProvider(cheatService, ServiceLocator.Get<GamePlayerDataService>()));
            cheatService.RegisterCheatProvider(new AdCheatsProvider(cheatService));
            cheatService.RegisterCheatProvider(new LocalizationCheatsProvider(cheatService));
        }  
#endif
        
        UniTask IState.Exit(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}