using System;
using Cysharp.Threading.Tasks;

namespace Modules.PlatformService.FbPlatformService
{
    public class AdController
    {
        private UniTaskCompletionSource _completionSource;
        private AdStatus _status;
        private string Id { get; }
        private Action ShowAd { get; }
        private Action<string> PreloadAd { get; }

        public AdController(string id, Action<string> preloadAd, Action showAd)
        {
            Id = id;
            PreloadAd = preloadAd;
            ShowAd = showAd;
        }
        
        public void Preload()
        {
            if (_status == AdStatus.Preloading || _status == AdStatus.Preloaded)
            {
                return;
            }

            _status = AdStatus.Preloading;
            PreloadAd(Id);
        }

        public async UniTask<bool> Show()
        {
            if (_status == AdStatus.NotPreloaded)
            {
                Preload();
                return false;
            }

            else if (_status == AdStatus.Preloading)
            {
                return false;
            }

            else if (_status == AdStatus.Preloaded)
            {
                _completionSource?.TrySetCanceled();
                _completionSource = new UniTaskCompletionSource();
                ShowAd();

                var result = false;
                try
                {
                    await _completionSource.Task;
                    result = true;
                }
                catch
                {
                    result = false;
                }

                _status = AdStatus.NotPreloaded;
                Preload();
                return result;
            }

            return false;
        }

        public void OnLoaded()
        {
            _status = AdStatus.Preloaded;
        }

        public void OnNotLoaded()
        {
            _status = AdStatus.NotPreloaded;
        }

        public void OnShown()
        {
            _completionSource.TrySetResult();
            _completionSource = null;
        }
        
        public void OnNotShown(string e)
        {
            _completionSource.TrySetException(new Exception(e));
            _completionSource = null;
        }
    }
}