using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Modules.UIService.ViewAnimation
{
    internal static class TweenUtils
    {
        public enum UpdateType : byte
        {
            GameTime,
            Realtime
        }

        internal static UniTask DOFade(this CanvasGroup canvasGroup,
            float target,
            float duration,
            UpdateType updateType = UpdateType.GameTime,
            CancellationToken cancellationToken = default)
        {
            return DOFade(() => canvasGroup.alpha, v => canvasGroup.alpha = v, target, duration, updateType,
                cancellationToken);
        }

        private static async UniTask DOFade(Func<float> getter, Action<float> setter, float target,
            float duration,
            UpdateType updateType = UpdateType.GameTime,
            CancellationToken cancellationToken = default)
        {
            var start = getter();

            var t = duration;
            while (t > 0)
            {
                await UniTask.NextFrame(cancellationToken);
                switch (updateType)
                {
                    case UpdateType.GameTime:
                        t -= Time.deltaTime;
                        break;
                    case UpdateType.Realtime:
                        t -= Time.unscaledDeltaTime;
                        break;
                }

                setter(Mathf.Lerp(start, target, 1 - t / duration));
            }

            await UniTask.NextFrame(cancellationToken);
            setter(target);
        }
    }
}