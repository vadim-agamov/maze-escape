using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Maze.Components;
using Maze.Configs;
using Modules.AnalyticsService;
using Modules.ServiceLocator;
using Services.PlayerDataService;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Maze.Service
{
    public class MazeService: IMazeService
    {
        private readonly List<IComponent> _components = new List<IComponent>();
        private readonly Context _context = new Context();
        private IAnalyticsService AnalyticsService { get; } = ServiceLocator.Get<IAnalyticsService>();

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

        async UniTask IService.Initialize(CancellationToken cancellationToken)
        {
            Debug.Log($"[{nameof(MazeService)}] Initialize begin");

            var playerDataService = ServiceLocator.Get<IPlayerDataService>();
            var levels = await Addressables.LoadAssetAsync<LevelsConfig>("LevelsConfig");
            Context.Level = levels.LevelsConfigs[playerDataService.Data.Level % levels.LevelsConfigs.Length];
            Context.Camera = GameObject.Find("Camera").GetComponent<Camera>();
            
            _components.Add(GameObject.Find("Field").GetComponent<FieldViewComponent>());
            _components.Add(GameObject.Find("Path").GetComponent<PathComponent>());
            _components.Add(GameObject.Find("Crab").GetComponent<CharacterComponent>());
            _components.Add(new WinLevelCheckerComponent());
            _components.Add(new GameObject($"{nameof(AttentionFxComponent)}").AddComponent<AttentionFxComponent>());

            foreach (var component in _components)
            {
                await component.Initialize(_context, this);
            }
            
            Debug.Log($"[{nameof(MazeService)}] Initialize end");
            AnalyticsService.TrackEvent($"StartLevel_{playerDataService.Data.Level}");
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