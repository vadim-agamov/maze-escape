#if PLATFORM_ANDROID
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Belka.Rnd.Plugins.LoggerUtils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Rnd.Modules.UIService
{
    public class NavigationsService : INavigationService
    {
        private readonly LinkedList<UIModel> _list = new();
        private UniTask _goBackAction;
        private bool _interactableState;

#if DEV
        private bool _prevInteractableState;
#endif
        
        public UniTask Initialize(IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            _interactableState = true;
#if DEV
            _prevInteractableState = _interactableState;
#endif
            UpdateLoop(cancellationToken).Forget();
            
            progress?.Report(1f);
            return UniTask.CompletedTask;
        }

        private async UniTask UpdateLoop(CancellationToken cancellationToken)
        {
            while (true)
            {
                Update();
                try
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                }
                catch (OperationCanceledException _)
                {
                    break;
                }
            }
        }

        void INavigationService.OnShown(UIModel model)
        {
            _list.AddFirst(model);
        }

        void INavigationService.OnHidden(UIModel model)
        {
            _list.Remove(model);
        }

        void INavigationService.SetInteractiveState(bool state)
        {
#if DEV
            if (_prevInteractableState == state)
                Log.Info($"[NavigationService]::Service already {(state ? "Activated" : "Deactivate")}");
            _prevInteractableState = state;
#endif
            
            _interactableState = state;
        }
        
        private void Update()
        {
            if (!_interactableState)
                return;
            
            if (Input.GetKeyDown(KeyCode.Escape) &&
                _goBackAction.Status != UniTaskStatus.Pending)
            {
                _goBackAction = ProcessGoBack();
            }
        }

        private UniTask ProcessGoBack(CancellationToken cancellationToken = default)
        {
            var model = _list.FirstOrDefault(model => model.GoBackAction != null);
            return model == null ? UniTask.CompletedTask : model.GoBackAction.Invoke(cancellationToken);
        }

        public void Dispose()
        {
        }
    }
}
#endif