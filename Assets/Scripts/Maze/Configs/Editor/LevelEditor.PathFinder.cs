using System;
using System.Collections.Generic;

namespace Maze.Configs.Editor
{
    public partial class LevelEditor
    {
        private int _totalPath;
        
        (int Row, int Col) GetStart()
        {
            var cells = _cells;
            for (var r = 0; r < cells.GetLength(0); r++)
            {
                for (var c = 0; c < cells.GetLength(1); c++)
                {
                    var cellType = cells[r, c];
                    if (cellType.HasFlag(CellType.Start))
                        return (Row: r, Col: c);
                }
            }

            throw new Exception("can't find start");
        }
        
        private List<(int NeigbourRow, int NeigbourCol, CellType SelfWall, CellType NeigbourWall)> GetUnvisitedReachableNeighbours(int r, int c, CellType[,] cells)
        {
            var neighbours = new List<(int NeigbourRow, int NeigbourCol, CellType SelfWall, CellType NeigbourWall)>();
            
            var cell = cells[r, c];
            
            if (!cell.HasFlag(CellType.UpWall) && r > 0 && !cells[r - 1, c].HasFlag(CellType.Visited))
            {
                neighbours.Add((NeigbourRow: r-1, NeigbourCol: c, SelfWall: CellType.UpWall, NeigbourWall: CellType.DownWall));
            }
            
            if (!cell.HasFlag(CellType.DownWall) && r < cells.GetLength(0) - 1 && !cells[r + 1, c].HasFlag(CellType.Visited))
            {
                neighbours.Add((NeigbourRow: r+1, NeigbourCol: c, SelfWall: CellType.DownWall, NeigbourWall: CellType.UpWall));
            }
            
            if (!cell.HasFlag(CellType.LeftWall) && c > 0 && !cells[r, c-1].HasFlag(CellType.Visited))
            {
                neighbours.Add((NeigbourRow: r, NeigbourCol: c-1, SelfWall: CellType.LeftWall, NeigbourWall: CellType.RightWall));
            }
            
            if (!cell.HasFlag(CellType.RightWall) && c < cells.GetLength(1) - 1 && !cells[r, c+1].HasFlag(CellType.Visited))
            {
                neighbours.Add((NeigbourRow: r, NeigbourCol: c+1, SelfWall: CellType.RightWall, NeigbourWall: CellType.LeftWall));
            }

            return neighbours;
        }
    }
}