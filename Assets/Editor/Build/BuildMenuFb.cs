using System;
using System.IO;
using Unity.SharpZipLib.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Build
{
    public static class BuildMenuFb
    {
        //constants
        private const string FbDefine = "FB";
        private const string FbWebGLTemplate = "PROJECT:FbMinimal";
        private const string AppId = "644999433795437";
        private const string UploadToken = "GG|644999433795437|TfueuQGqjs8BA0pjbsBOS945O1o";
        
        [MenuItem("DEV/FB/BUILD")]
        public static void BuildProd()
        {
            PlayerSettings.WebGL.template = FbWebGLTemplate;
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;

            var now = DateTime.Now;
            var path = Application.dataPath.Replace("/Assets", $"/Builds/fb/v_{now.Day}.{now.Month}-{now.Hour}.{now.Minute}.{now.Second}");
            if (Directory.Exists(path))
            {
                Directory.Delete(path);
            }

            Directory.CreateDirectory(path);

            if (string.IsNullOrEmpty(path) != false)
            {
                return;
            }
            
            ChangeConstants(true);
            BuildBase.BuildAddressables();
            BuildPipeline.BuildPlayer(BuildBase.GetProdLevels(), path, BuildTarget.WebGL, BuildOptions.None);
                
            var zipFile = ZipBuild(path);
            UploadToFb(zipFile);
        }

        private static string ZipBuild(string directoryPath)
        {
            var zipFile = directoryPath + ".zip";
            ZipUtility.CompressFolderToZip(zipFile, null, directoryPath);
            return zipFile;
        }

        private static void UploadToFb(string pathToFile)
        {
            var filename = Path.GetFileName(pathToFile);
            var zipBytes = File.ReadAllBytes(pathToFile);
            
            var url = $"https://graph-video.facebook.com/{AppId}/assets";
            var form = new WWWForm();
            form.AddBinaryData("asset", zipBytes, filename);
            form.AddField("comment", $"uploaded at {DateTime.Now.ToShortDateString()}");
            form.AddField("type", "BUNDLE");
            form.AddField("access_token", UploadToken);
            var request = UnityWebRequest.Post(url, form);
            Debug.Log($"UploadToFb begin: {request.url}");
            var operation = request.SendWebRequest();
            operation.completed += _ =>
            {
                Debug.Log($"UploadToFb end: status {operation.webRequest.downloadHandler.text}");
            };
        }

        [MenuItem("DEV/FB/SET DEFINES/DEBUG")]
        public static void SetFbDebugDefines()
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL ,GetFbDebugDefines());
            // PlayerSettings.WebGL.template = FbWebGLTemplate;

            BuildBase.EnableDevFolders();
        }
        
        [MenuItem("DEV/FB/SET DEFINES/PROD")]
        public static void SetFbProdDefines()
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL ,GetFbProdDefines());
            
            BuildBase.DisableDevFolders();
        }
        
        private static string GetFbDebugDefines()
        {
            return BuildBase.GetDebugDefines() + ";" + FbDefine;
        }

        private static string GetFbProdDefines()
        {
            return BuildBase.GetProdDefines() + ";" + FbDefine;
        }

       private static void ChangeConstants(bool incrementBuildNumber)
       {
           // var constants = (Constants)AssetDatabase.LoadAssetAtPath(@"Assets\Resources\Constants.asset", typeof(Constants));
           //
           // AssetDatabase.StartAssetEditing();
           //
           // constants.BuildTime = DateTime.UtcNow.ToString("dd.MM.yyyy hh:mm:ss");
           // if (incrementBuildNumber)
           // {
           //     constants.BuildNumberFB++;
           // }
           //
           // EditorUtility.SetDirty(constants);
           //
           // AssetDatabase.StopAssetEditing();
           // AssetDatabase.SaveAssets();
       }
    }
}

