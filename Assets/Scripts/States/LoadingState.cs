using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.FlyItemsService;
using Modules.FSM;
using Modules.InputService;
using Modules.ServiceLocator;
using Modules.SoundService;
using Modules.UIService;
using Services.PlayerDataService;
using SN;
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

            var eventSystemPrefab = Resources.Load("EventSystem");
            var eventSystem = GameObject.Instantiate(eventSystemPrefab);
            GameObject.DontDestroyOnLoad(eventSystem);
            
            var loading = GameObject.Find("LoadingPanel").GetComponent<LoadingPanel>();
            
            var tasks = new []
            {
                SnBridge.Initialize(),
                RegisterUI(),
                ServiceLocator.RegisterAndInitialize<ISoundService>(new GameObject().AddComponent<SoundService>(), cancellationToken: cancellationToken),
                ServiceLocator.RegisterAndInitialize<IPlayerDataService>(new PlayerDataService(), cancellationToken: cancellationToken),
                ServiceLocator.RegisterAndInitialize<IInputService>(new InputService(), cancellationToken: cancellationToken),
            };

            await tasks.WhenAll(new Progress<float>(p =>
            {
                Debug.Log($"[{nameof(LoadingState)}] progress {p}");
                loading.Progress = p;
            }));
            
            await loading.Hide();
            await _playModel.OpenAndShow("PlayPanel", cancellationToken);
            
            async UniTask RegisterUI()
            {
                IUIService uiService = new UIService();
                await ServiceLocator.RegisterAndInitialize(uiService, cancellationToken: cancellationToken);
                await ServiceLocator.RegisterAndInitialize<IFlyItemsService>(new FlyItemsService(uiService.RootCanvas), cancellationToken: cancellationToken);
            }
        }

        void IDisposable.Dispose()
        {
        }

        async UniTask IState.Exit(CancellationToken cancellationToken)
        {
            await _playModel.HideAndClose(cancellationToken);
        }
    }
}