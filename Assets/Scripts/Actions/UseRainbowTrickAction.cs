using System;
using System.Linq;
using Config;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Items;
using Modules.ServiceLocator;
using Services.CoreService;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Actions
{
    public class UseRainbowTrickAction
    {
        private readonly ICoreService _coreService;
        private ItemsConfig _itemsConfig;

        public UseRainbowTrickAction()
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
                            await Apply(item);
                            trickUsed = true;
                        }
                    }

                    await UniTask.Yield();
                    break;
                }

                await UniTask.Yield();
            }

            _coreService.Context.CanSpawn = true;
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

            return trickUsed;
        }

        private async UniTask Apply(Item item)
        {
            var items = _coreService.Context.Items.ToArray().Where(x => x.Config.Id == item.Config.Id);
            var path = items.Where(x=> x.Id != item.Id ).Select(x => x.transform.position).ToArray();
            var fxPrefab = Resources.Load<GameObject>("BeamFx");
            var fx = GameObject.Instantiate(fxPrefab);

            fx.transform.position = item.transform.position;
            await fx.transform.DOPath(path, 0.2f * path.Length).SetEase(Ease.Linear);
            // await UniTask.Delay(TimeSpan.FromSeconds(2f));
            Object.Destroy(fx);

            var list = Enumerable.Select(items, DestroyItem);
            await list;
        }

        private async UniTask DestroyItem(Item item)
        {
            _coreService.Context.Items.Remove(item);
            await item.PlayExplode(); 
            _coreService.Context.ItemsFactory.ReleaseItem(item);
        }
    }
}