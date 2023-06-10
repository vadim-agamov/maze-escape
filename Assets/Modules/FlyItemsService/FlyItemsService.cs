using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Modules.ServiceLocator;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Modules.FlyItemsService
{
    public interface IFlyItemsService : IService
    {
        UniTask Fly(string name, string from, string to, int count);
        UniTask Fly(string name, Vector3 from, string to, int count);
        void RegisterAnchor(FlyItemAnchor anchor);
        void UnregisterAnchor(FlyItemAnchor anchor);
    }
    
    public class FlyItemsService: IFlyItemsService 
    {
        private ObjectPool<Image> _pool;
        private readonly Canvas _canvas;
        private FlyItemsConfig _config;
        private List<FlyItemAnchor> _anchors = new List<FlyItemAnchor>();

        public FlyItemsService(Canvas canvas)
        {
            _canvas = canvas;
        }

        private void OnReleaseItem(Image item)
        {
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
            var image = go.AddComponent<Image>();
            image.preserveAspect = true;
            return image;
        }

        public UniTask Fly(string name, Vector3 from, string toId, int count)
        {
            var to = _anchors.First(x => x.Id == toId);
            return Fly(name, from, null, to.transform.position, to.Play, count);
        }

        public UniTask Fly(string name, string fromId, string toId, int count)
        {
            var from = _anchors.First(x => x.Id == fromId);
            var to = _anchors.First(x => x.Id == toId);
            return Fly(name, from.transform.position, from.Play, to.transform.position, to.Play, count);
        }
        
        public UniTask Initialize(IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            _pool = new ObjectPool<Image>(OnCreateItem, OnGetItem, OnReleaseItem);
            _config = Resources.Load<FlyItemsConfig>("FlyItemsConfig");
            return UniTask.CompletedTask;
        }

        public void Dispose()
        {
        }

        void IFlyItemsService.RegisterAnchor(FlyItemAnchor anchor) => _anchors.Add(anchor);

        void IFlyItemsService.UnregisterAnchor(FlyItemAnchor anchor) => _anchors.Remove(anchor);

        private async UniTask Fly(string name, Vector3 from, Action<int> fromAction, Vector3 to, Action<int> toAction, int count)
        {
            var taskCompletionSource = new UniTaskCompletionSource();

            var midPoint = Vector3.Lerp(from, to, 0.5f);
            var xDelta = Screen.width * 0.2f;
            var yDelta = 0;// Screen.height * 0.2f;
            midPoint = new Vector3(midPoint.x + Random.Range(-xDelta, xDelta), midPoint.y + Random.Range(-yDelta, yDelta));
                
            for (var i = 0; i < count; i++)
            {
                var item = _pool.Get();
                item.sprite = _config.GetIcon(name);
                item.transform.position = from;
                item.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

                fromAction?.Invoke(-1);
                var localIndex = i;

                var sequence = DOTween.Sequence();
                sequence
                    .Append(item.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.2f))
                    .Append(item.transform.DOScale(Vector3.one, 0.2f))
                    .Insert(i * 0.1f + 0.4f,item.transform.DOPath(new[] {midPoint, to}, 1f, PathType.CatmullRom)
                        .SetEase(Ease.InCubic)
                        .OnComplete(() =>
                        {
                            if (localIndex == 0)
                            {
                                taskCompletionSource.TrySetResult();
                                taskCompletionSource = null;
                            }

                            _pool.Release(item);

                            toAction?.Invoke(1);
                        }));
            }

            await taskCompletionSource.Task;
        }
    }
}