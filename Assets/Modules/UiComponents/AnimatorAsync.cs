using System.Threading;
using Cysharp.Threading.Tasks;
using Maze.Components;
using UnityEngine;

namespace Modules.UiComponents
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorAsync: MonoBehaviour
    {
        [HideInInspector]
        [SerializeField]
        private Animator _animator;
        
        private UniTaskCompletionSource _taskCompletionSource;
        private string _clipId;

        public void Awake()
        {
            foreach (var clip in _animator.runtimeAnimatorController.animationClips)
            {
                clip.events = new AnimationEvent[]
                {
                    new AnimationEvent
                    {
                        functionName = nameof(OnAnimationEvent),
                        time = clip.length,
                        stringParameter = clip.name
                    }
                };
            }
        }

        public void SetTrigger(string trigger)
        {
            _clipId = string.Empty;
            _taskCompletionSource?.TrySetCanceled();
            _taskCompletionSource = null;
            _animator.SetTrigger(trigger);
        }

        public async UniTask SetTrigger(string trigger, string waitClipEnd, CancellationToken token = default)
        {
            _taskCompletionSource?.TrySetCanceled();
            _taskCompletionSource = new UniTaskCompletionSource();
            _clipId = waitClipEnd;
            _animator.SetTrigger(trigger);
            await _taskCompletionSource.Task.AttachExternalCancellation(token);
            await UniTask.Yield();
        }

        public void OnAnimationEvent(string clipName)
        {
            if (_clipId != clipName || _taskCompletionSource == null)
            {
                return;
            }

            _clipId = string.Empty;
            _taskCompletionSource?.TrySetResult();
            _taskCompletionSource = null;
        }
        
        private void OnValidate()
        {
            _animator ??= GetComponent<Animator>();
        }
    }
}