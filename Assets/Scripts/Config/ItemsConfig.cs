using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Config
{
    [Serializable]
    public class ItemsMergeRule
    {
        public ItemConfig A;
        public ItemConfig B;
        public ItemConfig Result;
    }

    [CreateAssetMenu(menuName = "Config/ItemsConfig", fileName = "ItemsConfig", order = 0)]
    public class ItemsConfig : ScriptableObject
    {
        [SerializeField]
        private ItemConfig[] _items;

        [SerializeField]
        private ItemsMergeRule[] _mergeRules;

        public IEnumerable<ItemConfig> Items => _items;

        public bool TryGetMergeResult(ItemConfig a, ItemConfig b, out ItemConfig result)
        {
            foreach (var rule in _mergeRules)
            {
                if (rule.A == a && rule.B == b)
                {
                    result = rule.Result;
                    return true;
                }
            }

            result = null;
            return false;
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            _items = AssetDatabase.FindAssets($"t:{nameof(ItemConfig)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<ItemConfig>)
                .ToArray();
        }
#endif
    }
}