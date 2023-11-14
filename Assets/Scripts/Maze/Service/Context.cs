using System.Collections.Generic;
using Maze.Components;
using Maze.Configs;
using UnityEngine;

namespace Maze.Service
{
    public class Context
    {
        public Camera Camera;
        public CellView[,] CellViews;
        public CellType[,] Cells;
        public readonly LinkedList<CellView> Path = new();
        public PathGoal[] Goals;
        public LevelConfig Level;
        public bool Active;
        public int CurrentGoal;

        // character
        public CellView CurrentCell;      // destination cell
        public IPositionProvider CharacterPositionProvider;
        // public bool IsWalking;
        public bool GoalReached;
    }
}