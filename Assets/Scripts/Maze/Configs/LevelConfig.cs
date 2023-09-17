using System;
using UnityEngine;

namespace Maze.Configs
{
    [Flags]
    public enum CellType : int
    {
        NoWall = 0,
        LeftWall = 1 << 0,
        RightWall = 1 << 1,
        UpWall = 1 << 2,
        DownWall = 1 << 3,

        Start = 1 << 4,
        Finish = 1 << 5,
        Path0 = 1 << 6,
        Path1 = 1 << 7,
        Path2 = 1 << 8,
        PathItem = 1 << 9,

        Visited = 1 << 15,
        
        Path =  Path0 | Path1 | Path2,
        PermanentTypes = LeftWall | RightWall | UpWall | DownWall | Start | Finish | Path | PathItem
    }

    [CreateAssetMenu(menuName = "Create LevelConfig", fileName = "LevelConfig", order = 0)]
    public class LevelConfig : ScriptableObject
    {
        [SerializeField] 
        private CellType[] _cells;

        [SerializeField] 
        private int _rows;

        [SerializeField] 
        private int _cols;
        
        [SerializeField] 
        private int _levelId;

        [SerializeField]
        private int _minPath;

        public int LevelId
        {
            get => _levelId;
            set { _levelId = value; }
        }

        public int MinPath => _minPath;
        public void SetMinPath(int v) => _minPath = v;
        
        public CellType[,] PivotedCells
        {
            /*
             *   U     U -> L 
             * L   R   R -> D
             *   D     L -> U
             *         D -> R
             *    c1 c2 c3
             * r1 a  b  c   
             * r2 d  e  f
             *
             *    c1 c2
             * r1 a  d
             * r2 b  e
             * r3 c  f
             */
            
            get
            {
                var res = new CellType[_cols, _rows];
                for (var r = 0; r < _rows; r++)
                {
                    for (var c = 0; c < _cols; c++)
                    {
                        var cell = _cells[r * _cols + c];
                        var pivotedCell = new CellType();
                        if(cell.HasFlag(CellType.Start)) pivotedCell |= CellType.Start;
                        if(cell.HasFlag(CellType.Finish)) pivotedCell |= CellType.Finish;
                        
                        if(cell.HasFlag(CellType.UpWall)) pivotedCell |= CellType.LeftWall;
                        if(cell.HasFlag(CellType.RightWall)) pivotedCell |= CellType.DownWall;
                        if(cell.HasFlag(CellType.LeftWall)) pivotedCell |= CellType.UpWall;
                        if(cell.HasFlag(CellType.DownWall)) pivotedCell |= CellType.RightWall;
                        
                        res[c, r] = pivotedCell;
                    }
                }

                return res;
            }
        }

        public CellType[,] Cells 
        {
            get
            {
                var res = new CellType[_rows, _cols];
                for (var r = 0; r < _rows; r++)
                {
                    for (var c = 0; c < _cols; c++)
                    {
                        res[r, c] = _cells[r * _cols + c];
                    }
                }

                return res;
            }
            set
            {
                _rows = value.GetLength(0);
                _cols = value.GetLength(1);
                _cells = new CellType[_rows * _cols];
                var index = 0;
                var rows = value.GetLength(0);
                var col = value.GetLength(1);
                for (var r = 0; r < rows; r++)
                {
                    for (var c = 0; c < col; c++)
                    {
                        _cells[index++] = value[r, c] & CellType.PermanentTypes;
                    }
                }
            }
        }
    }
}
