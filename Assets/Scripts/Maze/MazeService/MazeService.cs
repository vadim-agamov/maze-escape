using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Maze.Components;
using Modules.ServiceLocator;
using UnityEngine;

namespace Maze.MazeService
{
    public class MazeService: IMazeService
    {
        private readonly List<IComponent> _components = new List<IComponent>();

        private readonly Context _context = new Context();

        public T GetComponent<T>() where T : IComponent
        {
            foreach (var levelChecker in _components)
            {
                if (levelChecker is T checker)
                    return checker;
            }

            throw new Exception($"no component {typeof(T)}");
        }

        public Context Context => _context;

        async UniTask IService.Initialize(IProgress<float> progress, CancellationToken cancellationToken)
        {
            _components.Add(GameObject.Find("Field").GetComponent<FieldViewComponent>());
            _components.Add(GameObject.Find("Camera").GetComponent<CameraSizeFitterComponent>());
            _components.Add(GameObject.Find("Path").GetComponent<PathComponent>());
            _components.Add(new WinLevelCheckerComponent());

            foreach (var component in _components)
            {
                await component.Initialize(_context);
            }
        }

        void IService.Dispose()
        {
            foreach (var checker in _components)
            {
                checker.Dispose();
            }
        }
    }
}