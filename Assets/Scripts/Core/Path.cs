using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core
{
    public class Path : MonoBehaviour
    {
        [SerializeField] 
        private LineRenderer _lineRenderer;
        private readonly List<CellView> _path = new List<CellView>();
        private Transform _lastCell;

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
 
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
    }
}
