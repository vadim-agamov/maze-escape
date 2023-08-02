using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Maze.Configs.Editor
{
    public partial class LevelEditor
    {
        private int _totalPath;
        private int _foundMinPath;

        private void GenerateMazeAndFindPath(int minPath)
        {
            _foundMinPath = 0;
            
            for (var i = 0; i < 500; i++)
            {
                Generate();
                var p = FindPath();
                var success = p >= _minPath && p < _minPath + minPath * 0.1f;
                Debug.Log($"Found path {p} {success}");
                if (success)
                {
                    Debug.Log($"SUCCESS: Found path {p}");
                    _foundMinPath = p;
                    return;
                }
            }

            Debug.LogError($"FAIL: Unable to find path less than {minPath}");
        }

        private int FindPath()
        {
            for (var row = 0; row < _cells.GetLength(0); row++)
            {
                for (var col = 0; col < _cells.GetLength(1); col++)
                {
                    _cells[row, col] &= ~CellType.Path;
                    _cells[row, col] &= ~CellType.Visited;
                }
            }

            var (r, c) = GetStart();
            var paths = new List<List<(int Row, int Col)>>();

            FindPath(r, c, _cells, new Stack<(int Row, int Col)>(), paths);
            _totalPath = paths.Count;
            Debug.Log($"found {paths.Count} paths");

            paths.Sort((a, b) => a.Count - b.Count);
            
            var filteredPath = new List<List<(int Row, int Col)>>();
            filteredPath.Add(paths.First()); 
            filteredPath.Add(paths[paths.Count / 2]); 
            filteredPath.Add(paths.Last()); 
            
            // Debug.Log($"path {filteredPath.First().Count} -- {filteredPath.Last().Count}");

            var pathIndex = 0;
            foreach (var path in filteredPath)
            {
                Debug.Log($"path {pathIndex}, count {path.Count}");

                CellType pathFlag = CellType.Path;
                if (pathIndex == 0) pathFlag = CellType.Path0;
                if (pathIndex == 1) pathFlag = CellType.Path1;
                if (pathIndex == 2) pathFlag = CellType.Path2;
                foreach (var (row, col) in path)
                {
                    _cells[row, col] |= pathFlag;
                }

                pathIndex++;
            }

            return filteredPath.First().Count;
        }

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
        
        private void FindPath(int r, int c, CellType[,] cells, Stack<(int Row,int Col)> currentPath, List<List<(int Row,int Col)>> paths)
        {
            if (cells[r, c].HasFlag(CellType.Finish))
            {
                currentPath.Push((Row: r, Col: c));
                paths.Add(currentPath.ToList());
            }

            if (paths.Count >= 10000)
            {
                Debug.LogError($"FindPath too much path {paths.Count}");
                return;
            }
            
            currentPath.Push((Row: r, Col: c));
            cells[r, c] |= CellType.Visited;
            // while (true)
            // {
            //     var neighbours = GetUnvisitedReachableNeighbours(r, c, cells);
            //     if(neighbours.Count == 0)
            //         break;
            //
            //     var (neighbourRow, neighbourCol,_ , _) = neighbours.First();
            //     FindPath(neighbourRow, neighbourCol, cells, currentPath, paths);
            // }
            
            // var neighbours = GetUnvisitedReachableNeighbours(r, c, cells);
            // foreach (var (neighbourRow, neighbourCol, _, _) in neighbours)
            // {
                // FindPath(neighbourRow, neighbourCol, cells, currentPath, paths);
                // if (res)
                // {
                    // cells[r, c] |= CellType.Path;
                    // currentPath.Pop();
                    // return true;
                // }
            // }
            
            var cell = cells[r, c];
            if (!cell.HasFlag(CellType.UpWall) && r > 0 && !cells[r - 1, c].HasFlag(CellType.Visited))
            {
                // neighbours.Add((NeigbourRow: r-1, NeigbourCol: c, SelfWall: CellType.UpWall, NeigbourWall: CellType.DownWall));
                FindPath(r - 1, c, cells, currentPath, paths);
            }
            
            if (!cell.HasFlag(CellType.DownWall) && r < cells.GetLength(0) - 1 && !cells[r + 1, c].HasFlag(CellType.Visited))
            {
                // neighbours.Add((NeigbourRow: r+1, NeigbourCol: c, SelfWall: CellType.DownWall, NeigbourWall: CellType.UpWall));
                FindPath(r + 1, c, cells, currentPath, paths);
            }
            
            if (!cell.HasFlag(CellType.LeftWall) && c > 0 && !cells[r, c-1].HasFlag(CellType.Visited))
            {
                // neighbours.Add((NeigbourRow: r, NeigbourCol: c-1, SelfWall: CellType.LeftWall, NeigbourWall: CellType.RightWall));
                FindPath(r, c - 1, cells, currentPath, paths);
            }
            
            if (!cell.HasFlag(CellType.RightWall) && c < cells.GetLength(1) - 1 && !cells[r, c+1].HasFlag(CellType.Visited))
            {
                // neighbours.Add((NeigbourRow: r, NeigbourCol: c+1, SelfWall: CellType.RightWall, NeigbourWall: CellType.LeftWall));
                FindPath(r, c+1, cells, currentPath, paths);
            }

            currentPath.Pop();
            cells[r, c] &= ~CellType.Visited;
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