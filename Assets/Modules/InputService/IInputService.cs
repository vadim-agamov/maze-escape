using Modules.ServiceLocator;
using UnityEngine;

namespace Modules.InputService
{
    public interface IInputService: IService
    {
        Vector2 Touch0 { get; }
        Vector2 Touch1 { get; }
        int TouchesCount { get; }
    }
}