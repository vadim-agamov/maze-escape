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
        private IPlatformService PlatformService { get; set; }
        
        protected TData Data;
        private bool _needSave;
        private bool _savingIsInProgress;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        async UniTask IService.Initialize(CancellationToken cancellationToken)
        {
            PlatformService = ServiceLocator.ServiceLocator.Get<IPlatformService>();
            
            DontDestroyOnLoad(gameObject);
            gameObject.name = $"[{nameof(PlayerDataService)}]";

            var data = await PlatformService.LoadPlayerProgress();
            if (string.IsNullOrEmpty(data))
            {
                Data = new TData();
                Debug.Log($"[{nameof(PlayerDataService)}] Initialized with empty data");
            }
            else
            {
                try
                {
                    Data = JsonConvert.DeserializeObject<TData>(data);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[{nameof(PlayerDataService)}] Unable to deserialize player data:\r\n{e}");
                }

                Data ??= new TData();
            }
        }

        void IService.Dispose()
        {
            _cancellationTokenSource.Cancel();
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

        public void Commit() => SetDirty();

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
                    await PlatformService.SavePlayerProgress(data, _cancellationTokenSource.Token);
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