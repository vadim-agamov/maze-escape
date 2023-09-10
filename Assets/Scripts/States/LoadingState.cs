using System;
using System.Threading;
using Cheats;
using Cysharp.Threading.Tasks;
using Modules.AnalyticsService;
using Modules.CheatService;
using Modules.Extensions;
using Modules.FlyItemsService;
using Modules.FSM;
using Modules.InputService;
using Modules.ServiceLocator;
using Modules.SocialNetworkService;
using Modules.SocialNetworkService.EditorSocialNetworkService;
using Modules.SocialNetworkService.FbSocialNetworkService;
using Modules.SoundService;
using Modules.UIService;
using Services.JumpScreenService;
using Services.PlayerDataService;
using UI;
using UnityEngine;

namespace States
{
    public class LoadingState : IState
    {
        private PlayModel _playModel;

        async UniTask IState.Enter(CancellationToken cancellationToken)
        {
            Application.targetFrameRate = 60;
            Time.fixedDeltaTime = 1f / Application.targetFrameRate;
            Debug.Log($"[{nameof(LoadingState)}] {Application.targetFrameRate}, {Time.fixedDeltaTime}");
            
            _playModel = new PlayModel();

            SetupEventSystem();

            var loading = GameObject.Find("LoadingPanel").GetComponent<LoadingPanel>();
            
#if UNITY_EDITOR
            var socialNetworkService = new GameObject("EditorSN").AddComponent<EditorSocialNetworkService>();
#elif FB
            var socialNetworkService = new GameObject("FbBridge").AddComponent<FbSocialNetworkService>();
#endif
            
            var tasks = new []
            {
                ServiceLocator.Register<ISocialNetworkService>(socialNetworkService, cancellationToken: cancellationToken),
                ServiceLocator.Register<IPlayerDataService>(new PlayerDataService(), cancellationToken: cancellationToken),
                ServiceLocator.Register<IAnalyticsService>(new AnalyticsService(), cancellationToken: cancellationToken),
                RegisterUI(cancellationToken),
                ServiceLocator.Register<ISoundService>(new GameObject().AddComponent<SoundService>(), cancellationToken: cancellationToken),
                ServiceLocator.Register<IInputService>(new InputService(), cancellationToken: cancellationToken),
                ServiceLocator.Register<IJumpScreenService>(new JumpScreenService(), cancellationToken: cancellationToken),
#if DEV
                RegisterCheats(cancellationToken)
#endif
            };
            
            await tasks.WhenAll(new Progress<float>(p =>
            {
                Debug.Log($"[{nameof(LoadingState)}] progress {p}");
                loading.Progress = p;
            }));
            
            Debug.Log($"all services registered");
            
            ServiceLocator.Get<IPlayerDataService>().Data.LastSessionDate = DateTime.Now;
            ServiceLocator.Get<IAnalyticsService>().Start();
            
            await loading.Hide();
            await _playModel.OpenAndShow("PlayPanel", cancellationToken);
        }

        private async UniTask RegisterUI(CancellationToken token)
        {
            IUIService uiService = new UIService();
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
        
        async UniTask IState.Exit(CancellationToken cancellationToken)
        {
            await _playModel.HideAndClose(cancellationToken);
        }
    }
}