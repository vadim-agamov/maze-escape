using Maze.Components;
using Modules.ServiceLocator;

namespace Maze.Service
{
    public interface IMazeService : IService
    {
        T GetMazeComponent<T>() where T : IComponent;
        Context Context { get; }
    }
}