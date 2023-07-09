using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Events;
using Maze.Configs;
using Maze.MazeService;
using UnityEngine;
using UnityEngine.Pool;

namespace Maze.Components
{
    public class CharacterComponent : MonoBehaviour, IComponent
    {
        private LinkedList<CrabWaypoint> _waypoints;
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
        private CrabWaypoint _crabWaypoint;
        
        private ObjectPool<CrabWaypoint> _waypointsPool;

        private static readonly int State = Animator.StringToHash("state");

        public bool IsWalking => _currentWaypoint == Vector3.zero;
        
        UniTask IComponent.Initialize(Context context)
        {
            _context = context;
            Event<PathUpdatedEvent>.Subscribe(OnPathUpdated);
            _waypoints = new LinkedList<CrabWaypoint>();
            _waypointsPool = new ObjectPool<CrabWaypoint>(CreateWaypoint, GetWaypoint, ReleaseWaypoint, DestroyWaypoint);
            _currentWaypoint = Vector3.zero;
            SetupStartPosition();
            _initialized = true;
            return UniTask.CompletedTask;
        }

        #region Pool
        private CrabWaypoint CreateWaypoint() => Instantiate(_crabWaypoint);

        private void DestroyWaypoint(CrabWaypoint waypoint) => Destroy(waypoint.gameObject);

        private void GetWaypoint(CrabWaypoint waypoint) => waypoint.Show().Forget();

        private void ReleaseWaypoint(CrabWaypoint waypoint) => waypoint.Hide().Forget();

        #endregion
        
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
            var lastCell = evt.Cells.Last();
            if(lastCell.CellType.HasFlag(CellType.Finish))
                return;
            
            var point = lastCell.transform.position;

            if (_waypoints.Count(w => w.transform.position == point) > 0) 
            {
                while(true)
                {
                    var waypoint = _waypoints.Last.Value;

                    if(waypoint.transform.position == point)
                        break;
                    
                    _waypointsPool.Release(waypoint);
                    _waypoints.RemoveLast();
                }
            }
            else
            {
                var newWaypoint = _waypointsPool.Get();
                newWaypoint.transform.position = point;
                _waypoints.AddLast(newWaypoint);

                if (_waypoints.Count >= 2)
                {
                    var last = _waypoints.Last.Value;
                    var preLast = _waypoints.Last.Previous.Value;
                    
                    var intermediate =  _waypointsPool.Get();
                    intermediate.transform.position = Vector3.Lerp(last.transform.position, preLast.transform.position, 0.5f);
                
                    _waypoints.AddBefore(_waypoints.Last, intermediate);
                }
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
                _currentWaypoint = _waypoints.Last.Value.transform.position;
                _waypointsPool.Release(_waypoints.Last.Value);
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
                    _currentWaypoint = _waypoints.First.Value.transform.position;
                    _waypointsPool.Release(_waypoints.First.Value);
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
            _waypointsPool.Dispose();
            Event<PathUpdatedEvent>.Unsubscribe(OnPathUpdated);
        }
    }
}