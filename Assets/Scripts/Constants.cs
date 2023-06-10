using System;
using System.IO;
using UnityEngine;

#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace Utils
{
    [CreateAssetMenu(fileName = "Constants", menuName = "Constants")]
    public class Constants : ScriptableObject
    {
        public const string ProjectName = "Rtts";
        
        private static Constants _instance;

        public static Constants Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                _instance = Resources.Load<Constants>("Constants");
                
                return _instance;
            }
        }

        public void Initialize()
        {
            PersistanceDataPath = Application.persistentDataPath;
        }
        
        public static string GetPlayerProgressPath()
        {
            return Path.Combine(PersistanceDataPath, PlayerProgressFileName);
        }

        public const float FixedDeltaTime = 0.02f;

        public static Version ClientVersion;

        public static string PersistanceDataPath;

        public const string CommaSeparator = ",";


        //PlayerProgress
        public const string PlayerProgressFileName = "PlayerProgress";

        
        [SerializeField] private Material _grayScaleMaterial;
        public Material GrayScaleMaterial => _grayScaleMaterial;

        public static Color BlackVeilColor => new Color(0, 0, 0, 0.5f);

        public static Vector3 UIPosition = new Vector3(0, -100, 0);
        public static float UiCameraDistance = 1;

        public bool IsIPhoneX
        {
            get
            {
                #if UNITY_IOS
                    return Device.generation == DeviceGeneration.iPhoneX;
                #else
                    return false;
                #endif
            }
        }


        // Drag threshold in screen diagonal percent
        public const float TownDragThreshold = 0.01f;

        public const float BLACK_VEIL_TWEEN_DURATION = 0.12f;
        public const int POPUP_TWEEN_IN_START_SHIFT = 380;
        


  
        
        [SerializeField] private Material _grayScaleMaterial_ETC1;
        public Material GrayScaleMaterial_ETC1 => _grayScaleMaterial_ETC1;
     
        
        public string BuildTime = "none";
        
        
        public const string SN_Android = "an";
        public const string SN_Ios = "io";
        public const string SN_Amazon = "am";
        public const string SN_Facebook = "fb";
        public const string SN_Windows = "wi";
        public const string SN_MacOS = "ma";
        public static string GetCurrentPlatformType()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            return SN_Windows;
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
                return SN_MacOS;
            #elif UNITY_IOS
                return SN_Ios;
            #elif UNITY_ANDROID
                return SN_Android;
            #elif UNITY_WEBGL || PLATFORM_WEBGL
                return SN_Facebook;
            #else
                return SN_Android;
            #endif
        }
        
        public int BuildNumber 
        {
            get
            {
#if VK
                return BuildNumberVK;
#elif FB
                return BuildNumberFB;
#elif YANDEX
                return BuildNumberYANDEX;
#else
                return BundleVersionCode;
#endif
            }
        }
        public int BundleVersionCode = -1;
        public int BuildNumberVK = 0;
        public int BuildNumberYANDEX = 0;
        public int BuildNumberFB = 0;


        public static class ServerConstants
        {
            public const string ServerCommonPath = "https://m3decor.net/v3/server_common/";
        }
    }
}