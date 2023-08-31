using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using Events;
using Events.CommonEvents;
using Managers.AdsManager.Events;
using UnityEngine;

//ADS
public partial class VKBridge : SNBridgeBase
{
	private PendingAds _pendingRewardedAds ;

	public override async UniTask<PendingAds.State> ShowRewarded(string placement)
	{
		Debug.LogWarning("Show rewarded: " + placement );

		GlobalEventManager.Publish(new ManualLockInputEvent());

		_pendingRewardedAds = new PendingAds(placement);
		
		ShowRewardedAdsToJs();
		
		await UniTask.WaitUntil(() => _pendingRewardedAds.Finished);

		var res = _pendingRewardedAds.CurrentState;

		_pendingRewardedAds = null;

		GlobalEventManager.Publish(new ManualUnlockInputEvent());

		return res;
	}

	public void OnRewardAdCompleteSuccessFromJs()
	{
		Debug.LogWarning("OnRewardAdCompleteSuccessFromJs: ");

		_pendingRewardedAds.CurrentState = PendingAds.State.CompleteSucceed;
	}
	
	public void OnRewardAdCompleteFailedFromJs()
	{
		_pendingRewardedAds.CurrentState = PendingAds.State.Failed;

	}
	
	

	[DllImport("__Internal")]
	public static extern void ShowRewardedAdsToJs();
}
