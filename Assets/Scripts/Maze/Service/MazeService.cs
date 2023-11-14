using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Maze.Components;
using Maze.Configs;
using Modules.AnalyticsService;
using Modules.LocalizationService;
using Modules.ServiceLocator;
using Services.GamePlayerDataService;
using UnityEngine;
using UnityEngine.AddressableAssets;
using CharacterController = Maze.Components.CharacterController;

namespace Maze.Service
{
    public class MazeService: MonoBehaviour, IMazeService
    {
        private readonly List<IComponent> _components = new List<IComponent>();
        private readonly Context _context = new Context();
        private LocalizationProviderConfig _localizationProvider;
        private bool _initialized;
        private IAnalyticsService AnalyticsService { get; } = ServiceLocator.Get<IAnalyticsService>();
        private GamePlayerDataService DataService { get; } = ServiceLocator.Get<GamePlayerDataService>();

        private const int InitialLevels = 5;

        public T GetMazeComponent<T>() where T : IComponent
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
            _components.Add(GameObject.Find("Crab").GetComponent<CharacterController>());
            _components.Add(new GoalsCheckerComponent());
            _components.Add(new InputComponent());
            // _components.Add(new GameObject($"{nameof(AttentionFxComponent)}").AddComponent<AttentionFxComponent>());

            foreach (var component in _components)
            {
                await component.Initialize(_context, this);
            } 
            
            AnalyticsService.TrackEvent("StartLevel", new Dictionary<string, object>
            {
                {"level", DataService.PlayerData.Level}
            });

            _initialized = true;
        }

        private void Update()
        {
            if (!_initialized)
            {
                return;
            }

            foreach (var component in _components)
            {
                component.Update();
            }
        }

        void IService.Dispose()
        {
            _initialized = false;
            foreach (var checker in _components)
            {
                checker.Dispose();
            }
        }
        
        private async UniTask SetupLevel(CancellationToken cancellationToken)
        {
            var levels = await Addressables.LoadAssetAsync<LevelsConfig>("LevelsConfig").ToUniTask(cancellationToken: cancellationToken);

            if (DataService.PlayerData.Level <= InitialLevels)
            {
                Context.Level = levels.LevelsConfigs[DataService.PlayerData.Level];
            }
            else
            {
                var index = InitialLevels + DataService.PlayerData.Level % (levels.LevelsConfigs.Length - InitialLevels);
                Context.Level = levels.LevelsConfigs[index];
            }
        }
    }
}