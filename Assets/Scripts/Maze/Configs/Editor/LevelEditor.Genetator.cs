using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utils;

namespace Maze.Configs.Editor
{
    public partial class LevelEditor
    {
        private string _sizeFiled;
        private int _size = 5;
        private CellType[,] _cells;
        
        private void GenerateButton()
        {
            EditorGUILayout.BeginHorizontal();
            _sizeFiled = GUILayout.TextField(_sizeFiled);
            if (int.TryParse(_sizeFiled, out var size))
            {
                _size = size;
            }

            if (GUILayout.Button("Generate"))
            {
                Generate();
                FindPath();
                GeneratePathItems();
                
                // RemoveStartFinishWalls();
                _levelConfig = null;
                _levelName = $"level_{_size}_{_totalPath}_{_cells.GetHashCode():X4}";
            }
            EditorGUILayout.EndHorizontal();
        }
        
        private void RemoveStartFinishWalls()
        {
            for (var r = 0; r < _cells.GetLength(0); r++)
            for (var c = 0; c < _cells.GetLength(1); c++)
            {
                if (_cells[r,c].HasFlag(CellType.Finish))
                {
                    _cells[r,c] &= ~CellType.UpWall;
                }
                else if (_cells[r, c].HasFlag(CellType.Start))
                {
                    _cells[r,c] &= ~CellType.DownWall;
                }
            }
        }

        private void Generate()
        {
            _cells = new CellType[_size * 2, _size];
            var rows = _cells.GetLength(0);
            var cols = _cells.GetLength(1);
            
            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    // if (r > 0)
                    _cells[r, c] |= CellType.UpWall;

                    // if (r < _rows - 1)
                    _cells[r, c] |= CellType.DownWall;

                    // if(c > 0)
                    _cells[r, c] |= CellType.LeftWall;

                    // if(c < _cols - 1)
                    _cells[r, c] |= CellType.RightWall;
                }
            }
            
            ProcessCell(Random.Range(0, rows), Random.Range(0, cols), _cells);

            MakeStartAndFinish(_cells, cols, rows);
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
                
                if (Random.Range(0, 10) < 1)
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