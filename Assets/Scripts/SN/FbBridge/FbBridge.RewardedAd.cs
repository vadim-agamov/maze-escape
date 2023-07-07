using System;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace SN.FbBridge
{
    public partial class FbBridge
    {
        [DllImport("__Internal")]
        private static extern void FbShowRewarded();
        
        [DllImport("__Internal")]
        private static extern void FbPreloadRewarded();
        
        private UniTaskCompletionSource _rewardedAdCompletionSource;
        private bool _rewardedPreloaded;
        private bool _rewardedPreloading;

        void ISnBridge.PreloadRewarded()
        {
            if(_rewardedPreloaded || _rewardedPreloading)
                return;
            
            FbPreloadRewarded();
        }

        async UniTask<PendingAds.State> ISnBridge.ShowRewarded()
        {
            if (!_rewardedPreloaded)
                return PendingAds.State.Failed;

            _rewardedAdCompletionSource?.TrySetCanceled();
            _rewardedAdCompletionSource = new UniTaskCompletionSource();
        
            FbShowRewarded();

            var result = PendingAds.State.CompleteSucceed;
            try
            {
                await _rewardedAdCompletionSource.Task;
            }
            catch
            {
                result = PendingAds.State.Failed;
            }

            _rewardedPreloaded = false;
            ((ISnBridge) this).PreloadRewarded();
            return result;
        }
        
        [UsedImplicitly]
        public void OnRewardedLoaded()
        {
            _rewardedPreloading = false;
            _rewardedPreloaded = true;
        }

        [UsedImplicitly]
        public void OnRewardedNotLoaded()
        {
            _rewardedPreloading = false;
            _rewardedPreloaded = false;
            // ((ISnBridge) this).PreloadRewarded();
        }

        public void OnRewardedShown()
        {
            _rewardedAdCompletionSource.TrySetResult();
            _rewardedAdCompletionSource = null;
        }
        
        public void OnRewardedNotShown(string error)
        {
            _rewardedAdCompletionSource.TrySetException(new Exception(error));
            _rewardedAdCompletionSource = null;
        }
    }
}