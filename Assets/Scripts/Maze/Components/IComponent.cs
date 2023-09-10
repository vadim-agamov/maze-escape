using System;
using Cysharp.Threading.Tasks;
using Maze.MazeService;

namespace Maze.Components
{
    public interface IComponent : IDisposable
    {
        UniTask Initialize(Context context, IMazeService mazeService);
    }
}