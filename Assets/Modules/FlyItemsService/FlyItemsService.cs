using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Modules.ServiceLocator;
using Modules.UIService;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Modules.FlyItemsService
{
    public interface IFlyItemsService : IService
    {
        UniTask Fly(string name, string from, string to, int count);
        UniTask Fly(string name, Vector3 from, string to, int count);
        UniTask Fly(IReadOnlyList<(string Id, int Count)> names, Vector3 from, string to);
        void RegisterAnchor(FlyItemAnchor anchor);
        void UnregisterAnchor(FlyItemAnchor anchor);
    }
    
    public class FlyItemsService: IFlyItemsService 
    {
        private ObjectPool<Image> _pool;
        private Canvas _canvas;
        private FlyItemsConfig _config;
        private readonly List<FlyItemAnchor> _anchors = new();
        
        private void OnReleaseItem(Image item)
        {
            item.transform.position = Vector3.zero;
            item.gameObject.SetActive(false); 
        }

        private void OnGetItem(Image item)
        {
            item.gameObject.SetActive(true); 
        }

        private Image OnCreateItem()
        {
            var go = new GameObject();
            go.transform.SetParent(_canvas.transform);
            go.transform.localScale = Vector3.one;
            go.transform.transform.position = Vector3.zero;
            go.name = "FlyItem";
            var image = go.AddComponent<Image>();
            image.preserveAspect = true;
            return image;
        }

        public UniTask Fly(string name, Vector3 from, string toId, int count)
        {
            var to = _anchors.First(x => x.Id == toId);
            var names = Enumerable.Repeat(name, count).ToList();
            return Fly(names, from, null, to.transform.position, to.Play);
        }

        public UniTask Fly(string name, string fromId, string toId, int count)
        {
            var from = _anchors.First(x => x.Id == fromId);
            var to = _anchors.First(x => x.Id == toId);
            var names = Enumerable.Repeat(name, count).ToList();
            return Fly(names, from.transform.position, from.Play, to.transform.position, to.Play);
        }

        public UniTask Fly(IReadOnlyList<(string Id, int Count)> names, Vector3 from, string toId)
        {
            var to = _anchors.First(x => x.Id == toId);
            var namesList = names.SelectMany(x => Enumerable.Repeat(x.Id, x.Count)).ToList();
            return Fly(namesList, from, null, to.transform.position, to.Play);
        }

        async UniTask IService.Initialize(CancellationToken cancellationToken)
        {
            _canvas = ServiceLocator.ServiceLocator.Get<IUIService>().Canvas;
            _pool = new ObjectPool<Image>(OnCreateItem, OnGetItem, OnReleaseItem);
            _config = await Addressables.LoadAssetAsync<FlyItemsConfig>("FlyItemsConfig");
        }
        
        void IService.Dispose()
        {
        }

        void IFlyItemsService.RegisterAnchor(FlyItemAnchor anchor) => _anchors.Add(anchor);

        void IFlyItemsService.UnregisterAnchor(FlyItemAnchor anchor) => _anchors.Remove(anchor);

        private async UniTask Fly(IReadOnlyList<string> names, Vector3 from, Action<string,int> fromAction, Vector3 to, Action<string,int> toAction)
        {
            var taskCompletionSource = new UniTaskCompletionSource();
            from = new Vector3(from.x, from.y, _canvas.transform.position.z); 

            var distance = Vector3.Distance(from, to);
            
            var midPoint = Vector3.Lerp(from, to, 0.2f);
            var xDelta = distance * 0.2f;
            var yDelta = distance * 0.2f;
            midPoint = new Vector3(midPoint.x + Random.Range(-xDelta, xDelta), midPoint.y + Random.Range(-yDelta, yDelta), midPoint.z);
                
            var sequence = DOTween.Sequence();
            sequence.timeScale = 1.2f;
            sequence.Pause();
            for (var i = 0; i < names.Count; i++)
            {
                var id = names[i];
                var item = _pool.Get();
                item.sprite = _config.GetIcon(id);
                item.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                item.transform.position = from;

                fromAction?.Invoke(id, -1);
                var localIndex = i;

                sequence
                    .Insert(i * 0.1f, item.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.2f))
                    .Insert(i * 0.1f + 0.2f, item.transform.DOScale(Vector3.one, 0.2f))
                    .Insert(i * 0.5f + 0.4f,item.transform.DOPath(new[] {midPoint, to}, distance * 0.05f, PathType.CatmullRom)
                        .SetEase(Ease.InCubic)
                        .OnComplete(() =>
                        {
                            if (localIndex == 0)
                            {
                                taskCompletionSource.TrySetResult();
                                taskCompletionSource = null;
                            }

                            _pool.Release(item);

                            toAction?.Invoke(id, 1);
                        }));
            }
            sequence.Play();

            await taskCompletionSource.Task;
        }
    }
}