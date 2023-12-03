using System;
using System.Linq;
using UnityEngine;

namespace Modules.FlyItemsService
{
    [Serializable]
    public class FlyItemIcon
    {
        public Sprite Icon;
        public string Name;
    }

    [CreateAssetMenu(fileName = "FlyItemsConfig", menuName = "Configs/FlyItemsConfig", order = 0)]
    public class FlyItemsConfig : ScriptableObject
    {
        [SerializeField] private FlyItemIcon[] _icons;

        public Sprite GetIcon(string n) => _icons.First(x => x.Name == n).Icon;

        private void OnValidate()
        {
            foreach (var icon in _icons)
            {
                if (icon.Icon != null)
                {
                    icon.Name = icon.Icon.name;
                }
            }
        }
    }
}