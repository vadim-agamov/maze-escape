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
        private float _paddingTop;
        
        [SerializeField] 
        private float _paddingBottom;
        
        [SerializeField] 
        private float _paddingLeft;
        
        [SerializeField] 
        private float _paddingRight;

        // [SerializeField]
        // private Transform _cellsContainer;

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

            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, cols * CellSize + _paddingLeft + _paddingRight);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rows * CellSize + _paddingBottom + _paddingTop);
            var rect = _rectTransform.rect;
            var startCell = new Vector2(rect.xMin + CellSize * 0.5f, rect.yMax - CellSize * 0.5f);
            startCell += new Vector2(_paddingLeft, -_paddingTop);

            var cellIds = new HashSet<string>();
            _context.CellViews = _cellViews = new CellView[rows, cols];
            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    var configCell = _context.Cells[r, c];
                    var cell = Instantiate(_cell, transform);
                    cell.transform.localPosition = startCell + new Vector2(CellSize * c, CellSize * -r);
                    cell.Setup(configCell, r, c, cellIds);
                    _cellViews[r, c] = cell;
                }
            }
            
            var firstGoal = context.Goals.First();
            _cellViews[firstGoal.Row, firstGoal.Col].AddType(CellType.Finish);
            _context.GoalsFactoryComponent.Create(_cellViews[firstGoal.Row, firstGoal.Col]).PlayAppear().Forget();

            context.Active = true;
            
            _initialized?.Invoke();
            
            return UniTask.CompletedTask;
        }
        
        void IComponent.Tick()
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