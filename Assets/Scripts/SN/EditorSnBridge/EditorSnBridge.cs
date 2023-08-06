using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Services;
using Services.PlayerDataService;
using UnityEngine;

namespace SN.EditorSnBridge
{
	public class EditorSnBridge : MonoBehaviour, ISnBridge
	{
		UniTask ISnBridge.Initialize()
		{

			return UniTask.CompletedTask;
		}

		string ISnBridge.GetUserId()
		{
			return SystemInfo.deviceUniqueIdentifier;
		}

		public static string PlayerProgressPath => System.IO.Path.Combine(Application.persistentDataPath, "player_progress.json");

		UniTask<PlayerData> ISnBridge.LoadPlayerProgress()
		{
			PlayerData playerData;
			if (!File.Exists(PlayerProgressPath))
			{
				playerData = new PlayerData();
				playerData.Reset();
			}
			else
			{
				playerData = JsonConvert.DeserializeObject<PlayerData>(File.ReadAllText(PlayerProgressPath));
				playerData.LastSessionDate = DateTime.Now;
			}

			return UniTask.FromResult(playerData);
		}

		UniTask ISnBridge.SavePlayerProgress(PlayerData playerData)
		{
			File.WriteAllText(PlayerProgressPath, JsonConvert.SerializeObject(playerData));
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

		void ISnBridge.PreloadInterstitial()
		{
		}
	}
}
