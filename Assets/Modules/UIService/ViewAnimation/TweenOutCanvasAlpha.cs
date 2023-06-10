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

        public async UniTask PlayAsync(UIViewBase viewBase, CancellationToken cancellationToken = default)
        {
            if (viewBase.CanvasGroup == null)
            {
                Debug.LogError($"Canvas group in view base is null {viewBase.gameObject.name}");
                return;
            }

            viewBase.CanvasGroup.interactable = false;

            await viewBase.CanvasGroup.DOFade(0, duration, cancellationToken: cancellationToken);

            viewBase.CanvasGroup.interactable = true;
        }
    }
}