using System;
using System.Collections.Generic;
using System.Linq;
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
    public class CharacterComponent : MonoBehaviour, IComponent, IAttentionFxControl
    {
        [SerializeField] 
        private float _speed = 1;
        
        [SerializeField]
        private GameObject _attentionFx;
        
        private readonly LinkedList<Vector3> _waypoints = new LinkedList<Vector3>();
        private Context _context;
        private bool _initialized;
        private readonly ValueChangedTrigger<bool> _isWalkingTrigger = new ValueChangedTrigger<bool>();

        [SerializeField]
        private AnimatorAsync _animator;

        private CancellationTokenSource _disposalTokenSource;
        
        UniTask IComponent.Initialize(Context context, IMazeService mazeService)
        {
            _context = context;
            Event<PathUpdatedEvent>.Subscribe(OnPathUpdated);
            SetupStartPosition();
            _disposalTokenSource = new CancellationTokenSource();
            _initialized = true;
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

        private void OnPathUpdated(PathUpdatedEvent @event)
        {
            var cells = @event.Cells.Select(x => x.transform.position);

            if (!_context.Active)
            {
                return;
            }

            foreach (var cell in cells)
            {
                while (_waypoints.Any(p => p == cell))
                {
                    _waypoints.RemoveLast();
                }
                
                _waypoints.AddLast(cell);
            }
        }

        private void Update()
        {
            if (!_initialized || _waypoints.Count == 0)
            {
                return;
            }

            if (Vector3.Distance(transform.position, _waypoints.First.Value) < 0.01f)
            {
                _waypoints.RemoveFirst();
            }

            DoStep();
        }

        private void DoStep()
        {
            if (_waypoints.Count == 0)
            {
                if (_isWalkingTrigger.SetValue(false))
                {
                    _animator.SetTrigger("Stop");
                    Event<EndWalkEvent>.Publish();
                }

                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, _waypoints.First.Value, _speed * Time.deltaTime);

            if (_isWalkingTrigger.SetValue(true))
            {
                _animator.SetTrigger("Walk");
                Event<BeginWalkEvent>.Publish();
            }
        }

        public async UniTask WaitCharacter(CancellationToken token)
        {
            if (_waypoints.Count < 10)
            {
                await UniTask.WaitUntil(() => _waypoints.Count <= 1, cancellationToken: token);
                return;
            }

            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);
            
            // Do teleport
            await _animator.SetTrigger("Dissapear", "CrabDissapear", _disposalTokenSource.Token)
                .SuppressCancellationThrow();

            transform.position = _waypoints.Last.Previous.Value;
            _waypoints.Clear();

            Debug.Log($">>> appear");
            await _animator.SetTrigger("Appear", "CrabAppear", _disposalTokenSource.Token)
                .SuppressCancellationThrow();
        }

        void IDisposable.Dispose()
        {
            _disposalTokenSource?.Cancel();
            _disposalTokenSource = null;
            _initialized = false;
            Event<PathUpdatedEvent>.Unsubscribe(OnPathUpdated);
        }

        void IAttentionFxControl.Show() => _attentionFx.SetActive(true);
        void IAttentionFxControl.Hide() => _attentionFx.SetActive(false);
        
        private void OnDrawGizmos()
        {
            foreach (var waypoint in _waypoints)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(waypoint, 0.1f);
            }

            if (_waypoints.Count > 0)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(_waypoints.First.Value, 0.2f);
            }
        }
    }
}