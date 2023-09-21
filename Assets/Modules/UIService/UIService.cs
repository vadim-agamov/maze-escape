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
        private GameObject _uiRoot;
        private Vector2 ReferenceResolution { get; }
        Canvas IUIService.RootCanvas => _rootCanvas;

        public UIService(Vector2 referenceResolution)
        {
            ReferenceResolution = referenceResolution;
        }

        UniTask IService.Initialize(CancellationToken _)
        {
            Debug.Log($"[{nameof(UIService)}] Initialize begin");

            _uiRoot = new GameObject("[UIRoot]", typeof(Canvas), typeof(GraphicRaycaster), typeof(CanvasScaler));
            _rootCanvas = _uiRoot.GetComponentInChildren<Canvas>();
            _rootCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            var canvasScaler = _uiRoot.GetComponent<CanvasScaler>();
            canvasScaler.referenceResolution = ReferenceResolution;
            canvasScaler.matchWidthOrHeight = 1;
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

            Object.DontDestroyOnLoad(_uiRoot);
            Debug.Log($"[{nameof(UIService)}] Initialize end");
            return UniTask.CompletedTask;
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

            var viewGameObject = op.Result;
            viewGameObject.name = key;
            viewGameObject.SetActive(false);
            var view = viewGameObject.GetComponent<UIViewBase>();
            model.AttachView(view);
        }

        void IUIService.Close<TModel>(TModel model)
        {
            Addressables.ReleaseInstance(model.View.gameObject);
            model.DeattachView();
        }
    }
}