using System.Collections.Generic;
using System.Linq;
using Configs;
using Cysharp.Threading.Tasks;
using Events;
using Maze.MazeService;
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

        private Camera _camera;
        private readonly List<CellView> _path = new List<CellView>();
        private Transform _lastCell;
        private bool _active;
        private PathUpdatedEvent _pathUpdatedEvent = new PathUpdatedEvent();

        private void Update()
        {
            if(!_active)
                return;
            
            if (Input.GetMouseButton(0))
            {
                var hit = Physics2D.Raycast(_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
 
                if(hit.collider != null)
                {
                    if(hit.transform == _lastCell)
                        return;

                    _lastCell = hit.transform;

                    var cell = hit.transform.gameObject.GetComponent<CellView>();

                    if (_path.Contains(cell))
                    {
                        while (_path.Count > 0 &&
                               _path.Last() != cell)
                        {
                            _path.Last().IsSelected = false;
                            _path.RemoveAt(_path.Count - 1);
                        }
                    }
                    else
                    {
                        if(_path.Count > 0 && !_path.Last().Contact(cell))
                            return;
                        
                        _path.Add(cell);
                        cell.IsSelected = true;
                    }
                    
                    UpdateLineRenderer();

                    _pathUpdatedEvent.LastCell = cell;
                    Event<PathUpdatedEvent>.Publish(_pathUpdatedEvent);
                }
                else
                {
                    _lastCell = null;
                }
            }
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

        public void Dispose()
        {
            _active = false;
        }

        public UniTask Initialize(Context context)
        {
            context.Path = _path;
            _camera = context.Camera;
            _active = true;
            return UniTask.CompletedTask;
        }
    }
}
