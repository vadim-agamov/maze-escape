using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Maze.Configs;
using Maze.Service;
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
        private const float CellSize = 1.5f;
        
        public UniTask Initialize(Context context, IMazeService mazeService)
        {
            var portraitScreen = Screen.width < Screen.height;
            Debug.Log($"[{nameof(FieldViewComponent)}] Initialize begin, portraitScreen: {portraitScreen} {Screen.width}x{Screen.height}");
            
            _context = context;
            _context.Cells = portraitScreen ? context.Level.Cells : context.Level.PivotedCells;
            _context.Goals = portraitScreen ? context.Level.Goals : context.Level.PivotedGoals;
            
            var rows = _context.Cells.GetLength(0);
            var cols = _context.Cells.GetLength(1);

            _rectTransform.SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, cols * CellSize + _padding.x);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rows * CellSize + _padding.y);
            _cellsContainer.transform.position -= new Vector3(_padding.x, _padding.y, 0);
            var rect = _rectTransform.rect;
            var startCell = new Vector2(CellSize * 0.5f + rect.xMin, -CellSize * 0.5f + rect.yMax);

            var cellIds = new HashSet<string>();
            _context.CellViews = _cellViews = new CellView[rows, cols];
            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    var configCell = _context.Cells[r, c];
                    var cell = Instantiate(_cell, _cellsContainer.transform, true);
                    cell.transform.localPosition = startCell + new Vector2(CellSize * c, CellSize * -r);
                    cell.Setup(configCell, r, c, cellIds);
                    _cellViews[r, c] = cell;
                }
            }
            
            var firstGoal = context.Goals.First();
            _cellViews[firstGoal.Row, firstGoal.Col].AddType(CellType.Finish);

            context.Active = true;
            
            _initialized?.Invoke();
            
            return UniTask.CompletedTask;
        }
        
        void IComponent.Update()
        {
            // nothing
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