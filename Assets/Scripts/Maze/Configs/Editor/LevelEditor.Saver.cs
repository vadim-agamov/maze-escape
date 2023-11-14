using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Maze.Configs.Editor
{
    public partial class LevelEditor
    {
        private string _levelName = $"level_";
        private Object _levelConfigObject;

        private void LoadButton()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Load");
            _levelConfigObject = EditorGUILayout.ObjectField(_levelConfigObject, typeof(LevelConfig), true);
            if (GUILayout.Button("Load"))
            {
                var levelConfig = _levelConfigObject as LevelConfig;
                _levelName = levelConfig.name;
                _cells = levelConfig.Cells;
                _goals = levelConfig.Goals.ToList();
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private void SaveButton()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Save");
            _levelName = GUILayout.TextField(_levelName);
            if (GUILayout.Button("Save"))
            {
                var asset = CreateInstance<LevelConfig>();
                asset.Cells = _cells;
                asset.Goals = _goals.ToArray();
                AssetDatabase.CreateAsset(asset, $"Assets/Configs/{_levelName}.asset");
                AssetDatabase.SaveAssets();
                _levelConfigObject = asset;
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}