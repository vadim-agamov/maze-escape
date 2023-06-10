using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Modules.UIService
{
    public class UIService: IUIService
    {
        private Canvas _rootCanvas;
        private GraphicRaycaster _rootRaycaster;
        private GameObject _uiRoot;
        private Camera _camera;
        private bool _prevInteractableState;
        Canvas IUIService.RootCanvas => _rootCanvas;
        Camera IUIService.Camera => _camera;
        
        async UniTask IService.Initialize(IProgress<float> progress, CancellationToken cancellationToken)
        {
            _uiRoot = await Addressables.InstantiateAsync("UIRoot");
            _uiRoot.name = "[IURoot]";
            _rootCanvas = _uiRoot.GetComponentInChildren<Canvas>();
            _rootRaycaster = _uiRoot.GetComponentInChildren<GraphicRaycaster>();
            _camera = _uiRoot.GetComponentInChildren<Camera>();

#if DEV
            _prevInteractableState = _rootRaycaster.enabled;
#endif

            Object.DontDestroyOnLoad(_uiRoot);
            
            progress?.Report(1f);
        }

        void IService.Dispose()
        {
            Addressables.ReleaseInstance(_uiRoot);
        }

        async UniTask IUIService.Open<TModel>(TModel model, string key, CancellationToken cancellationToken)
        {
            var op = Addressables.InstantiateAsync(key, _rootCanvas.transform);
            await op.ToUniTask(cancellationToken: cancellationToken);
            if (op.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError($"Unable to instantiate asset from location {key}:\r\n{op.OperationException}");
                return;
            }

            var result = op.Result;
            result.name = key;
            var view = result.GetComponent<UIViewBase>();
            model.AttachView(view);
            view.gameObject.SetActive(false);
        }

        void IUIService.Close<TModel>(TModel model)
        {
            Addressables.ReleaseInstance(model.View.gameObject);
            model.DeattachView();
        }

        bool IUIService.SetInteractiveState(bool state)
        {
#if DEV
            if (_prevInteractableState == state)
                Debug.Log($"[UIService]::Service already {(state ? "Activated" : "Deactivate")}");
#endif

            var prevInteractableState = _prevInteractableState;
            _prevInteractableState = state;
            _rootRaycaster.enabled = state;
            return prevInteractableState;
        }
    }
}