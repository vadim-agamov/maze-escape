using System;
using Newtonsoft.Json;

namespace Services.GamePlayerDataService
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
        public DateTime AdsLastShownDate;
        
        [JsonProperty]
        public int Level;
        
        [JsonProperty]
        public bool MuteSound;
        
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
            MuteSound = true;
        }
    }
}