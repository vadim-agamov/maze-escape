using System.Collections.Generic;
using System.Linq;
using Actions;
using Cysharp.Threading.Tasks;
using Events;
using Maze.Configs;
using Maze.MazeService;
using Modules.ServiceLocator;
using UnityEngine;

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

        private Context _context;
        private Camera _camera;
        private bool _initialized;
        private readonly LinkedList<CellView> _path = new LinkedList<CellView>();
        private readonly PathUpdatedEvent _pathUpdatedEvent = new PathUpdatedEvent();
        private FieldViewComponent _fieldViewComponent;

        private void Update()
        {
            if(!_initialized)
                return;
            
            if(!_context.Active)
                return;
            
            if (Input.GetMouseButton(0))
            {
                var hitPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
                var pathCell = _path.Last.Value;//GetNearestCellFromPath(hitPosition);
                // AddCellToPath(pathCell);
                
                var cell = GetNearestNeighbour(_path.Last.Value, hitPosition);
                AddCellToPath(cell);


                // Debug.Log($"pathCell {pathCell.Row},{pathCell.Col}, cell {cell.Row},{cell.Col}");
                // AddCellToPath(pathCell);
                // AddCellToPath(cell);

                UpdateLineRenderer();
                Event<PathUpdatedEvent>.Publish(_pathUpdatedEvent);
            }
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
                    
                    Debug.Log($"remove {_path.Last().Row},{_path.Last().Col}");
                    _path.RemoveLast();
                }
            }
            else
            {
                Debug.Log($"add {cell.Row},{cell.Col}");
                _path.AddLast(cell);
            }
        }

        private void UpdateLineRenderer()
        {
            _lineRenderer.positionCount = _path.Count;
            var index = 0;
            foreach (var cellView in _path)
            {
                var position = cellView.transform.localPosition;
                position.z = -0.5f;
                _lineRenderer.SetPosition(index++, position);
            }
        }

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
            var mazeService = ServiceLocator.Get<IMazeService>();
            _fieldViewComponent = mazeService.GetComponent<FieldViewComponent>();
            
            _context = context;
            _camera = _context.Camera;
            _path.AddLast(_fieldViewComponent.GetStartCell());

            _pathUpdatedEvent.Cells = _path; 

            _initialized = true;
            return UniTask.CompletedTask;
        }

        public void Dispose()
        {
            _initialized = false;
        }

#if DEV
        private void OnGUI()
        {
            if (GUI.Button(new Rect(Screen.width - 210, Screen.height - 50, 200, 50), "Restart"))
            {
                new GotoStateAction(new MazeState(), true).Execute(Bootstrapper.SessionToken).Forget();

                // if (_path.Count > 3)
                // {
                //     _path.RemoveLast();
                // }
                //
                // UpdateLineRenderer();
            }
        }
#endif
    }
}
