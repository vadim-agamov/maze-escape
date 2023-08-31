using System;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace Modules.SnBridge.FbBridge
{
    public partial class FbBridge
    {
        [DllImport("__Internal")]
        private static extern void FbShowInterstitial();
        
        [DllImport("__Internal")]
        private static extern void FbPreloadInterstitial();
        
        private UniTaskCompletionSource _adsCompletionSource;
        private bool _interstitialPreloaded;
        private bool _interstitialPreloading;

        void ISnBridge.PreloadInterstitial()
        {
            if(_interstitialPreloaded || _interstitialPreloading)
                return;

            _interstitialPreloading = true;
            FbPreloadInterstitial();
        }

        async UniTask ISnBridge.ShowInterstitial()
        {
            if(!_interstitialPreloaded)
                return;
            
            _adsCompletionSource?.TrySetCanceled();
            _adsCompletionSource = new UniTaskCompletionSource();
            FbShowInterstitial();
        
            try
            {
                await _adsCompletionSource.Task;
            }
            catch (OperationCanceledException _)
            {
            }

            _interstitialPreloaded = false;
            This.PreloadInterstitial();
        }

        [UsedImplicitly]
        public void OnInterstitialLoaded()
        {
            _interstitialPreloading = false;
            _interstitialPreloaded = true;
        }

        [UsedImplicitly]
        public void OnInterstitialNotLoaded()
        {
            _interstitialPreloading = false;
            _interstitialPreloaded = false;
            // ((ISnBridge) this).PreloadInterstitial();
        }

        public void OnInterstitialShown()
        {
            _adsCompletionSource.TrySetResult();
            _adsCompletionSource = null;
        }
    }
}