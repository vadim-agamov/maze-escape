using System.Linq;
using Cysharp.Threading.Tasks;
using Maze.Service;
using Modules.Pool;
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
        
        private GameObject _cursor;
        private Context _context;

        public UniTask Initialize(Context context, IMazeService mazeService)
        {
            _context = context;
            _cursor = Instantiate(_cursorPreafab, transform);
            return UniTask.CompletedTask;
        }
        
        void IComponent.Update()
        {
            if (_context.Active && _context.Path.Count > 0)
            {
                _cursor.transform.position = _context.Path.Last.Value.transform.position;
                _cursor.SetActive(true);
            }
            else
            {
                _cursor.SetActive(false);
            }
            
            UpdateLineRenderer();
        }

        private void UpdateLineRenderer()
        {
            using var d0 = ScopedList<Vector3>.Create(out var pathPoints);
            using var d1 = ScopedList<Vector3>.Create(out var points);
            using var d2 = ScopedList<Vector3>.Create(out var smoothLine);
            
            pathPoints.AddRange(_context.Path.Reverse().Select(x => x.transform.position));
            pathPoints.Add(_context.CharacterPositionProvider.Position);
            
            Bezier.AddSegments(pathPoints, 3, points);
            Bezier.Create(points, 5, smoothLine);

            _lineRenderer.positionCount = 0;
            var index = 0;
            foreach (var pos in smoothLine.Skip(1))
            {
                _lineRenderer.positionCount = index + 1;
                _lineRenderer.SetPosition(index, pos);
                index++;
            }
            _lineRenderer.useWorldSpace = true;
        }

        private void OnDrawGizmos()
        {
            if (_context == null || _context.Path.Count == 0)
            {
                return;
            }

            foreach (var waypoint in _context.Path.Select(x=>x.transform.position))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(waypoint, 0.2f);
            }
        }

        public void Dispose()
        {
        }
    }
}
