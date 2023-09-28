using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Modules.UIService.ViewAnimation
{
    [Serializable]
    public class TweenOutCanvasAlpha : IViewAnimation
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

            await _canvasGroup.DOFade(0, duration, cancellationToken: cancellationToken);

            _canvasGroup.interactable = true;
        }
    }
}