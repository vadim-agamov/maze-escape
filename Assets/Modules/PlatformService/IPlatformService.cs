using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;

namespace Modules.PlatformService
{
	public interface IPlatformService : IService
	{
		Language GetLocale();
		string GetUserId();
		UniTask<string> LoadPlayerProgress();
		UniTask SavePlayerProgress(string playerProgress, CancellationToken token);

		void PreloadRewardedVideo();
		UniTask<bool> ShowRewardedVideo(CancellationToken token);

		void PreloadInterstitial();
		UniTask<bool> ShowInterstitial(CancellationToken token);
		
		void PreloadRewardedInterstitial();
		UniTask<bool> ShowRewardedInterstitial(CancellationToken token);
		
		void LogEvent(string eventName, Dictionary<string, object> parameters);
		void GameReady();
	}
}