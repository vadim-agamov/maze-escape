using System;
using Cysharp.Threading.Tasks;
using Maze.Service;

namespace Maze.Components
{
    public interface IComponent : IDisposable
    {
        UniTask Initialize(Context context, IMazeService mazeService);
        void Tick();
    }
}