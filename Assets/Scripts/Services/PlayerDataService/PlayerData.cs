using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Services.PlayerDataService
{
    public class PlayerData
    {
        [JsonProperty]
        public int MaxScore;
        
        [JsonProperty]
        public DateTime InstallDate;
        
        [JsonProperty]
        public DateTime LastSessionDate;
        
        [JsonProperty]
        public int Level;
        
        public PlayerData()
        {
            Reset();
        }
        
        public void Reset()
        {
            Level = 0;
            MaxScore = default;
            InstallDate = DateTime.Now;
            LastSessionDate = DateTime.Now;
        }
    }
}