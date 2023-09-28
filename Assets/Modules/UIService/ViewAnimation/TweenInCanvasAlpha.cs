using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Modules.UIService.ViewAnimation
{
    [Serializable]
    public class TweenInCanvasAlpha : IViewAnimation
    {
        [SerializeField]
        private float duration = .3f;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        public async UniTask PlayAsync(UIView viewBase, CancellationToken cancellationToken = default)
        {
            if (_canvasGroup == null)
            {
                Debug.LogError($"Canvas group in view base is null {viewBase.gameObject.name}");
                return;
            }

            _canvasGroup.interactable = false;
            _canvasGroup.alpha = 0;
            
            await _canvasGroup.DOFade(1, duration, cancellationToken: cancellationToken);

            _canvasGroup.interactable = true;
        }
    }
}