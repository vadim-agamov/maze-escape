using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Maze.Components;
using Maze.Configs;
using Modules.ServiceLocator;
using Services.PlayerDataService;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Maze.MazeService
{
    public class MazeService: IMazeService
    {
        private readonly List<IComponent> _components = new List<IComponent>();

        private readonly Context _context = new Context();

        public T GetComponent<T>() where T : IComponent
        {
            foreach (var component in _components)
            {
                if (component is T checker)
                    return checker;
            }

            throw new Exception($"no component {typeof(T)}");
        }

        public Context Context => _context;

        async UniTask IService.Initialize(IProgress<float> progress, CancellationToken cancellationToken)
        {
            var playerDataService = ServiceLocator.Get<IPlayerDataService>();
            var levels = await Addressables.LoadAssetAsync<LevelsConfig>("LevelsConfig");
            
            Context.Level = levels.LevelsConfigs[playerDataService.Data.Level % levels.LevelsConfigs.Length];
            
            _components.Add(GameObject.Find("Field").GetComponent<FieldViewComponent>());
            _components.Add(GameObject.Find("Camera").GetComponent<CameraSizeFitterComponent>());
            _components.Add(GameObject.Find("Path").GetComponent<PathComponent>());
            _components.Add(GameObject.Find("Crab").GetComponent<CharacterComponent>());
            // _components.Add(GameObject.Find("HUD").GetComponent<MazeHUD>());
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