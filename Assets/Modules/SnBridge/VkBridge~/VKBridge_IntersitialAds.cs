using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using Events;
using Managers.AdsManager.Events;
using UnityEngine;

//ADS VKWebAppShowNativeAds 
public partial class VKBridge : SNBridgeBase
{
	private bool _pendingInterAds ;

	public override async UniTask ShowInterstitial()
	{
		Debug.LogWarning("Show inter: " );

		_pendingInterAds = true;
		
		ShowInterAdsToJs();
		
		await UniTask.WaitWhile(() => _pendingInterAds);

	}

	public void OnInterAdCompleteSuccessFromJs()
	{
		Debug.LogWarning("OnInterAdCompleteSuccessFromJs: ");

		_pendingInterAds = false;
	}
	
	public void OnInterAdCompleteFailedFromJs()
	{
		Debug.LogWarning("OnInterAdCompleteFailedFromJs: ");

		_pendingInterAds = false;

	}
	
	

	[DllImport("__Internal")]
	public static extern void ShowInterAdsToJs();
}
