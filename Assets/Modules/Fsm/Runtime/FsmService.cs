using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;
using UnityEngine;

namespace Modules.FSM
{
    public sealed class FsmService : IFsmService
    {
        private IState _state { get; set; }
        
        UniTask IService.Initialize(IProgress<float> progress, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        async UniTask IFsmService.Enter(IState state, CancellationToken cancellationToken)
        {
            if (_state != null)
            {
                Debug.Log($"[IFsmService] Exit {_state}");
                await _state.Exit(cancellationToken);
                _state.Dispose();
            }

            _state = state;
            Debug.Log($"[IFsmService] Enter {_state}");
            await _state.Enter(cancellationToken);
        }

        public void Dispose()
        {
            _state?.Exit();
            _state?.Dispose();
            _state = null;
        }
        
        IState IFsmService.CurrentState => _state;
    }
}