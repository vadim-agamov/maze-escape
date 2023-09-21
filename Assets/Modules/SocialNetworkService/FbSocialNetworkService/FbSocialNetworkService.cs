using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Modules.ServiceLocator;
using UnityEngine;

namespace Modules.SocialNetworkService.FbSocialNetworkService
{
    // https://docs.unity3d.com/Manual/webgl-interactingwithbrowserscripting.html
    public partial class FbSocialNetworkService : MonoBehaviour, ISocialNetworkService
    {
        [DllImport("__Internal")]
        private static extern string FbGetUserId();
        
        [DllImport("__Internal")]
        private static extern void FBGetData();
    
        [DllImport("__Internal")]
        private static extern void FBSetData(string data);
        
        [DllImport("__Internal")]
        private static extern void FBLogEvent(string eventName);
        
        [DllImport("__Internal")]
        private static extern void FBStartGame();
        
        private ISocialNetworkService This => this;
        private UniTaskCompletionSource<string> _loadPlayerProgressCompletionSource;
        private UniTaskCompletionSource _startGameCompletionSource;


        private const string RewardedInterstitialAdId = "644999433795437_683351186626928";

        public bool IsInitialized { get; private set; }
        
        string ISocialNetworkService.GetUserId() => FbGetUserId();

        #region Player Progress

        UniTask<string> ISocialNetworkService.LoadPlayerProgress()
        {
            _loadPlayerProgressCompletionSource = new UniTaskCompletionSource<string>();
            FBGetData();
            return _loadPlayerProgressCompletionSource.Task;
        }

        [UsedImplicitly]
        public void OnPlayerProgressLoaded(string dataStr)
        {
            Debug.Log($"[{nameof(FbSocialNetworkService)}] OnPlayerProgressLoaded: {dataStr}");
            _loadPlayerProgressCompletionSource.TrySetResult(dataStr);
            _loadPlayerProgressCompletionSource = null;
        }

        UniTask ISocialNetworkService.SavePlayerProgress(string data)
        {
            Debug.Log($"[{nameof(FbSocialNetworkService)}] SavePlayerProgress: {data}");
            FBSetData(data);
            return UniTask.CompletedTask;
        }

        #endregion

        public void LogEvent(string eventName, Dictionary<string, object> _)
        {
            Debug.Log($"[{nameof(FbSocialNetworkService)}] LogEvent: {eventName}");
            
            FBLogEvent(eventName);
        }

        async UniTask IService.Initialize(CancellationToken cancellationToken)
        {
            Debug.Log($"[{nameof(FbSocialNetworkService)}] Initialize");
            DontDestroyOnLoad(gameObject);
            _startGameCompletionSource = new UniTaskCompletionSource();
            FBStartGame();
            await _startGameCompletionSource.Task;
            
            This.PreloadInterstitial();
            This.PreloadRewardedVideo();
            This.PreloadRewardedInterstitial();
            IsInitialized = true;
        }

        void IService.Dispose()
        {
        }

        [UsedImplicitly]
        public void OnGameStarted()
        {
            Debug.Log($"[{nameof(FbSocialNetworkService)}] OnGameStarted");
            _startGameCompletionSource.TrySetResult();
            _startGameCompletionSource = null;
        }
        
        [UsedImplicitly]
        public void OnGameNotStarted(string message)
        {
            Debug.Log($"[{nameof(FbSocialNetworkService)}] OnGameNotStarted: {message}");
            _startGameCompletionSource.TrySetException(new Exception(message));
            _startGameCompletionSource = null;
        }
    }
}