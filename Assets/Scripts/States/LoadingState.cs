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
using Modules.ServiceLocator;
using Modules.SocialNetworkService;
using Modules.SoundService;
using Modules.UIService;
using Services.JumpScreenService;
using Services.PlayerDataService;
using UnityEngine;

#if UNITY_EDITOR
    using Modules.SocialNetworkService.EditorSocialNetworkService;
#elif FB
    using Modules.SocialNetworkService.FbSocialNetworkService;
#elif DUMMY_WEBGL
    using Modules.SocialNetworkService.DummySocialNetworkService;
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
            var socialNetworkService = new GameObject("EditorSN").AddComponent<EditorSocialNetworkService>();
#elif FB
            var socialNetworkService = new GameObject("FbBridge").AddComponent<FbSocialNetworkService>();
#elif DUMMY_WEBGL
            var socialNetworkService = new GameObject("DummySN").AddComponent<DummySocialNetworkService>();
#endif

            var tasks = new[]
            {
                ServiceLocator.Register<ISocialNetworkService>(socialNetworkService, cancellationToken: cancellationToken),
                ServiceLocator.Register<IPlayerDataService>(new PlayerDataService(), cancellationToken: cancellationToken),
                ServiceLocator.Register<IAnalyticsService>(new AnalyticsService(), cancellationToken: cancellationToken),
                RegisterUI(cancellationToken),
                ServiceLocator.Register<ISoundService>(new GameObject().AddComponent<SoundService>(), cancellationToken: cancellationToken),
                ServiceLocator.Register<IInputService>(new InputService(), cancellationToken: cancellationToken),
#if DEV
                RegisterCheats(cancellationToken)
#endif
            };

            await tasks.WhenAll(new Progress<float>(p =>
            {
                Debug.Log($"[{nameof(LoadingState)}] progress {p}");
                jumpScreenService.Progress = p;
            }));
            
            Debug.Log($"[{nameof(LoadingState)}] all services registered");

            ServiceLocator.Get<IPlayerDataService>().Data.LastSessionDate = DateTime.Now;
            ServiceLocator.Get<IAnalyticsService>().Start();
            ServiceLocator.Get<IAnalyticsService>().TrackEvent($"Loaded");

            await Fsm.Enter(new MazeState(), cancellationToken);
            
            await jumpScreenService.Hide(cancellationToken);
        }

        private async UniTask RegisterUI(CancellationToken token)
        {
            IUIService uiService = new UIService(new Vector2(1080, 1920));
            await ServiceLocator.Register(uiService, cancellationToken: token);
            await ServiceLocator.Register<IFlyItemsService>(new FlyItemsService(uiService.RootCanvas), cancellationToken: token);
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
            var playerDataService = await ServiceLocator.GetAsync<IPlayerDataService>(token);
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