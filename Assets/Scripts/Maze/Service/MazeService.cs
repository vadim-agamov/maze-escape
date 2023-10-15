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
        private IPlayerDataService DataService { get; } = ServiceLocator.Get<IPlayerDataService>();

        private const int InitialLevels = 5;

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
            await SetupLevel(cancellationToken);
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
            
            AnalyticsService.TrackEvent("StartLevel", new Dictionary<string, object>
            {
                {"level", DataService.Data.Level}
            });
        }

        void IService.Dispose()
        {
            foreach (var checker in _components)
            {
                checker.Dispose();
            }
        }

        private async UniTask SetupLevel(CancellationToken cancellationToken)
        {
            var levels = await Addressables.LoadAssetAsync<LevelsConfig>("LevelsConfig").ToUniTask(cancellationToken: cancellationToken);

            if (DataService.Data.Level <= InitialLevels)
            {
                Context.Level = levels.LevelsConfigs[DataService.Data.Level];
            }
            else
            {
                var index = InitialLevels + DataService.Data.Level % (levels.LevelsConfigs.Length - InitialLevels);
                Context.Level = levels.LevelsConfigs[index];
            }
        }
    }
}