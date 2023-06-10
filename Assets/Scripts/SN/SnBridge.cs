using Cysharp.Threading.Tasks;
using Services;
using UnityEngine;

namespace SN
{
	
#if UNITY_WEBGL
	public interface ISnBridge
	{
		UniTask Initialize();
		string GetUserId();
		UniTask<PlayerData> LoadPlayerProgress();
		UniTask SavePlayerProgress(PlayerData pp);

		void PreloadRewarded();
		UniTask<PendingAds.State> ShowRewarded();

		void PreloadInterstitial();
		UniTask ShowInterstitial();
	}

	public abstract class SnBridge : MonoBehaviour
	{
		private static ISnBridge _instance;

		public static async UniTask Initialize()
		{
			var go = new GameObject("SNBridge");
			DontDestroyOnLoad(go);
#if UNITY_EDITOR
			go.name = "EditorSN_FakeBridge";
			_instance = go.AddComponent<EditorSnBridge.EditorSnBridge>();
#elif FB
			go.name = "FbBridge";
			_instance = go.AddComponent<FbBridge.FbBridge>();
#endif
			await _instance.Initialize();
		}

		public static ISnBridge Instance => _instance;
	}
#endif

	public class PendingAds
	{
		// public readonly string AdId;
		// public State CurrentState;
		//
		// public PendingAds(string adId)
		// {
		// 	AdId = adId;
		// 	CurrentState = State.NotStarted;
		// }

		public enum State
		{
			NotStarted,
			Showing,
			Canceled,
			Failed,
			CompleteSucceed
		}
	}
}