using System.Collections.Generic;
using Modules.Extensions;
using Random = UnityEngine.Random;

namespace Maze.Configs.Editor
{
    public partial class LevelEditor
    {
        private string _sizeFiled;
        private int _size = 5;
        private CellType[,] _cells;

        private void GenerateMaze()
        {
            _cells = new CellType[_size * 2, _size];
            var rows = _cells.GetLength(0);
            var cols = _cells.GetLength(1);

            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    _cells[r, c] |= CellType.UpWall;
                    _cells[r, c] |= CellType.DownWall;
                    _cells[r, c] |= CellType.LeftWall;
                    _cells[r, c] |= CellType.RightWall;
                }
            }

            ProcessCell(Random.Range(0, rows), Random.Range(0, cols), _cells);
            
            _cells[Random.Range(rows / 2, rows), Random.Range(0, cols)] |= CellType.Start;
            
            for (var row = 0; row < _cells.GetLength(0); row++)
            for (var col = 0; col < _cells.GetLength(1); col++)
            {
                _cells[row, col] &= ~CellType.Visited;
            }
        }
        
        private void ProcessCell(int r, int c, CellType[,] cells)
        {
            cells[r, c] |= CellType.Visited;

            while (true)
            {
                var neighbours = GetUnvisitedNeighbours(r, c, cells);
                if (neighbours.Count == 0)
                    break;
                
                if (Random.Range(0, 5) == 0)
                {
                    BrakeRandomWall(neighbours);
                }

                var (neighbourRow, neighbourCol) = BrakeRandomWall(neighbours);
                
                ProcessCell(neighbourRow, neighbourCol, cells);
            }

            (int NeigbourRow, int NeigbourCol) BrakeRandomWall(List<(int NeigbourRow, int NeigbourCol, CellType SelfWall, CellType NeigbourWall)> neighbours)
            {
                var (neighbourRow, neighbourCol, selfWall, neighbourWall) = neighbours.Random();
                cells[r, c] &= ~selfWall;
                cells[neighbourRow, neighbourCol] &= ~neighbourWall;

                return (NeigbourRow: neighbourRow, NeigbourCol: neighbourCol);
            }
        }

        private List<(int NeigbourRow, int NeigbourCol, CellType SelfWall, CellType NeigbourWall)> GetUnvisitedNeighbours(int r, int c, CellType[,] cells)
        {
            var neighbours = new List<(int NeigbourRow, int NeigbourCol, CellType SelfWall, CellType NeigbourWall)>();
            if (r > 0 && !cells[r - 1, c].HasFlag(CellType.Visited))
            {
                neighbours.Add((NeigbourRow: r-1, NeigbourCol: c, SelfWall: CellType.UpWall, NeigbourWall: CellType.DownWall));
            }
            
            if (r < cells.GetLength(0) - 1 && !cells[r+1, c].HasFlag(CellType.Visited))
            {
                neighbours.Add((NeigbourRow: r+1, NeigbourCol: c, SelfWall: CellType.DownWall, NeigbourWall: CellType.UpWall));
            }
            
            if (c > 0 && !cells[r, c-1].HasFlag(CellType.Visited))
            {
                neighbours.Add((NeigbourRow: r, NeigbourCol: c-1, SelfWall: CellType.LeftWall, NeigbourWall: CellType.RightWall));
            }
            
            if (c < cells.GetLength(1) - 1 && !cells[r, c+1].HasFlag(CellType.Visited))
            {
                neighbours.Add((NeigbourRow: r, NeigbourCol: c+1, SelfWall: CellType.RightWall, NeigbourWall: CellType.LeftWall));
            }
            
            return neighbours;
        }
    }
}