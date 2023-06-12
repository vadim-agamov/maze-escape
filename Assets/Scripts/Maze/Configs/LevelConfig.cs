using System;
using System.Collections.Generic;
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

        Visited = 1 << 15,

        Path =  Path0 | Path1 | Path2,
        AllWalls = LeftWall | RightWall | UpWall | DownWall | Start | Finish | Path
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
        private List<Vector2> _paths;

        [SerializeField] 
        private int _levelId;

        public int LevelId
        {
            get => _levelId;
            set { _levelId = value; }
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
                _cells = new CellType[_rows * _cols];
                var index = 0;
                var rows = value.GetLength(0);
                var col = value.GetLength(1);
                for (var r = 0; r < rows; r++)
                {
                    for (var c = 0; c < col; c++)
                    {
                        _cells[index++] = value[r, c] & CellType.AllWalls;
                    }
                }
            }
        }

        public void Clear()
        {
            var res = new CellType[_rows, _cols];
            for (var r = 0; r < _rows; r++)
            {
                for (var c = 0; c < _cols; c++)
                {
                    // if (r > 0)
                        res[r, c] |= CellType.UpWall;

                    // if (r < _rows - 1)
                        res[r, c] |= CellType.DownWall;
                    
                    // if(c > 0)
                        res[r, c] |= CellType.LeftWall;
                    
                    // if(c < _cols - 1)
                        res[r, c] |= CellType.RightWall;
                }
            }

            Cells = res;
        }

        private void OnValidate()
        {
            if(_cells.Length == _rows * _cols)
                return;

            Clear();
        }
    }
}
