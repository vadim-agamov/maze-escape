using Maze.Configs;
using UnityEngine;

namespace Maze.Service
{
    public class Context
    {
        public Camera Camera;
        public CellView[,] CellViews;
        public CellType[,] Cells;
        public LevelConfig Level;
        public bool Active;
    }
}