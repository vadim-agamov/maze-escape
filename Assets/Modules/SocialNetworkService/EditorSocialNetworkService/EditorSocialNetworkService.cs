using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;
using UnityEngine;

namespace Modules.SocialNetworkService.EditorSocialNetworkService
{
	public class EditorSocialNetworkService : MonoBehaviour, ISocialNetworkService
	{
		private static string PlayerProgressPath => Path.Combine(Application.persistentDataPath, "player_progress.json");
		
		string ISocialNetworkService.GetUserId() => SystemInfo.deviceUniqueIdentifier;

		UniTask<string> ISocialNetworkService.LoadPlayerProgress()
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

		UniTask ISocialNetworkService.SavePlayerProgress(string playerData)
		{
			File.WriteAllText(PlayerProgressPath, playerData);
			return UniTask.CompletedTask;
		}

		void ISocialNetworkService.PreloadRewardedVideo()
		{
		}

		async UniTask<bool> ISocialNetworkService.ShowRewardedVideo()
		{
			await UniTask.Delay(TimeSpan.FromSeconds(1));
			return true;
		}

		async UniTask<bool> ISocialNetworkService.ShowInterstitial()
		{
			await UniTask.Delay(TimeSpan.FromSeconds(1));
			return true;
		}

		void ISocialNetworkService.PreloadRewardedInterstitial()
		{
		}

		async UniTask<bool> ISocialNetworkService.ShowRewardedInterstitial()
		{
			await UniTask.Delay(TimeSpan.FromSeconds(1));
			return true;
		}

		void ISocialNetworkService.LogEvent(string eventName, Dictionary<string, object> parameters)
		{
		}

		void ISocialNetworkService.PreloadInterstitial()
		{
		}

		async UniTask IService.Initialize(CancellationToken cancellationToken)
		{
			DontDestroyOnLoad(gameObject);
			await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: cancellationToken);
		}

		void IService.Dispose()
		{
		}
	}
}
