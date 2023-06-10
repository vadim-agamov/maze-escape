using System.Collections.Generic;
using System.Runtime.InteropServices;
using CommandProcessor.Rewards;
using Cysharp.Threading.Tasks;
using Events;
using Managers.AdsManager.Events;
using SN;
using UnityEngine;

public partial class YandexBridge : SNBridgeBase
{
    private bool isShowingInterstitial ;

    public override async UniTask ShowInterstitial()
    {
        isShowingInterstitial = true;
        
        ShowFullscreenAd();

        await UniTask.WaitWhile(() => isShowingInterstitial);
    }
    

// Callbacks from index.html
    public void OnInterstitialShown()
    {
        Debug.LogWarning("Unity OnInterstitialShown");
     
        isShowingInterstitial = false;

        GlobalEventManager.Publish(new IntestitialAdsCompleteEvent());
    }
    public void OnInterstitialError(string error) 
    {
        Debug.LogWarning("Unity OnInterstitialError: " + error);

        isShowingInterstitial = false;

        GlobalEventManager.Publish(new IntestitialAdsErrorEvent());
    }

 
//dll
    [DllImport("__Internal")]
    private static extern void ShowFullscreenAd();
}

