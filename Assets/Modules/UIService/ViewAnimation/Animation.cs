using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Modules.UIService.ViewAnimation
{
    [Serializable]
    public sealed class Animation : IViewAnimation
    {
        [SerializeField] 
        private string _trigger;
        
        [SerializeField] 
        private float _duration = 0.5f;
        
        [SerializeField]
        private Animator _animator;
        
        public async UniTask PlayAsync(UIView viewBase, CancellationToken cancellationToken)
        {
            if (_animator == null)
            {
                Debug.LogError($"Animator in view base is null {viewBase.gameObject.name}");
                return;
            }
            
            _animator.SetTrigger(_trigger);
            await UniTask.Delay(TimeSpan.FromSeconds(_duration), cancellationToken: cancellationToken);
        }
    }
}