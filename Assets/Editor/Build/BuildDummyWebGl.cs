using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor.Build
{
    public static class BuildDummyWebGl
    {
        [MenuItem("Game/Build/WebGl/Build")]
        public static void Build()
        {
            SetDev();
            
            BuildBase.IncrementBuildNumber();
            PlayerSettings.WebGL.template = "PROJECT:Minimal";
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;

            var path = Application.dataPath.Replace("/Assets", $"/Builds/DummyWebGl");
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            Directory.CreateDirectory(path);

            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            
            BuildBase.BuildAddressables();
            BuildPipeline.BuildPlayer(BuildBase.GetProdScenes(), path, BuildTarget.WebGL, BuildOptions.CleanBuildCache);
        }

        [MenuItem("Game/Build/WebGl/Set Defines Dev")]
        public static void SetDev()
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, $"{BuildBase.GetDebugDefines()};DUMMY_WEBGL");
        }
        
        [MenuItem("Game/Build/WebGl/Set Defines Prod")]
        public static void SetProd()
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, $"{BuildBase.GetProdDefines()};DUMMY_WEBGL");
        }
    }
}