using Maze.Components;
using Modules.ServiceLocator;

namespace Maze.MazeService
{
    public interface IMazeService : IService
    {
        T GetComponent<T>() where T : IComponent;
        Context Context { get; }
    }
}