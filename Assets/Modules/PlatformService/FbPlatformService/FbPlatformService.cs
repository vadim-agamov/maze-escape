using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Modules.ServiceLocator;
using UnityEngine;

namespace Modules.PlatformService.FbPlatformService
{
    // https://docs.unity3d.com/Manual/webgl-interactingwithbrowserscripting.html
    public partial class FbPlatformService : MonoBehaviour, IPlatformService
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
        
        private IPlatformService This => this;
        private UniTaskCompletionSource<string> _loadPlayerProgressCompletionSource;
        private UniTaskCompletionSource _startGameCompletionSource;
        
        public bool IsInitialized { get; private set; }
        
        string IPlatformService.GetUserId() => FbGetUserId();

        #region Player Progress

        UniTask<string> IPlatformService.LoadPlayerProgress()
        {
            _loadPlayerProgressCompletionSource = new UniTaskCompletionSource<string>();
            FBGetData();
            return _loadPlayerProgressCompletionSource.Task;
        }

        [UsedImplicitly]
        public void OnPlayerProgressLoaded(string dataStr)
        {
            Debug.Log($"[{nameof(FbPlatformService)}] OnPlayerProgressLoaded: {dataStr}");
            _loadPlayerProgressCompletionSource.TrySetResult(dataStr);
            _loadPlayerProgressCompletionSource = null;
        }

        UniTask IPlatformService.SavePlayerProgress(string data)
        {
            Debug.Log($"[{nameof(FbPlatformService)}] SavePlayerProgress: {data}");
            FBSetData(data);
            return UniTask.CompletedTask;
        }

        #endregion

        public void LogEvent(string eventName, Dictionary<string, object> _)
        {
            Debug.Log($"[{nameof(FbPlatformService)}] LogEvent: {eventName}");
            
            FBLogEvent(eventName);
        }

        async UniTask IService.Initialize(CancellationToken cancellationToken)
        {
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
            Debug.Log($"[{nameof(FbPlatformService)}] OnGameStarted");
            _startGameCompletionSource.TrySetResult();
            _startGameCompletionSource = null;
        }
        
        [UsedImplicitly]
        public void OnGameNotStarted(string message)
        {
            Debug.Log($"[{nameof(FbPlatformService)}] OnGameNotStarted: {message}");
            _startGameCompletionSource.TrySetException(new Exception(message));
            _startGameCompletionSource = null;
        }
    }
}