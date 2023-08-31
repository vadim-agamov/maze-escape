using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;
using Modules.SnBridge;
using Newtonsoft.Json;
using UnityEngine;

namespace Services.PlayerDataService
{
    public class PlayerDataService: IPlayerDataService
    {
        private PlayerData _data;
        
        async UniTask IService.Initialize(IProgress<float> progress, CancellationToken cancellationToken)
        {
            Debug.Log($"[{nameof(PlayerDataService)}] Initialize");
            var data = await SnBridge.Instance.LoadPlayerProgress();
            if (string.IsNullOrEmpty(data))
            {
                _data = new PlayerData();
                Debug.Log($"[{nameof(PlayerDataService)}] Initialized with empty data");
                return;
            }
            
            try
            {
                _data = JsonConvert.DeserializeObject<PlayerData>(data);
            }
            catch(Exception e)
            {
               Debug.LogError($"[{nameof(PlayerDataService)}] Unable to deserialize player data:\r\n{e}");
            }

            _data ??= new PlayerData();
            Debug.Log($"[{nameof(PlayerDataService)}] Initialized");
        }

        void IService.Dispose()
        {
        }

        PlayerData IPlayerDataService.Data => _data;

        void IPlayerDataService.Commit()
        {
            var data = JsonConvert.SerializeObject(_data);
            SnBridge.Instance.SavePlayerProgress(data);
        }

        void IPlayerDataService.Reset()
        {
            _data = new PlayerData();
            ((IPlayerDataService)this).Commit();
        }
    }
}