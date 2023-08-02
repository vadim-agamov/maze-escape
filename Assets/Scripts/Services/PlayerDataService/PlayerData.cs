using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;
using Newtonsoft.Json;
using SN;

namespace Services.PlayerDataService
{
    public class PlayerDataService: IPlayerDataService
    {
        private PlayerData _data;
        
        async UniTask IService.Initialize(IProgress<float> progress, CancellationToken cancellationToken)
        {
            _data = await SnBridge.Instance.LoadPlayerProgress();
        }

        void IService.Dispose()
        {
        }

        PlayerData IPlayerDataService.Data => _data;

        void IPlayerDataService.Commit()
        {
            SnBridge.Instance.SavePlayerProgress(_data);
        }
    }
    
    public class PlayerData
    {
        [Serializable]
        public struct Consumable
        {
            public string Id;
            public int Amount;
            [JsonProperty]
            public DateTime EndTime;
            [JsonIgnore]
            public bool IsInfinity => EndTime > DateTime.UtcNow;
        }
        
        public int MaxScore;
        public DateTime InstallDate;
        public int Level;
        
        [JsonProperty]
        public List<string> UnlockedItems = new List<string>();
        
        [JsonProperty("Consumables")]
        private List<Consumable> _consumables = new List<Consumable>();

        public Consumable GetConsumable(string id)
        {
            for (var index = 0; index < _consumables.Count; index++)
            {
                var c = _consumables[index];
                if (c.Id == id)
                {
                    return c;
                }
            }

            var newConsumable = new Consumable { Id = id };
            _consumables.Add(newConsumable);
            return newConsumable;
        }

        public Consumable AddConsumable(string id, int amount)
        {
            for (var index = 0; index < _consumables.Count; index++)
            {
                var c = _consumables[index];
                if (c.Id == id)
                {
                    c.Amount += amount;
                    _consumables[index] = c;
                    return c;
                }
            }

            var newConsumable = new Consumable
            {
                Id = id,
                Amount = amount
            };
            _consumables.Add(newConsumable);
            return newConsumable;
        }

        public Consumable AddConsumable(string id, TimeSpan duration)
        {
            for (var index = 0; index < _consumables.Count; index++)
            {
                var c = _consumables[index];
                if (c.Id == id)
                {
                    c.EndTime += duration;
                    _consumables[index] = c;
                    return c;
                }
            }

            var newConsumable = new Consumable
            {
                Id = id,
                EndTime = DateTime.UtcNow + duration
            };
            _consumables.Add(newConsumable);
            return newConsumable;
        }
        
        public void Reset()
        {
            Level = 0;
            MaxScore = default;
            InstallDate = default;
            UnlockedItems = new List<string>();
            _consumables = new List<Consumable>();
        }
    }
}