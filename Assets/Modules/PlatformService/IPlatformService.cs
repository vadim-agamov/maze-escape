using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;

namespace Modules.PlatformService
{
	public interface IPlatformService : IService
	{
		string GetUserId();
		UniTask<string> LoadPlayerProgress();
		UniTask SavePlayerProgress(string playerProgress);

		void PreloadRewardedVideo();
		UniTask<bool> ShowRewardedVideo();

		void PreloadInterstitial();
		UniTask<bool> ShowInterstitial();
		
		void PreloadRewardedInterstitial();
		UniTask<bool> ShowRewardedInterstitial();
		
		void LogEvent(string eventName, Dictionary<string, object> parameters);
	}
}