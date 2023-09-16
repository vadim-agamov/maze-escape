using System.IO;
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
            var levels = GetProdLevels().ToList();
         //   levels.Add("Assets/Scenes/DevScenes_DevFolder/LevelEditorScene.unity");
       
            return levels.ToArray();
        }
       
        public static string[] GetProdLevels()
        {
            var levels = new []
            {
                "Assets/Scenes/Loading.unity",
                "Assets/Scenes/CoreMaze.unity",
                "Assets/Scenes/Lobby.unity",
                "Assets/Scenes/Empty.unity"
            };

            return levels;
        }

        public static void RemoveMobileWebglWarning(string buildPath)
        {
            var path = Path.Combine(buildPath, "Build/UnityLoader.js");
            var text = File.ReadAllText(path);
            text = text.Replace("UnityLoader.SystemInfo.mobile", "false");
            File.WriteAllText(path, text);
        }
        
        
        // public static void UploadFolderToHostinger(string localPath, string remotePath)
        // {
        //     using (var ftp = new FtpClient(FTPUtils.FtpHostingerIp, FTPUtils.FtpHostingerLogin, FTPUtils.FtpHostingerPswrd))
        //     {
        //         ftp.Connect();
        //
        //         if ( ftp.DirectoryExists(remotePath))
        //         {
        //             ftp.DeleteDirectory(remotePath);         
        //
        //             ftp.CreateDirectory(remotePath);
        //         }
        //
        //         ftp.UploadDirectory(localPath, remotePath, FtpFolderSyncMode.Update);
        //     }
        //
        //     Debug.Log("Upload complete" );
        // }
        
        
        public static void BuildAddressables()
        {
            Debug.Log("BuildAddressablesProcessor.PreExport start");
            AddressableAssetSettings.CleanPlayerContent(AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder);
            AddressableAssetSettings.BuildPlayerContent();
            Debug.Log("BuildAddressablesProcessor.PreExport done");
        }
        
        public static void EnableDevFolders()
        {
            var devEndingEnabled = "_DevFolder";
            var devEndingDisabled = devEndingEnabled + "~";

            var dirs = Directory.GetDirectories("Assets", "*", SearchOption.AllDirectories).Where(d => d.EndsWith(devEndingDisabled));

            foreach (var path in dirs)
            {
                var newPath = path.Replace(devEndingDisabled, devEndingEnabled);
                   
                Directory.Move(path, newPath);
                AssetDatabase.Refresh();
            }
        }
        
        public static void DisableDevFolders()
        {
            var devEndingEnabled = "_DevFolder";
            var devEndingDisabled = devEndingEnabled + "~";

            var dirs = Directory.GetDirectories("Assets", "*", SearchOption.AllDirectories).Where(d => d.EndsWith(devEndingEnabled));

            foreach (var path in dirs)
            {
                var newPath = path.Replace(devEndingEnabled, devEndingDisabled);
                   
                Directory.Move(path, newPath);
            }
           
            AssetDatabase.Refresh();
        }
        
        public static string GetDebugDefines()
        {
            return "DEV;UNITASK_DOTWEEN_SUPPORT;ANALYTICS";
        }
        public static string GetProdDefines()
        {
            return "UNITASK_DOTWEEN_SUPPORT;ANALYTICS;RELEASE";
        }
   }
}

