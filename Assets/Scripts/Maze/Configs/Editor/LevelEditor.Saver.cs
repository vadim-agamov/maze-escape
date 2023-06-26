using UnityEditor;
using UnityEngine;

namespace Maze.Configs.Editor
{
    public partial class LevelEditor
    {
        private LevelConfig _levelConfig;
        private string _levelName = $"level_";

        private void LoadButton()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Load");
            var config = (LevelConfig)EditorGUILayout.ObjectField(_levelConfig, typeof(LevelConfig), true);
            if (config != null && config != _levelConfig)
            {
                _levelConfig = config;
                _levelName = _levelConfig.name;
                _cells = _levelConfig.Cells;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void SaveButton()
        {
            if(_foundMinPath <= 0)
                return;
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Save");
            _levelName = GUILayout.TextField(_levelName);
            if (GUILayout.Button("Save"))
            {
                var asset = CreateInstance<LevelConfig>();
                asset.Cells = _cells;
                asset.SetMinPath(_foundMinPath);
                AssetDatabase.CreateAsset(asset, $"Assets/Configs/{_levelName}.asset");
                AssetDatabase.SaveAssets();
                _levelConfig = asset;
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}