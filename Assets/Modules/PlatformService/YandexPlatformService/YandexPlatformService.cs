#if YANDEX
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Modules.ServiceLocator;
using UnityEngine;

namespace Modules.PlatformService.YandexPlatformService
{
    public partial class YandexPlatformService : MonoBehaviour, IPlatformService
    {
        [DllImport("__Internal")]
        private static extern string YandexGetUserId();
        
        [DllImport("__Internal")]
        private static extern void YandexGetData();
        
        [DllImport("__Internal")]
        private static extern void YandexSetData(string data);
        
        [DllImport("__Internal")]
        private static extern void YandexLogEvent(string eventName);
        
        [DllImport("__Internal")]
        private static extern void YandexStartGame();
        
        [DllImport("__Internal")]
        private static extern void YandexGameReady();
        
        [DllImport("__Internal")]
        private static extern string YandexGetLanguage();
        
        private IPlatformService This => this;
        private UniTaskCompletionSource<string> _loadPlayerProgressCompletionSource;
        private UniTaskCompletionSource _startGameCompletionSource;

        Language IPlatformService.GetLocale()
        {
            var locale = YandexGetLanguage();
            return locale switch
            {
                "ru" => Language.Russian,
                "en" => Language.English,
                "tr" => Language.Turkish,
                _ => Language.English
            };
        }

        string IPlatformService.GetUserId() => YandexGetUserId();

        #region Player Progress

        UniTask<string> IPlatformService.LoadPlayerProgress()
        {
            _loadPlayerProgressCompletionSource = new UniTaskCompletionSource<string>();
            YandexGetData();
            return _loadPlayerProgressCompletionSource.Task;
        }

        [UsedImplicitly]
        public void YandexOnPlayerProgressLoaded(string dataStr)
        {
            Debug.Log($"[{nameof(YandexPlatformService)}] OnPlayerProgressLoaded: {dataStr}");
            _loadPlayerProgressCompletionSource.TrySetResult(dataStr);
            _loadPlayerProgressCompletionSource = null;
        }

        UniTask IPlatformService.SavePlayerProgress(string data, CancellationToken token)
        {
            Debug.Log($"[{nameof(YandexPlatformService)}] SavePlayerProgress: {data}");
            YandexSetData(data);
            return UniTask.CompletedTask;
        }

        #endregion

        public void LogEvent(string eventName, Dictionary<string, object> _)
        {
            Debug.Log($"[{nameof(YandexPlatformService)}] LogEvent: {eventName}");
            YandexLogEvent(eventName);
        }

        void IPlatformService.GameReady() => YandexGameReady();

        async UniTask IService.Initialize(CancellationToken cancellationToken)
        {
            DontDestroyOnLoad(gameObject);
            _startGameCompletionSource = new UniTaskCompletionSource();
            YandexStartGame();
            await _startGameCompletionSource.Task;
            
            This.PreloadInterstitial();
            This.PreloadRewardedVideo();
            This.PreloadRewardedInterstitial();
        }

        void IService.Dispose()
        {
        }

        [UsedImplicitly]
        public void YandexOnGameStarted()
        {
            Debug.Log($"[{nameof(YandexPlatformService)}] OnGameStarted");
            _startGameCompletionSource.TrySetResult();
            _startGameCompletionSource = null;
        }
        
        [UsedImplicitly]
        public void YandexOnGameNotStarted(string message)
        {
            Debug.Log($"[{nameof(YandexPlatformService)}] OnGameNotStarted: {message}");
            _startGameCompletionSource.TrySetException(new Exception(message));
            _startGameCompletionSource = null;
        }
    }
}
#endif