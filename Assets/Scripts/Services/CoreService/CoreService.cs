using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Items;
using Modules.ServiceLocator;
using UnityEngine;

namespace Services.CoreService
{
    public class CoreService: ICoreService
    {
        private readonly List<ICoreComponent> _components = new List<ICoreComponent>();

        private readonly CoreContext _context = new CoreContext();

        public T GetComponent<T>() where T : ICoreComponent
        {
            foreach (var levelChecker in _components)
            {
                if (levelChecker is T checker)
                    return checker;
            }

            throw new Exception($"no level checker {typeof(T)}");
        }

        public CoreContext Context => _context;

        async UniTask IService.Initialize(IProgress<float> progress, CancellationToken cancellationToken)
        {
            // var itemsFactory = new ItemsFactoryComponent();
            // _context.ItemsFactory = itemsFactory;
            // _components.Add(itemsFactory);
            // _components.Add(GameObject.Find("LooseLevelChecker").GetComponent<LooseLevelCheckerComponent>());
            // _components.Add(GameObject.Find("SpawnPoint").GetComponent<SpawnComponent>());
            // _components.Add(new GameObject("ItemsCollidedChecker").AddComponent<ItemsCollidedChecker>());
            // _components.Add(new ScoreTrackerComponent());
            // _components.Add(new WinLevelCheckerComponent());
            // _components.Add( new GameObject("ItemsCollidedSeriesCheckerComponent").AddComponent<ItemsCollidedSeriesCheckerComponent>());
            //
            // foreach (var component in _components)
            // {
            //     await component.Initialize(_context);
            // }
            //
            // _context.CanSpawn = true;
        }

        void IService.Dispose()
        {
            // foreach (var checker in _components)
            // {
            //     checker.Dispose();
            // }
        }
    }
}