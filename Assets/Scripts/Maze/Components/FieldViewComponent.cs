using System;
using Cysharp.Threading.Tasks;
using Maze.Configs;
using Maze.MazeService;
using UnityEngine;

namespace Maze.Components
{
    public class FieldViewComponent : MonoBehaviour, IComponent
    {
        [SerializeField] 
        private CellView _cell;

        [SerializeField]
        private RectTransform _rectTransform;

        private CellView[,] _cellViews;

        private Context _context;
        private float _cellSize = 1.5f;

        public UniTask Initialize(Context context)
        {
            _context = context;
            _context.Cells = context.Level.Cells;
            var rows = _context.Cells.GetLength(0);
            var cols = _context.Cells.GetLength(1);

            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, cols * _cellSize);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rows * _cellSize);
            var rect = _rectTransform.rect;
            var startCell = new Vector2(_cellSize * 0.5f + rect.xMin, -_cellSize * 0.5f + rect.yMax);

            _cellViews = new CellView[rows, cols];
            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    var configCell = _context.Cells[r, c];
                    var cell = Instantiate(_cell, transform, true);
                    cell.transform.localPosition = startCell + new Vector2(_cellSize * c, _cellSize * -r);
                    cell.Setup(configCell, r, c);
                    _cellViews[r, c] = cell;
                }
            }

            context.Active = true;
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
        
#if DEV
        private void OnGUI()
        {
            if(_context == null || !_context.Active)
                return;
            
            GUI.Label(new Rect(10, Screen.height - 20, 200, 20), $"level {_context.Level.LevelId}");
        }
#endif
    }
}