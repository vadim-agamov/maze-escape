using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Modules.SnBridge
{
	public interface ISnBridge
	{
		void Initialize();
		string GetUserId();
		UniTask<string> LoadPlayerProgress();
		UniTask SavePlayerProgress(string playerProgress);
		
		void PreloadRewarded();
		UniTask<PendingAds.State> ShowRewarded();

		void PreloadInterstitial();
		UniTask ShowInterstitial();
		void LogEvent(string eventName, Dictionary<string, object> parameters);
	}

	public abstract class SnBridge : MonoBehaviour
	{
		private static ISnBridge _instance;

		public static void Initialize()
		{
			var go = new GameObject("SNBridge");
			DontDestroyOnLoad(go);
#if UNITY_EDITOR
			go.name = "EditorSN";
			_instance = go.AddComponent<EditorSnBridge.EditorSnBridge>();
#elif FB
			go.name = "FbBridge";
			_instance = go.AddComponent<FbBridge.FbBridge>();
#endif
			_instance.Initialize();
		}

		public static ISnBridge Instance => _instance;
	}

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