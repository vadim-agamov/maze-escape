using System;
using Config;
using Cysharp.Threading.Tasks;
using Items;
using Modules.ServiceLocator;
using Services.CoreService;
using UnityEngine;

namespace Actions
{
    public class UseRocketTrickAction
    {
        private readonly ICoreService _coreService;
        private ItemsConfig _itemsConfig;

        public UseRocketTrickAction()
        {
            _coreService = ServiceLocator.Get<ICoreService>();
        }

        public async UniTask<bool> Execute()
        {
            if (!_coreService.GetComponent<SpawnComponent>().HasItem)
                return false;

            _coreService.Context.CanSpawn = false;
            while (true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    var rocketPrefab = Resources.Load<RocketTrick>("RocketTrick");
                    var rocket = GameObject.Instantiate(rocketPrefab);

                    var hit = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    rocket.OnCollide += OnHitItem;
                    await rocket.Launch(hit);
                    break;
                }

                await UniTask.Yield();
            }

            _coreService.Context.CanSpawn = true;
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

            return true;
        }

        private void OnHitItem(Item item) => DestroyItem(item).Forget();
        
        private async UniTask DestroyItem(Item item)
        {
            _coreService.Context.Items.Remove(item);
            await item.PlayExplode(); 
            _coreService.Context.ItemsFactory.ReleaseItem(item);
        }
    }
}