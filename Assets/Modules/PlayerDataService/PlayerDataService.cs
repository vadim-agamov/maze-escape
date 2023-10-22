using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.PlatformService;
using Modules.ServiceLocator;
using Newtonsoft.Json;
using UnityEngine;

namespace Modules.PlayerDataService
{
    public abstract class PlayerDataService<TData> : MonoBehaviour, IService where TData : new()
    {
        protected TData Data;
        private IPlatformService _platformService;
        private bool _needSave;
        private bool _savingIsInProgress;
        private CancellationTokenSource _cancellationTokenSource;
        
        async UniTask IService.Initialize(CancellationToken cancellationToken)
        {
            DontDestroyOnLoad(gameObject);
            gameObject.name = $"[{nameof(PlayerDataService)}]";
            _cancellationTokenSource = new CancellationTokenSource();
            _platformService = await ServiceLocator.ServiceLocator.GetAsync<IPlatformService>(cancellationToken);
            
            var data = await _platformService.LoadPlayerProgress();
            if (string.IsNullOrEmpty(data))
            {
                Data = new TData();
                Debug.Log($"[{nameof(PlayerDataService)}] Initialized with empty data");
                return;
            }
            
            try
            {
                Data = JsonConvert.DeserializeObject<TData>(data);
            }
            catch(Exception e)
            {
                Debug.LogError($"[{nameof(PlayerDataService)}] Unable to deserialize player data:\r\n{e}");
            }

            Data ??= new TData();
        }

        void IService.Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = null;
        }
        
        protected void SetDirty()
        {
            _needSave = true;
        }

        public void ResetData()
        {
            Data = new TData();
            SetDirty();
        }

        private void Update()
        {
            if (_needSave && !_savingIsInProgress)
            {
                CommitAsync().Forget();
            }

            async UniTask CommitAsync()
            {
                try
                {
                    _savingIsInProgress = true;
                    var data = JsonConvert.SerializeObject(Data);
                    await _platformService.SavePlayerProgress(data, _cancellationTokenSource.Token);
                }
                catch(Exception exception)
                {
                    Debug.LogError($"[{nameof(PlayerDataService)}] error while saving: {exception}");
                }

                _needSave = false;
                _savingIsInProgress = false;
            }
        }
    }
}