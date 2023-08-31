using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Modules.SnBridge.EditorSnBridge
{
	public class EditorSnBridge : MonoBehaviour, ISnBridge
	{
		private static string PlayerProgressPath => Path.Combine(Application.persistentDataPath, "player_progress.json");

		void ISnBridge.Initialize()
		{
		}

		string ISnBridge.GetUserId() => SystemInfo.deviceUniqueIdentifier;

		UniTask<string> ISnBridge.LoadPlayerProgress()
		{
			string playerData;
			if (!File.Exists(PlayerProgressPath))
			{
				playerData = string.Empty;
			}
			else
			{
				playerData = File.ReadAllText(PlayerProgressPath);
			}

			return UniTask.FromResult(playerData);
		}

		UniTask ISnBridge.SavePlayerProgress(string playerData)
		{
			File.WriteAllText(PlayerProgressPath, playerData);
			return UniTask.CompletedTask;
		}

		void ISnBridge.PreloadRewarded()
		{
		}

		async UniTask<PendingAds.State> ISnBridge.ShowRewarded()
		{
			await UniTask.Delay(TimeSpan.FromSeconds(1));
			return PendingAds.State.CompleteSucceed;
		}

		async UniTask ISnBridge.ShowInterstitial()
		{
			await UniTask.Delay(TimeSpan.FromSeconds(1));
		}

		void ISnBridge.LogEvent(string eventName, Dictionary<string, object> parameters)
		{
		}

		void ISnBridge.PreloadInterstitial()
		{
		}
	}
}
