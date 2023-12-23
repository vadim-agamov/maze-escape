using System;
using System.Runtime.InteropServices;
using AOT;
using Modules.Events;
using UnityEngine;

namespace Modules.PlatformService
{
    public struct AppFocusState
    {
        public bool IsFocus;
    }
    
    public static class AppFocusHandle
    {
        [DllImport("__Internal")]
        private static extern void FocusAppHandleInit(Action focus, Action unFocus);

        private static bool _isInitialize = false;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (_isInitialize)
            {
                return;
            }

            _isInitialize = true;

            if (Application.isEditor || Application.platform != RuntimePlatform.WebGLPlayer)
            {
                return;
            }
            
            FocusAppHandleInit(Focus, UnFocus);
        }

        [MonoPInvokeCallback(typeof(Action))]
        private static void Focus()
        {
            Debug.Log($"[{nameof(AppFocusHandle)}] focus");
            Event<AppFocusState>.Publish(new AppFocusState {IsFocus = true});
        }

        [MonoPInvokeCallback(typeof(Action))]
        private static void UnFocus()
        {
            Debug.Log($"[{nameof(AppFocusHandle)}] unfocus");
            Event<AppFocusState>.Publish(new AppFocusState {IsFocus = false});
        }
    }
}