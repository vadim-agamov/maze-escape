using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Modules.Events;
using Maze.Configs;
using Maze.Events;
using Maze.Service;
using Modules.InputService;
using Modules.ServiceLocator;
using Modules.Utils;
using UnityEngine;

namespace Maze.Components
{
    public class PathComponent : MonoBehaviour, IComponent
    {
        [SerializeField] 
        private LineRenderer _lineRenderer;

        [SerializeField] 
        private GameObject _cursorPreafab;
        
        private GameObject _beginCursor;
        private GameObject _cursor;
        private Context _context;
        private Camera _camera;
        private bool _initialized;
        private readonly LinkedList<CellView> _path = new LinkedList<CellView>();
        private readonly ValueChangedTrigger<bool> _touchTrigger = new ValueChangedTrigger<bool>();
        private FieldViewComponent _fieldViewComponent;
        private IInputService _inputService;

        private void Update()
        {
            if(!_initialized)
                return;
            
            if(!_context.Active)
                return;

            var touchChanged = _touchTrigger.SetValue(_inputService.TouchesCount > 0);


            if (_touchTrigger.Value)
            {
                var hitPosition = _camera.ScreenToWorldPoint(_inputService.Touch0);
                hitPosition = new Vector3(hitPosition.x, hitPosition.y, 0);
                var cellView = _path.FirstOrDefault(cell => Vector3.Distance(cell.transform.position, hitPosition) < 0.5f);
                
                // touch cell in path -> cut loop
                if (touchChanged && cellView != null)
                {
                    AddCellToPath(cellView);
                }

                // touch cell not in path -> add cell to path
                else 
                {
                    var cell = GetNearestNeighbour(_path.Last.Value, hitPosition);
                    AddCellToPath(cell);
                }
            }

            _cursor.transform.position = _path.Last.Value.transform.position;
            _cursor.SetActive(_context.Active);
            
            UpdateLineRenderer();
        }

        private CellView GetNearestNeighbour(CellView cell, Vector3 hitPosition)
        {
            var neighbors = new List<CellView>
            {
                cell
            };

            if (!cell.CellType.HasFlag(CellType.UpWall))
            {
                var r = cell.Row - 1;
                var c = cell.Col;
                AddNeighbor(r, c);
            }
            
            if (!cell.CellType.HasFlag(CellType.DownWall))
            {
                var r = cell.Row + 1;
                var c = cell.Col;
                AddNeighbor(r, c);
            }
            
            if (!cell.CellType.HasFlag(CellType.LeftWall))
            {
                var r = cell.Row;
                var c = cell.Col-1;
                AddNeighbor(r, c);
            }
            
            if (!cell.CellType.HasFlag(CellType.RightWall))
            {
                var r = cell.Row;
                var c = cell.Col + 1;
                AddNeighbor(r, c);
            }
            
            var prevDistance = float.MaxValue;
            var result = neighbors.First();
            foreach (var neighbor in neighbors)
            {
                var distance = Vector3.Distance(neighbor.transform.position, hitPosition);
                if (distance < prevDistance)
                {
                    prevDistance = distance;
                    result = neighbor;
                }
            }

            return result;

            void AddNeighbor(int r, int c)
            {
                if (r >= 0 && r < _context.Cells.GetLength(0) &&
                    c >= 0 && c < _context.Cells.GetLength(1))
                {
                    neighbors.Add(_fieldViewComponent.GetCellView(r,c));
                }
            }
        }

        private void AddCellToPath(CellView cell)
        {
            var deltaCells = new List<CellView>();
            while (_path.Contains(cell))
            {
                deltaCells.Add(_path.Last.Value);
                _path.RemoveLast();
            }

            deltaCells.Add(cell);
            _path.AddLast(cell);
            Event<PathUpdatedEvent>.Publish(new PathUpdatedEvent { Cells = deltaCells });
        }

        private void UpdateLineRenderer()
        {
            var pathPoints = _path.Select(x => x.transform.position).ToList();
            var points = Bezier.AddSegments(pathPoints, 3);
            var smoothLine = Bezier.Create(points, 5).Skip(1);

            var index = 0;
            foreach (var pos in smoothLine)
            {
                _lineRenderer.positionCount = index + 1;
                _lineRenderer.SetPosition(index, pos);
                index++;
            }
            _lineRenderer.useWorldSpace = true;
        }

        // private void OnDrawGizmos()
        // {
        //     for (var i = 0; i < _lineRenderer.positionCount; i++)
        //     {
        //         Gizmos.color = Color.red;
        //         Gizmos.DrawSphere(_lineRenderer.GetPosition(i), 0.05f);
        //     }
        // }

        // private void OnDrawGizmos()
        // {
        //     var points = Bezier.AddSegments(_path.Select(x => x.transform.position).ToList(), 2);
        //     foreach (var point in points)
        //     {
        //         Gizmos.color = Color.red;
        //         Gizmos.DrawSphere(point, 0.2f);
        //     }
        //
        //     var smoothLine = Bezier.Create(points, 10);
        //     foreach (var point in smoothLine)
        //     {
        //         Gizmos.color = Color.yellow;
        //         Gizmos.DrawSphere(point, 0.1f);
        //     }
        // }
        
        public UniTask Initialize(Context context, IMazeService mazeService)
        {
            _inputService = ServiceLocator.Get<IInputService>();
            _fieldViewComponent = mazeService.GetComponent<FieldViewComponent>();

            _context = context;
            _camera = _context.Camera;
            _path.AddLast(_fieldViewComponent.GetStartCell());

            _cursor = Instantiate(_cursorPreafab, transform);
            _beginCursor = Instantiate(_cursorPreafab, transform);
            _beginCursor.transform.position = _path.First.Value.transform.position;

            _initialized = true;
            return UniTask.CompletedTask;
        }

        public void Dispose()
        {
            _initialized = false;
        }
    }
}
