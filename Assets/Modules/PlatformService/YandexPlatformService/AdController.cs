using System;
using Cysharp.Threading.Tasks;

namespace Modules.PlatformService.YandexPlatformService
{
    public class AdController
    {
        private UniTaskCompletionSource _completionSource;
        private Action ShowAd { get; }

        public AdController(Action showAd)
        {
            ShowAd = showAd;
        }

        public async UniTask<bool> Show()
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
            
            return result;
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