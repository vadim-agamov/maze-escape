using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Editor.Build
{
    public static class BuildBase 
    {
        public static string[] GetDebugLevels()
        {
            var levels = GetProdScenes().ToList();
            return levels.ToArray();
        }
       
        public static string[] GetProdScenes()
        {
            var levels = new []
            {
                "Assets/Scenes/Loading.unity",
                "Assets/Scenes/CoreMaze.unity",
                "Assets/Scenes/Empty.unity"
            };

            return levels;
        }
        
        public static void BuildAddressables()
        {
            Debug.Log("BuildAddressablesProcessor.PreExport start");
            AddressableAssetSettings.CleanPlayerContent(AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder);
            AddressableAssetSettings.BuildPlayerContent();
            Debug.Log("BuildAddressablesProcessor.PreExport done");
        }
        
        public static string GetDebugDefines()
        {
            return "DEV;UNITASK_DOTWEEN_SUPPORT;ANALYTICS";
        }
        public static string GetProdDefines()
        {
            return "UNITASK_DOTWEEN_SUPPORT;ANALYTICS;RELEASE";
        }
        
        public static void IncrementBuildNumber()
        {
            var buildNumber = PlayerSettings.bundleVersion.Split('.');
            buildNumber[^1] = (int.Parse(buildNumber[^1]) + 1).ToString();
            PlayerSettings.bundleVersion = string.Join(".", buildNumber);
        }
   }
}

