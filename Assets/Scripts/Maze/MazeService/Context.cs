using System.Collections.Generic;
using Maze.Configs;
using UnityEngine;

namespace Maze.MazeService
{
    public class Context
    {
        public Camera Camera;
        public int Score;
        public CellType[,] Cells;
        public LevelConfig Level;
        public bool Active;
    }
}