using System.Collections.Generic;
using Configs;
using Maze.Components;
using Modules.ServiceLocator;
using UnityEngine;

namespace Maze.MazeService
{
    public class Context
    {
        public Camera Camera;
        public int Score;
        public LevelConfig LevelConfig;
        public IEnumerable<CellView> Path;
    }

    public interface IMazeService : IService
    {
        T GetComponent<T>() where T : IComponent;
        Context Context { get; }
    }
}