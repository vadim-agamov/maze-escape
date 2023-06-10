using UnityEngine;

namespace Consumables.Scripts
{
    [CreateAssetMenu(fileName = "Consumable", menuName = "Configs/ConsumableConfig")]
    public class Consumable: ScriptableObject
    {
        [SerializeField]
        private string _id;
        [SerializeField]
        private Sprite _icon;
        [SerializeField]
        private string _name;

        public string Id => _id;
        public Sprite Icon => _icon;
        public string Name => _name;
    }
}