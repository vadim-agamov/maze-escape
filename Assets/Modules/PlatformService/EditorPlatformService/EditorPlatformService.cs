using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;
using UnityEngine;

namespace Modules.PlatformService.EditorPlatformService
{
	public class EditorPlatformService : MonoBehaviour, IPlatformService
	{
		private static string PlayerProgressPath => Path.Combine(Application.persistentDataPath, "player_progress.json");

		Language IPlatformService.GetLocale() => Language.English;

		string IPlatformService.GetUserId() => SystemInfo.deviceUniqueIdentifier;

		UniTask<string> IPlatformService.LoadPlayerProgress()
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

		UniTask IPlatformService.SavePlayerProgress(string playerData, CancellationToken token)
		{
			File.WriteAllText(PlayerProgressPath, playerData);
			return UniTask.CompletedTask;
		}

		void IPlatformService.PreloadRewardedVideo()
		{
		}

		async UniTask<bool> IPlatformService.ShowRewardedVideo(CancellationToken token)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(1));
			return true;
		}

		async UniTask<bool> IPlatformService.ShowInterstitial(CancellationToken token)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(1));
			return true;
		}

		void IPlatformService.PreloadRewardedInterstitial()
		{
		}

		async UniTask<bool> IPlatformService.ShowRewardedInterstitial(CancellationToken token)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(1));
			return true;
		}

		void IPlatformService.LogEvent(string eventName, Dictionary<string, object> parameters)
		{
		}

		void IPlatformService.GameReady()
		{
		}

		void IPlatformService.PreloadInterstitial()
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
