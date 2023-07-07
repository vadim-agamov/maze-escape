using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Events;
using Maze.Configs;
using Maze.MazeService;
using UnityEngine;

namespace Maze.Components
{
    public class CharacterComponent : MonoBehaviour, IComponent
    {
        private LinkedList<Vector3> _waypoints;
        private Vector3 _currentWaypoint;
        private Context _context;
        private bool _initialized;

        private const int IdleState = 0;
        private const int WalkState = 1;
        private const int JumpState = 2;
        private int _currentState = IdleState;

        [SerializeField] 
        private float _speed = 1;

        [SerializeField] 
        private Animator _animator;

        [SerializeField] 
        private LineRenderer _lineRenderer;

        private static readonly int State = Animator.StringToHash("state");

        public bool IsWalking => _currentWaypoint == Vector3.zero;
        
        UniTask IComponent.Initialize(Context context)
        {
            _context = context;
            Event<PathUpdatedEvent>.Subscribe(OnPathUpdated);
            _waypoints = new LinkedList<Vector3>();
            _currentWaypoint = Vector3.zero;
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
            var point = evt.Cells.Last().transform.position;

            if (_waypoints.Contains(point))
            {
                while(true)
                {
                    if(_waypoints.Last.Value == point)
                        break;
                    
                    _waypoints.RemoveLast();
                }
            }
            else
            {
                _waypoints.AddLast(point);
            }
        }

        private void SetState(int state)
        {
            if (_currentState == state)
            {
                return;
            }

            _currentState = state;
            _animator.SetInteger(State, _currentState);
        }

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
                SetState(_context.Active ? IdleState : JumpState);
                return;
            }

            transform.position  = Vector3.MoveTowards(transform.position, _currentWaypoint, _speed * Time.deltaTime);
            SetState(WalkState);

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

            UpdateLineRenderer(); 
        }

        private void UpdateLineRenderer()
        {
            if (_currentWaypoint == Vector3.zero)
            {
                _lineRenderer.positionCount = 0;
                return;
            }

            _lineRenderer.positionCount = 2 + _waypoints.Count;
            var index = 0;
            _lineRenderer.SetPosition(index++, transform.position);
            _lineRenderer.SetPosition(index++, _currentWaypoint);
            foreach (var waypoint in _waypoints)
            {
                _lineRenderer.SetPosition(index++, waypoint);
            }
        }

        void IDisposable.Dispose()
        {
            _initialized = false;
            Event<PathUpdatedEvent>.Unsubscribe(OnPathUpdated);
        }
    }
}