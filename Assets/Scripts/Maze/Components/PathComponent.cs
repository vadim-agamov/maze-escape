using System.Collections.Generic;
using System.Linq;
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
        public CellView LastCell;
    }

    public class PathComponent : MonoBehaviour, IComponent
    {
        [SerializeField] 
        private LineRenderer _lineRenderer;

        private Context _context;
        private Camera _camera;
        private bool _initialized;
        private readonly List<CellView> _path = new List<CellView>();
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
                var pathCell = GetNearestCellFromPath(hitPosition);
                var cell = GetNearestNeighbour(pathCell, hitPosition);
                // Debug.Log($"pathCell {pathCell.Row},{pathCell.Col}, cell {cell.Row},{cell.Col}");
                AddCellToPath(pathCell);
                AddCellToPath(cell);
            }
        }

        private CellView GetNearestCellFromPath(Vector3 position)
        {
            var prevDistance = float.MaxValue;
            var cell = _path.First();
            foreach (var p in _path)
            {
                var distance = Vector3.Distance(p.transform.position, position);
                if (distance < prevDistance)
                {
                    prevDistance = distance;
                    cell = p;
                }
            }

            return cell;
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
                    c >= 0 && c < _context.Cells.GetLength(1) &&
                    _path.Count(x => x.Row == r && x.Col == c) == 0)
                {
                    neighbors.Add(_fieldViewComponent.GetCellView(r,c));
                }
            }
        }

        private void AddCellToPath(CellView cell)
        {
            if (_path.Contains(cell))
            {
                while(_path.Last() != cell)
                {
                    Debug.Log($"remove {_path.Last().Row},{_path.Last().Col}");
                    _path.RemoveAt(_path.Count - 1);
                }
            }
            else
            {
                // if (_path.Count > 0 && !Contact(_path.Last(), cell))
                // return;

                // Debug.Log($"pathCell {pathCell.Row},{pathCell.Col}, cell {cell.Row},{cell.Col}");

                Debug.Log($"add {cell.Row},{cell.Col}");
                _path.Add(cell);
            }

            UpdateLineRenderer();

            _pathUpdatedEvent.LastCell = cell;
            Event<PathUpdatedEvent>.Publish(_pathUpdatedEvent);
        }

        private void UpdateLineRenderer()
        {
            _lineRenderer.positionCount = _path.Count;
            for (var i = 0; i < _path.Count; i++)
            {
                var cellView = _path[i];
                var position = cellView.transform.position;
                position.z = -0.5f;
                _lineRenderer.SetPosition(i, position);
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
            _context.Path = _path;
            _camera = _context.Camera;
            _path.Add(_fieldViewComponent.GetStartCell());

            _initialized = true;
            return UniTask.CompletedTask;
        }

        public void Dispose()
        {
            _initialized = false;
        }
    }
}
