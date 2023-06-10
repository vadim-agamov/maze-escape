using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Configs
{
    public partial class LevelEditor
    {
        private void GenerateButton()
        {
            if (_levelConfig == null)
                return;

            if (GUILayout.Button("Generate"))
            {
                Generate();
                FindPath();
                RemoveStartFinishWalls();
            }
        }

        private void RemoveStartFinishWalls()
        {
            var cells = _levelConfig.Cells;

            for (var r = 0; r < cells.GetLength(0); r++)
            for (var c = 0; c < cells.GetLength(1); c++)
            {
                if (cells[r,c].HasFlag(CellType.Finish))
                {
                    cells[r,c] &= ~CellType.UpWall;
                }
                else if (cells[r, c].HasFlag(CellType.Start))
                {
                    cells[r,c] &= ~CellType.DownWall;
                }
            }

            _levelConfig.Cells = cells;
        }

        private void Generate()
        {
            _levelConfig.Clear();
            var rows = _levelConfig.Cells.GetLength(0);
            var cols = _levelConfig.Cells.GetLength(1);

            var r = Random.Range(0, rows);
            var c = Random.Range(0, cols);
            var cells = _levelConfig.Cells;
            
            ProcessCell(r, c, cells);

            MakeStartAndFinish(cells, cols, rows);

            _levelConfig.Cells = cells;
        }

        private static void MakeStartAndFinish(CellType[,] cells, int cols, int rows)
        {
            cells[0, cols / 2] |= CellType.Finish;//&= ~CellType.UpWall;
            cells[rows - 1, cols / 2] |= CellType.Start; // &= ~CellType.DownWall;
        }

        private void ProcessCell(int r, int c, CellType[,] cells)
        {
            cells[r, c] |= CellType.Visited;

            while (true)
            {
                var neighbours = GetUnvisitedNeighbours(r, c, cells);
                if (neighbours.Count == 0)
                    break;

                // var (neighbourRow, neighbourCol, selfWall, neighbourWall) = neighbours.Random();
                // cells[r, c] &= ~selfWall;
                // cells[neighbourRow, neighbourCol] &= ~neighbourWall;


                if (Random.Range(0, 100) < 1)
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