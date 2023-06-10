using System;
using System.Collections.Generic;
using Config;
using Cysharp.Threading.Tasks;
using Items;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Services.CoreService
{
    public interface IItemsFactoryComponent
    {
        Item GetItem(string id);
        void ReleaseItem(Item item);
    }
    
    public class ItemsFactoryComponent : IItemsFactoryComponent, ICoreComponent
    {
        private GameObject _itemsContainer;
        private ItemsConfig _itemsConfig;
        private readonly Dictionary<string, ObjectPool<Item>> _pool = new Dictionary<string, ObjectPool<Item>>();
        private CoreContext _context;

        Item IItemsFactoryComponent.GetItem(string id)
        {
            var item = _pool[id].Get();
            return item;
        }

        void IItemsFactoryComponent.ReleaseItem(Item item)
        {
            _pool[item.Config.Id].Release(item);
        }

        private void DestroyItem(ItemConfig itemConfig, Item item)
        {
            Object.Destroy(item.gameObject);
        }

        private void ReleaseItem(ItemConfig itemConfig, Item item)
        {
            item.gameObject.SetActive(false);
            item.transform.rotation = Quaternion.identity;
            item.transform.position = Vector3.zero;
        }
        
        private void GetItem(ItemConfig itemConfig, Item item)
        {
            item.SetId(Item.NextId);
            item.gameObject.name = item.Id + "_" + itemConfig.Id;
            item.transform.localScale = Vector3.zero;
            item.Renderer.color = Color.white;
            item.Rigidbody.simulated = true;
            item.Rigidbody.gravityScale = 1;
            item.gameObject.layer = LayerMask.NameToLayer("Field");
            item.gameObject.SetActive(true);
        }

        private Item CreateItem(ItemConfig config)
        {
            var item = Object.Instantiate(config.Prefab, _itemsContainer.transform, true);
            item.Rigidbody.mass = config.Mass;
            return item;
        }


        async UniTask ICoreComponent.Initialize(CoreContext context)
        {
            _context = context;
            _context.Items = new LinkedList<Item>();
            _itemsContainer = new GameObject("[Items]");
            _itemsConfig = await Addressables.LoadAssetAsync<ItemsConfig>("ItemsConfig");

            foreach (var itemConfig in _itemsConfig.Items)
            {
                _pool.Add(itemConfig.Id, new ObjectPool<Item>(
                    () => CreateItem(itemConfig),
                    item => GetItem(itemConfig, item),
                    item => ReleaseItem(itemConfig, item),
                    item => DestroyItem(itemConfig, item)));

                var tmp = new List<Item>();
                for (var i = 0; i < 3; i++)
                    tmp.Add(_pool[itemConfig.Id].Get());

                foreach (var item in tmp)
                    _pool[itemConfig.Id].Release(item);
            }
        }

        void IDisposable.Dispose()
        {
            foreach (var (_, val) in _pool)
                val.Clear();

            _pool.Clear();
        }
    }
}