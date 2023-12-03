using System;
using System.Collections.Generic;

namespace Maze.Configs.Editor
{
    public class GoalFinder
    {
        private readonly Stack<(int Row, int Col)> _currentPath = new ();
        private readonly CellType[,] _cells;
        private int _row;
        private int _col;
        private bool _found;
        private int _desiredLenght;
        private int _length;

        public GoalFinder(CellType[,] cells, int desiredLenght)
        {
            _cells = cells;
            _desiredLenght = desiredLenght;
        }
            
        public (int Row, int Col, int Length) Execute(int r, int c)
        {
            FindPath(r, c);
            if (!_found)
            {
                throw new Exception("can't find path");
            }

            return (Row: _row, Col: _col, Length: _length);
        }
            
        private void FindPath(int r, int c)
        {
            if (_currentPath.Count >= _desiredLenght && !_cells[r, c].HasFlag(CellType.Start) && !_cells[r, c].HasFlag(CellType.PathItem))
            {
                _found = true;
                _row = r;
                _col = c;
                _length = _currentPath.Count;
                _cells[r, c] |= CellType.PathItem;
                return;
            }
            
            _currentPath.Push((Row: r, Col: c));
            _cells[r, c] |= CellType.Visited;
            var cell = _cells[r, c];
                
            if (!_found &&
                !cell.HasFlag(CellType.RightWall) &&
                c < _cells.GetLength(1) - 1 &&
                !_cells[r, c + 1].HasFlag(CellType.Visited))
            {
                FindPath(r, c + 1);
            }

            if (!_found &&
                !cell.HasFlag(CellType.LeftWall) && 
                c > 0 && 
                !_cells[r, c - 1].HasFlag(CellType.Visited))
            {
                FindPath(r, c - 1);
            }

            if (!_found &&
                !cell.HasFlag(CellType.DownWall) && 
                r < _cells.GetLength(0) - 1 && 
                !_cells[r + 1, c].HasFlag(CellType.Visited))
            {
                FindPath(r + 1, c);
            }

            if (!_found &&
                !cell.HasFlag(CellType.UpWall) && 
                r > 0 && 
                !_cells[r - 1, c].HasFlag(CellType.Visited))
            {
                FindPath(r - 1, c);
            }

            _currentPath.Pop();
            _cells[r, c] &= ~CellType.Visited;
        }
    }
}