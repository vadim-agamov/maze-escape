using Items;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(menuName = "Config/ItemConfig", fileName = "ItemConfig", order = 0)]
    public class ItemConfig: ScriptableObject
    {
        public string Id;
        public Item Prefab;
        public int Score;
        public float Size;
        public Sprite Icon;
        public bool UnlockedFromStart;
        public bool CanSpawn;
        public float Mass => 2;
    }
}