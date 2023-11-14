using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Maze.Configs;
using Maze.Events;
using Maze.Service;
using Modules.Events;
using Modules.UiComponents;
using Modules.Utils;
using UnityEngine;

namespace Maze.Components
{
    public interface IPositionProvider
    {
        Vector3 Position { get; }
    }
    
    public class CharacterController : MonoBehaviour, IComponent, IAttentionFxControl, IPositionProvider
    {
        [SerializeField] 
        private float _speed = 1;
        
        [SerializeField]
        private GameObject _attentionFx;

        [SerializeField]
        private AnimatorAsync _animator;

        private Context _context;
        private readonly ValueChangedTrigger<bool> _isWalkingTrigger = new ValueChangedTrigger<bool>();
        private CancellationTokenSource _disposalTokenSource;
        private bool _paused;
        
        Vector3 IPositionProvider.Position => transform.position;
        
        UniTask IComponent.Initialize(Context context, IMazeService mazeService)
        {
            _context = context;
            _context.CharacterPositionProvider = this;
            // Event<PathUpdatedEvent>.Subscribe(OnPathUpdated);
            SetupStartPosition();
            _disposalTokenSource = new CancellationTokenSource();
            return UniTask.CompletedTask;
        }
        
        private void SetupStartPosition()
        {
            foreach (var cell in _context.CellViews)
            {
                if (cell.CellType.HasFlag(CellType.Start))
                {
                    transform.position = cell.transform.position;
                }
            }
        }
        
        void IComponent.Update()
        {
            if (_paused)
            {
                return;
            }
            
            if (_context.Path.Count == 0)
            {
                if (_isWalkingTrigger.SetValue(false))
                {
                    _animator.SetTrigger("Stop");
                    Event<EndWalkEvent>.Publish();
                }
            }

            else
            {
                _context.CurrentCell = _context.Path.First.Value;

                if (Vector3.Distance(transform.position, _context.Path.First.Value.transform.position) < 0.01f)
                {
                    _context.Path.RemoveFirst();
                }
                else
                { 
                    transform.position = Vector3.MoveTowards(transform.position, _context.Path.First.Value.transform.position, _speed * Time.deltaTime);

                    if (_isWalkingTrigger.SetValue(true))
                    {
                        _animator.SetTrigger("Walk");
                        Event<BeginWalkEvent>.Publish();
                    }
                }
            }
        }
        
        void IDisposable.Dispose()
        {
            _disposalTokenSource?.Cancel();
            _disposalTokenSource = null;
        }

        void IAttentionFxControl.Show() => _attentionFx.SetActive(true);

        void IAttentionFxControl.Hide() => _attentionFx.SetActive(false);

        public async UniTask Teleport(CancellationToken token)
        {
            if (_context.Path.Count < 10)
            {
                await UniTask.WaitUntil(() => _context.Path.Count == 0, cancellationToken: token);
                return;
            }
            
            // Do teleport
            await _animator
                .SetTrigger("Dissapear", "CrabDissapear", _disposalTokenSource.Token)
                .SuppressCancellationThrow();

            _paused = true;
            
            // Remove elements until only the last two remain
            while (_context.Path.Count > 4)
            {
                _context.Path.RemoveFirst();
            }

            transform.position = _context.Path.First.Value.transform.position;
            
            await _animator
                .SetTrigger("Appear", "CrabAppear", _disposalTokenSource.Token)
                .SuppressCancellationThrow();
            
            _paused = false;
            
            await UniTask.WaitUntil(() => _context.Path.Count == 0, cancellationToken: token);
        }
    }
}