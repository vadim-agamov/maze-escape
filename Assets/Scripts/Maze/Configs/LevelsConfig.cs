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
    }
}