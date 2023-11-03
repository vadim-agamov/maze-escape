using System.Collections.Generic;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;
using UnityEngine;

namespace Modules.PlatformService.CrazyGamesPlatformService
{
    public class CrazyPlatformService: IPlatformService
    {
		private static string PlayerProgressPath => Path.Combine(Application.persistentDataPath, "player_progress.json");
		
		private UniTaskCompletionSource _showAdCompletionSource;

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
			return true;
		}

		async UniTask<bool> IPlatformService.ShowInterstitial(CancellationToken token)
		{
			_showAdCompletionSource.TrySetCanceled();
			_showAdCompletionSource = new UniTaskCompletionSource();
			CrazyGames.CrazyAds.Instance.beginAdBreak(
				() => _showAdCompletionSource.TrySetResult(),
				() => _showAdCompletionSource.TrySetResult());
			await _showAdCompletionSource.Task;
			return true;
		}

		void IPlatformService.PreloadRewardedInterstitial()
		{
		}

		async UniTask<bool> IPlatformService.ShowRewardedInterstitial(CancellationToken token)
		{ 
			return false;
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

		UniTask IService.Initialize(CancellationToken cancellationToken)
		{
			return UniTask.CompletedTask;
	    }

		void IService.Dispose()
		{
		}
    }
}