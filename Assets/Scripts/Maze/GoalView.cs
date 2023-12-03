using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Modules.UiComponents;
using UnityEngine;
using UnityEngine.Pool;

namespace Maze
{
    public class GoalView: MonoBehaviour, IDisposable
    {
        [SerializeField]
        private AnimatorAsync _animator;
        
        public ObjectPool<GoalView> Pool { private get; set; }

        public IEnumerable<string> Rewards { get; set; }
        
        public UniTask PlayAppear() => _animator.SetTrigger("Appear", "GoalAppear");

        public UniTask PlayCollected() => _animator.SetTrigger("Collect", "GoalCollected");

        public void Dispose() => Pool.Release(this);
    }
}