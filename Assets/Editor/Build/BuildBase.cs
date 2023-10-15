using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Editor.Build
{
    public static class BuildBase 
    {
        public static string[] GetScenes()
        {
            var scenes = new []
            {
                "Assets/Scenes/Loading.unity",
                "Assets/Scenes/CoreMaze.unity",
                "Assets/Scenes/Empty.unity"
            };

            return scenes;
        }
        
        public static void BuildAddressables()
        {
            Debug.Log("BuildAddressablesProcessor.PreExport start");
            AddressableAssetSettings.CleanPlayerContent(AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder);
            AddressableAssetSettings.BuildPlayerContent();
            Debug.Log("BuildAddressablesProcessor.PreExport done");
        }
        
        public static string DebugDefines => $"UNITASK_DOTWEEN_SUPPORT;ANALYTICS;DEV;";
        public static string ProdDefines => "UNITASK_DOTWEEN_SUPPORT;ANALYTICS;RELEASE";

        public static void IncrementBuildNumber()
        {
            var buildNumber = PlayerSettings.bundleVersion.Split('.');
            buildNumber[^1] = (int.Parse(buildNumber[^1]) + 1).ToString();
            PlayerSettings.bundleVersion = string.Join(".", buildNumber);
        }
   }
}

