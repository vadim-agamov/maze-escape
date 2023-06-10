using System;
using Cysharp.Threading.Tasks;
using Items;
using Modules.ServiceLocator;
using Services.CoreService;
using UnityEngine;

namespace Actions
{
    public class UseHammerTrickAction
    {
        private readonly ICoreService _coreService;

        public UseHammerTrickAction()
        {
            _coreService = ServiceLocator.Get<ICoreService>();
        }

        public async UniTask<bool> Execute()
        {
            if (!_coreService.GetComponent<SpawnComponent>().HasItem)
                return false;

            var trickUsed = false;
            _coreService.Context.CanSpawn = false;
            while (true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Field"))
                    {
                        var item = hit.transform.GetComponent<Item>();
                        if (item != null)
                        {
                            DestroyItem(item).Forget();
                            trickUsed = true;
                        }
                    }

                    await UniTask.Yield();
                    break;
                }

                await UniTask.Yield();
            }

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            _coreService.Context.CanSpawn = true;

            return trickUsed;
        }
        
        private async UniTask DestroyItem(Item item)
        {
            _coreService.Context.Items.Remove(item);
            await item.PlayExplode(); 
            _coreService.Context.ItemsFactory.ReleaseItem(item);
        }
    }
}