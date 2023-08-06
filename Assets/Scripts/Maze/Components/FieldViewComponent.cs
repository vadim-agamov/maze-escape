using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Maze.Configs;
using Maze.MazeService;
using UnityEngine;
using UnityEngine.Events;

namespace Maze.Components
{
    public class FieldViewComponent : MonoBehaviour, IComponent
    {
        [SerializeField] 
        private CellView _cell;

        [SerializeField]
        private RectTransform _rectTransform;

        [SerializeField] 
        private Vector2 _padding;

        [SerializeField]
        private Transform _cellsContainer;

        [SerializeField]
        private UnityEvent _initialized;

        private CellView[,] _cellViews;

        private Context _context;
        private float _cellSize = 1.5f;

        public UniTask Initialize(Context context)
        {
            _context = context;
            _context.Cells = context.Level.Cells;
            var rows = _context.Cells.GetLength(0);
            var cols = _context.Cells.GetLength(1);

            _rectTransform.SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, cols * _cellSize + _padding.x);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rows * _cellSize + _padding.y);
            _cellsContainer.transform.position -= new Vector3(_padding.x, _padding.y, 0);
            var rect = _rectTransform.rect;
            var startCell = new Vector2(_cellSize * 0.5f + rect.xMin, -_cellSize * 0.5f + rect.yMax);

            var cellIds = new HashSet<string>();
            _context.CellViews = _cellViews = new CellView[rows, cols];
            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    var configCell = _context.Cells[r, c];
                    var cell = Instantiate(_cell, _cellsContainer.transform, true);
                    cell.transform.localPosition = startCell + new Vector2(_cellSize * c, _cellSize * -r);
                    cell.Setup(configCell, r, c, cellIds);
                    _cellViews[r, c] = cell;
                }
            }

            context.Active = true;
            
            _initialized?.Invoke();
            
            return UniTask.CompletedTask;
        }
        
        public CellView GetCellView(int r, int c) => _cellViews[r, c];

        public CellView GetStartCell()
        {
            foreach (var cellView in _cellViews)
            {
                if (cellView.CellType.HasFlag(CellType.Start))
                    return cellView;
            }

            throw new Exception("no start cell view");
        }

        public void Dispose()
        {
        }
    }
}