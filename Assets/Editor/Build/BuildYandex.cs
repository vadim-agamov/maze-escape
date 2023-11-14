using System.IO;
using Unity.SharpZipLib.Utils;
using UnityEditor;
using UnityEngine;

namespace Editor.Build
{
    public static class BuildYandex
    {
        private const string YandexDefine = "YANDEX";
        private const string WebGLTemplate = "PROJECT:Yandex";

        [MenuItem("Game/Build/YANDEX/BuildDev")]
        public static void BuildDev()
        {
            SetFbDebugDefines();
            DoBuild();
        }
        
        [MenuItem("Game/Build/YANDEX/BuildProd")]
        public static void BuildProd()
        {
            SetFbProdDefines();
            DoBuild();
        }
        
        private static void DoBuild()
        {
            BuildBase.IncrementBuildNumber();
            PlayerSettings.WebGL.template = WebGLTemplate;
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;

            var path = Application.dataPath.Replace("/Assets", $"/Builds/yandex/v{PlayerSettings.bundleVersion}");
            if (Directory.Exists(path))
            {
                Directory.Delete(path);
            }

            Directory.CreateDirectory(path);

            if (string.IsNullOrEmpty(path) != false)
            {
                return;
            }

            BuildBase.BuildAddressables();
            BuildPipeline.BuildPlayer(BuildBase.GetScenes(), path, BuildTarget.WebGL, BuildOptions.CleanBuildCache);
            ZipBuild(path);
        }

        private static string ZipBuild(string directoryPath)
        {
            var zipFile = directoryPath + ".zip";
            ZipUtility.CompressFolderToZip(zipFile, null, directoryPath);
            return zipFile;
        }
        
        [MenuItem("Game/Build/YANDEX/Set Defines Dev")]
        public static void SetFbDebugDefines()
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, DebugDefines);
        }
        
        [MenuItem("Game/Build/YANDEX/Set Defines Prod")]
        public static void SetFbProdDefines()
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, ProdDefines);
        }
        
        private static string DebugDefines => $"{BuildBase.DebugDefines};{YandexDefine}";
        private static string ProdDefines => $"{BuildBase.ProdDefines};{YandexDefine}";
    }
}

