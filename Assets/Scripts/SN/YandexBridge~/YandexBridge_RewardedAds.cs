using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using Events;
using Events.CommonEvents;
using Managers.AdsManager.Events;
using UnityEngine;

public partial class YandexBridge : SNBridgeBase
{
    private PendingAds _pendingAds ;
    
    public override async UniTask<PendingAds.State> ShowRewarded(string placement)
    {
        placement = YANDEXConstants.RewardedAdId;
        
        Debug.LogWarning("Show rewarded: " + placement );
        GlobalEventManager.Publish(new ManualLockInputEvent());
        
        _pendingAds = new PendingAds(placement);

        ShowRewardedAd(placement);
        
        await UniTask.WaitUntil(() => _pendingAds.Finished);

        var res = _pendingAds.CurrentState;
Debug.LogWarning("rewarded ads result: " + res);        _pendingAds = null;

        GlobalEventManager.Publish(new ManualUnlockInputEvent());
        
        return res;
    }

    public void OnRewardedOpen(string placement) 
    {
        Debug.LogWarning("OnRewardedOpen: " + placement);

        if (placement == _pendingAds.AdId)
        {
            _pendingAds.CurrentState = PendingAds.State.Showing;
            //GlobalEventManager.Publish(new RewaededAdsOpenedEvent());
        }
        
    }

    public void OnRewarded(string placement)
    {
        Debug.LogWarning("OnRewarded: " + placement);
        if (placement == _pendingAds.AdId)
        {
            _pendingAds.CurrentState = PendingAds.State.CompleteSucceed;
            //GlobalEventManager.Publish(new RewardedAdsCompleteEvent());
        }
    }

    public void OnRewardedClose(string placement) 
    {
        if (placement == _pendingAds.AdId)
        {
            _pendingAds.CurrentState = PendingAds.State.Canceled;
            //GlobalEventManager.Publish(new RewardedAdsClosedEvent());
        }
    }

    public void OnRewardedError(string placement) 
    {
        Debug.LogWarning("OnRewardedError: " + placement);

        if (placement == _pendingAds.AdId)
        {
            _pendingAds.CurrentState = PendingAds.State.Failed;
            //GlobalEventManager.Publish(new RewardedAdsErrorEvent());
        }
    }
    
//dll
    [DllImport("__Internal")]
    private static extern int ShowRewardedAd(string placement);
    
  
}

