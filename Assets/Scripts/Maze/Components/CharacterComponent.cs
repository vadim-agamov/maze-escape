using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Maze.Configs;
using Maze.Service;
using Modules.Events;
using UnityEngine;

namespace Maze.Components
{
    public struct CharacterMoveBeginEvent
    {
    }
    
    public struct CharacterMoveEndEvent
    {
    }
    
    
    public class CharacterComponent : MonoBehaviour, IComponent, IAttentionFxControl
    {
        [SerializeField] 
        private float _speed = 1;
        
        [SerializeField]
        private GameObject _attentionFx;

        private LinkedList<Vector3> _waypoints;
        private Vector3 _currentWaypoint;
        private Context _context;
        private bool _initialized;
        private const int IdleState = 0;
        private const int WalkState = 1;
        private const int JumpState = 2;

        private int _currentState = IdleState;

        private static readonly int State = Animator.StringToHash("state");

        public bool IsWalking { get; private set; }

        UniTask IComponent.Initialize(Context context, IMazeService mazeService)
        {
            _context = context;
            _waypoints = new LinkedList<Vector3>();
            _currentWaypoint = Vector3.zero;
            Event<PathUpdatedEvent>.Subscribe(OnPathUpdated);
            SetupStartPosition();
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
        
        private void OnPathUpdated(PathUpdatedEvent evt)
        {
            if(!_context.Active)
                return;
            
            var point = evt.Cells.Last().transform.position;

            if (_waypoints.Count(w => w == point) > 0) 
            {
                while(true)
                {
                    var waypoint = _waypoints.Last.Value;
            
                    if(waypoint == point)
                        break;
                    
                    _waypoints.RemoveLast();
                }
            }
            else
            {
                _waypoints.AddLast(point);
            }
        }

        // private void SetState(int state)
        // {
        //     if (_currentState == state)
        //     {
        //         return;
        //     }
        //
        //     _currentState = state;
        //     _animator.SetInteger(State, _currentState);
        // }

        private void Update()
        {
            if (!_initialized)
            {
                return;
            }

            if (_currentWaypoint == Vector3.zero && _waypoints.Count > 0)
            {
                _currentWaypoint = _waypoints.Last.Value;
                _waypoints.RemoveLast();
            }

            if (_currentWaypoint == Vector3.zero)
            {
                // SetState(_context.Active ? IdleState : JumpState);
                IsWalking = false;
                return;
            }

            transform.position  = Vector3.MoveTowards(transform.position, _currentWaypoint, _speed * Time.deltaTime);
            IsWalking = true;
            // SetState(WalkState);

            if (Vector3.Distance(transform.position, _currentWaypoint) < 0.1f)
            {
                if (_waypoints.Count > 0)
                {
                    _currentWaypoint = _waypoints.First.Value;
                    _waypoints.RemoveFirst();
                }
                else
                {
                    _currentWaypoint = Vector3.zero;
                }
            }
        }
        
        void IDisposable.Dispose()
        {
            _initialized = false;
            Event<PathUpdatedEvent>.Unsubscribe(OnPathUpdated);
        }

        void IAttentionFxControl.Show() => _attentionFx.SetActive(true);
        void IAttentionFxControl.Hide() => _attentionFx.SetActive(false);
    }
}