using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Modules.Events;
using Maze.Configs;
using Maze.MazeService;
using Modules.InputService;
using Modules.ServiceLocator;
using UnityEngine;
using Utils;

namespace Maze.Components
{
    public class PathUpdatedEvent
    {
        public LinkedList<CellView> Cells;
    }

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
        private readonly PathUpdatedEvent _pathUpdatedEvent = new PathUpdatedEvent();
        private FieldViewComponent _fieldViewComponent;
        private IInputService _inputService;

        private void Update()
        {
            if(!_initialized)
                return;
            
            if(!_context.Active)
                return;
            
            if (_inputService.TouchesCount > 0)
            {
                var hitPosition = _camera.ScreenToWorldPoint(_inputService.Touch0);
                hitPosition = new Vector3(hitPosition.x, hitPosition.y, 0);
                // _cursor.transform.position = new Vector3(hitPosition.x, hitPosition.y, 0);
                // var pathCell = _path.Last.Value;//GetNearestCellFromPath(hitPosition);
                // AddCellToPath(pathCell);
                
                var cell = GetNearestNeighbour(_path.Last.Value, hitPosition);
                AddCellToPath(cell);

                // var distance = Vector3.Distance(cell.transform.position, hitPosition);
                // var t = 0.5f / distance;
                _cursor.transform.position = cell.transform.position;// Vector3.Lerp(cell.transform.position, hitPosition, t);
                
                // Debug.Log($"pathCell {pathCell.Row},{pathCell.Col}, cell {cell.Row},{cell.Col}");
                // AddCellToPath(pathCell);
                // AddCellToPath(cell);

                UpdateLineRenderer();
                Event<PathUpdatedEvent>.Publish(_pathUpdatedEvent);
            }
            else
            {
                _cursor.transform.position = _path.Last.Value.transform.position;
            }
            
            _cursor.SetActive(_context.Active);
        }

        private CellView GetNearestCellFromPath(Vector3 position)
        {
            // var prevDistance = float.MaxValue;
            var cell = _path.First.Value;
            foreach (var p in _path)
            {
                var distance = Vector3.Distance(p.transform.position, position);
                Debug.Log($"distance {distance}");
                if (distance < 1.5f)
                    return p;
                // if (distance < prevDistance)
                // {
                // prevDistance = distance;
                // cell = p;
                // }
            }

            return _path.Last.Value;
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
                    // Contact()
                    // _path.Count(x => x.Row == r && x.Col == c) == 0)
                {
                    neighbors.Add(_fieldViewComponent.GetCellView(r,c));
                }
            }
        }

        private void AddCellToPath(CellView cell)
        {
            if (_path.Contains(cell))
            {
                while(true)
                {
                    if(_path.Last.Value == cell)
                        break;
                    
                    _path.RemoveLast();
                }
            }
            else
            {
                _path.AddLast(cell);
            }
        }

        private void UpdateLineRenderer()
        {
            var pathPoints = _path.Select(x => x.transform.position).ToList();
            var points = Bezier.AddSegments(pathPoints, 3);
            var smoothLine = Bezier.Create(points, 5);

            _lineRenderer.positionCount = smoothLine.Count;
            for (var i = 0; i < smoothLine.Count; i++)
            {
                _lineRenderer.SetPosition(i, smoothLine[i]);
            }

            _lineRenderer.useWorldSpace = true;
        }

        private void OnDrawGizmos()
        {
            for (var i = 0; i < _lineRenderer.positionCount; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(_lineRenderer.GetPosition(i), 0.05f);
            }
        }

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

        private bool Contact(CellView a, CellView b)
        {
            if (a.Col == b.Col && a.Row == b.Row)
                return true;
            
            if (a.Col == b.Col)
            {
                if (b.Row < a.Row)
                {
                    (a, b) = (b, a);
                }

                for (var r = a.Row; r < b.Row; r++)
                {
                    var cell = _context.Cells[r, a.Col];
                    if (cell.HasFlag(CellType.DownWall))
                        return false;
                }

                return true;
            }
            
            if (a.Row == b.Row)
            {
                if (b.Col < a.Col)
                {
                    (a, b) = (b, a);
                }

                for (var c = a.Col; c < b.Col; c++)
                {
                    var cell = _context.Cells[a.Row, c];
                    if (cell.HasFlag(CellType.RightWall))
                        return false;
                }

                return true;
            }

            return false;
        }

        public UniTask Initialize(Context context)
        {
            _inputService = ServiceLocator.Get<IInputService>();
            var mazeService = ServiceLocator.Get<IMazeService>();
            _fieldViewComponent = mazeService.GetComponent<FieldViewComponent>();

            _context = context;
            _camera = _context.Camera;
            _path.AddLast(_fieldViewComponent.GetStartCell());

            _pathUpdatedEvent.Cells = _path;

            _cursor = GameObject.Instantiate(_cursorPreafab, transform);
            _beginCursor = GameObject.Instantiate(_cursorPreafab, transform);
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
