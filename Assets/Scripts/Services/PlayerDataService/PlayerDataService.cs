using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;
using Modules.SocialNetworkService;
using Newtonsoft.Json;
using UnityEngine;

namespace Services.PlayerDataService
{
    public class PlayerDataService: IPlayerDataService
    {
        private PlayerData _data;
        private ISocialNetworkService _socialNetworkService;
        
        
        async UniTask IService.Initialize(CancellationToken cancellationToken)
        {
            Debug.Log($"[{nameof(PlayerDataService)}] Initialize begin");

            _socialNetworkService = await ServiceLocator.GetAsync<ISocialNetworkService>(cancellationToken);
            
            var data = await _socialNetworkService.LoadPlayerProgress();
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
            Debug.Log($"[{nameof(PlayerDataService)}] Initialize end");
        }

        void IService.Dispose()
        {
        }

        PlayerData IPlayerDataService.Data => _data;

        void IPlayerDataService.Commit()
        {
            var data = JsonConvert.SerializeObject(_data);
            _socialNetworkService.SavePlayerProgress(data);
        }

        void IPlayerDataService.Reset()
        {
            _data = new PlayerData();
            ((IPlayerDataService)this).Commit();
        }
    }
}