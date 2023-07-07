using Maze.Configs;
using UnityEngine;

namespace Maze.MazeService
{
    public class Context
    {
        public Camera Camera;
        public int Score;
        public CellView[,] CellViews;
        public CellType[,] Cells;
        public LevelConfig Level;
        public bool Active;
    }
}