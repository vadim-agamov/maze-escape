using System;
using System.IO;
using System.Threading;
using Unity.SharpZipLib.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Editor.Build
{
    public static class BuildFb
    {
        //constants
        private const string FbDefine = "FB";
        private const string FbWebGLTemplate = "PROJECT:FbMinimal";
        private const string AppId = "644999433795437";
        private const string UploadToken = "GG|644999433795437|TfueuQGqjs8BA0pjbsBOS945O1o";
        
        private static UnityWebRequest _request;

        [MenuItem("Game/Build/FB/Build")]
        public static void Build()
        {
            SetFbDebugDefines();
            
            BuildBase.IncrementBuildNumber();
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
            
            BuildBase.BuildAddressables();
            BuildPipeline.BuildPlayer(BuildBase.GetProdScenes(), path, BuildTarget.WebGL, BuildOptions.CleanBuildCache);
                
            var zipFile = ZipBuild(path);
            UploadToFb(zipFile);

            while (!_request.isDone)
            {
                Thread.Sleep(5000);
                Debug.Log($"Waiting for upload to finish... {_request.uploadProgress}");
            }
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
            _request = UnityWebRequest.Post(url, form);
            Debug.Log($"UploadToFb begin: {_request.url}");
            var operation = _request.SendWebRequest();
            operation.completed += _ =>
            {
                Debug.Log($"UploadToFb end: status {operation.webRequest.downloadHandler.text}");
            };
        }

        [MenuItem("Game/Build/FB/Set Defines Dev")]
        public static void SetFbDebugDefines()
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL ,GetFbDebugDefines());
        }
        
        [MenuItem("Game/Build/FB/Set Defines Prod")]
        public static void SetFbProdDefines()
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL ,GetFbProdDefines());
        }
        
        private static string GetFbDebugDefines()
        {
            return BuildBase.GetDebugDefines() + ";" + FbDefine;
        }

        private static string GetFbProdDefines()
        {
            return BuildBase.GetProdDefines() + ";" + FbDefine;
        }
    }
}

