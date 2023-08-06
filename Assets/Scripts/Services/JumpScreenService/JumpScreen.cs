using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Services.JumpScreenService
{
    public class JumpScreen : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;

        public async UniTask Show(CancellationToken cancellationToken)
        {
            _canvasGroup.alpha = 0;
            await _canvasGroup
                .DOFade(1, 0.5f)
                .SetEase(Ease.InQuad)
                .ToUniTask(cancellationToken: cancellationToken);
        }

        public async UniTask Hide(CancellationToken cancellationToken)
        {
            await _canvasGroup
                .DOFade(0, 0.5f)
                .SetEase(Ease.InQuad)
                .ToUniTask(cancellationToken: cancellationToken);
        }
    }
}
