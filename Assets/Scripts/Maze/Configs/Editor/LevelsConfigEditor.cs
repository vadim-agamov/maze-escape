using UnityEditor;
using UnityEngine;

namespace Maze.Configs.Editor
{
    [CustomEditor(typeof(LevelsConfig))]
    public class LevelsConfigEditor : UnityEditor.Editor
    {
        private SerializedProperty _levels;

        private void OnEnable()
        {
            _levels = serializedObject.FindProperty("_levels");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_levels);

            if (GUILayout.Button("Generate"))
            {
                var levelsConfig = (LevelsConfig)serializedObject.targetObject;
                levelsConfig.FetchLevels();
                serializedObject.Update();
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}