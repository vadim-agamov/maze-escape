using System;
using UnityEditor;
using Utils;

namespace Build
{
    public static class BuildMenuMobile
    {
        [MenuItem("DEV/MOBILE/SET DEFINES/MOBILE_PROD")]
        public static void SetProdDefines()
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, GetProdDefines());
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, GetProdDefines());

            BuildBase.DisableDevFolders();
        }

        [MenuItem("DEV/MOBILE/SET DEFINES/MOBILE_DEBUG")]
        public static void SetDebugDefines()
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, BuildBase.GetDebugDefines());
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, GetDebugDefines());
            
            BuildBase.EnableDevFolders();
        }
        

       //ios
       [MenuItem("DEV/MOBILE/IOS/BUILD_PROD")]
       public static void BuildIOSProd()
       {
           PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, GetProdDefines());
           IncrementIOSVersion();

           // PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, 1);

           string path = EditorUtility.SaveFilePanel("Choose Location of Built Game", "", "", "ipa");

           if (string.IsNullOrEmpty(path) == false)
           {
               EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
               EditorUserBuildSettings.buildAppBundle = true;
               ChangeConstants(BuildTarget.iOS);

               BuildBase.BuildAddressables();
               BuildPipeline.BuildPlayer(GetProdLevels(), path, BuildTarget.iOS, BuildOptions.None);
           }
       }


       [MenuItem("DEV/MOBILE/IOS/BUILD_DEBUG")]
       public static void BuildIOSDebug()
       {
           PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, GetDebugDefines());
           IncrementIOSVersion();

           string path = EditorUtility.SaveFilePanel("Choose Location of Built Game", "", "", "ipa");

           if (string.IsNullOrEmpty(path) == false)
           {
               ChangeConstants(BuildTarget.iOS);

               BuildBase.BuildAddressables();
               BuildPipeline.BuildPlayer(GetDebugLevels(), path, BuildTarget.iOS, BuildOptions.None);
           }
       }

       private static void IncrementIOSVersion()
       {
           // PlayerSettings.iOS.buildNumber = (PlayerSettings.iOS.buildNumber.GetIntSafe() + 1).ToString();
       }

       //AN
       [MenuItem("DEV/MOBILE/AN/BUILD_PROD_AAB")]
       public static void BuildAndroidProd()
       {
           SetANKeyAlias();
           PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, GetProdDefines());
           IncrementANVersion();

           string path = EditorUtility.SaveFilePanel("Choose Location of Built Game", "", PlayerSettings.Android.bundleVersionCode.ToString(), "aab");

           if (string.IsNullOrEmpty(path) == false)
           {
               EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
               EditorUserBuildSettings.buildAppBundle = true;
               ChangeConstants(BuildTarget.Android);
               //EditorUserBuildSettings.
               BuildBase.BuildAddressables();
               BuildPipeline.BuildPlayer(GetProdLevels(), path, BuildTarget.Android, BuildOptions.None);
           }
       }

       [MenuItem("DEV/MOBILE/AN/BUILD_DEBUG_AAB")]
       public static void BuildAndroidDebugAAB()
       {
           SetANKeyAlias();
           PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, GetDebugDefines());
           IncrementANVersion();

           string path = EditorUtility.SaveFilePanel("Choose Location of Built Game", "", PlayerSettings.Android.bundleVersionCode.ToString(), "aab");

           if (string.IsNullOrEmpty(path) == false)
           {
               EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
               EditorUserBuildSettings.buildAppBundle = true;
               ChangeConstants(BuildTarget.Android);

               BuildBase.BuildAddressables();
               BuildPipeline.BuildPlayer(GetProdLevels(), path, BuildTarget.Android, BuildOptions.None);
           }
       }

    
       [MenuItem("DEV/MOBILE/AN/BUILD_ANDROID_PROD_APK")]
       public static void BuildAndroidProdApk()
       {
           SetANKeyAlias();
           PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, GetProdDefines());
           //IncrementANVersion();

           string path = EditorUtility.SaveFilePanel("Choose Location of Built Game", "", PlayerSettings.Android.bundleVersionCode + "_prod", "apk");

           if (string.IsNullOrEmpty(path) == false)
           {
               EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
               EditorUserBuildSettings.buildAppBundle = false;
               ChangeConstants(BuildTarget.Android);

               BuildBase.BuildAddressables();
               BuildPipeline.BuildPlayer(GetProdLevels(), path, BuildTarget.Android, BuildOptions.None);
           }
       }

       [MenuItem("DEV/MOBILE/AN/BUILD_ANDROID_DEBUG_APK")]
       public static void BuildAndroidDebugApk()
       {
           SetANKeyAlias();
           PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, GetDebugDefines());
           //IncrementANVersion();

           string path = EditorUtility.SaveFilePanel("Choose Location of Built Game", "", PlayerSettings.Android.bundleVersionCode + "_debug", "apk");

           if (string.IsNullOrEmpty(path) == false)
           {
               EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
               EditorUserBuildSettings.buildAppBundle = false;

               ChangeConstants(BuildTarget.Android);
               BuildBase.BuildAddressables();
               BuildPipeline.BuildPlayer(GetDebugLevels(), path, BuildTarget.Android, BuildOptions.None);
           }
       }

       //AN
       private static void IncrementANVersion()
       {
           PlayerSettings.Android.bundleVersionCode++;
       }

       private static void SetANKeyAlias()
       {
           //passwords:
           PlayerSettings.keyaliasPass = "workfor";
           PlayerSettings.keystorePass = "workfor";
       }

      
       //IOS


       //DEBUG
       //
      
       
       private static string GetDebugDefines()
       {
           return "DEV;UNITASK_DOTWEEN_SUPPORT;ANALYTICS";
       }

       private static string[] GetDebugLevels()
       {
           var levels = new []
           {
               "Assets/Scenes/GameStarter.unity",
               "Assets/Scenes/LobbyScene.unity",
               "Assets/Scenes/Match3Scene.unity",
               "Assets/Scenes/DevScenes_DevFolder/LevelEditorScene.unity",
           };

           return levels;
       }


       //PROD    
       private static string GetProdDefines()
       {
           return "UNITASK_DOTWEEN_SUPPORT;ANALYTICS;RELEASE";
       }

       private static string[] GetProdLevels()
       {
           var levels = new []
           {
               "Assets/Scenes/GameStarter.unity",
               "Assets/Scenes/LobbyScene.unity",
               "Assets/Scenes/Match3Scene.unity",
           };

           return levels;
       }
       
       private static void ChangeConstants(BuildTarget target)
       {
           //var constants = AssetDatabase.Фыыуе<Constants>("Assets/Resources/Constants.asset");
           //var prefab2 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/PREF_Constants_New.prefab");
           var constants = (Constants)AssetDatabase.LoadAssetAtPath(@"Assets\Resources\Constants.asset", typeof(Constants));

           AssetDatabase.StartAssetEditing();

           constants.BuildTime = DateTime.UtcNow.ToString("dd.MM.yyyy hh:mm:ss");
           //constants.GitCommit = GetGitInfo("rev-parse HEAD").Trim();
           //constants.GitBranch = GetGitInfo("describe --all").Replace("remotes/origin/", string.Empty).Trim();

           if (target == BuildTarget.Android)
           {
               constants.BundleVersionCode = PlayerSettings.Android.bundleVersionCode;
           }
           else if (target == BuildTarget.iOS)
           {
               // constants.BundleVersionCode = PlayerSettings.iOS.buildNumber.GetInt();
           }

           EditorUtility.SetDirty(constants);

           AssetDatabase.StopAssetEditing();
           AssetDatabase.SaveAssets();
       }

     
   }
}

