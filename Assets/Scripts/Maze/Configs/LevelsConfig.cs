using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Maze.Configs
{
    [CreateAssetMenu(menuName = "Create LevelsConfig", fileName = "LevelsConfig", order = 0)]
    public class LevelsConfig : ScriptableObject
    {
        [SerializeField] 
        private LevelConfig[] _levels;
        
        public LevelConfig[] LevelsConfigs => _levels;

        private void OnValidate()
        {
            for (var index = 0; index < _levels.Length; index++)
            {
                _levels[index].LevelId = index;
            }
        }

#if UNITY_EDITOR
        public void FetchLevels()
        {
            Clear();
            var levels = AssetDatabase.FindAssets($"t:{nameof(LevelConfig)}")    
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<LevelConfig>)
                .ToList();
            
            levels.Sort((a,b) => a.MinPath - b.MinPath);
            _levels = levels.ToArray();
        }

        public void Clear()
        {
            _levels = Array.Empty<LevelConfig>();
            EditorUtility.SetDirty(this);
        }
#endif
    }
}