using System;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Utils;

public struct OnInitData 
{
    public string id;
    public string lang;

  
}

public partial class YandexBridge : SNBridgeBase
{
    private bool _initialized = false;  

    public override async UniTask Initialize()
    {
        InitializeYSDK();

        await UniTask.WaitUntil(() => _initialized);
    }

    public void OnInitializedSuccess(string data)
    {
        Debug.LogWarning("YandexBridge OnInitializedSuccess: "+data);

        var onInitData = JsonUtility.FromJson<OnInitData>(data);
        var userId = onInitData.id;
        _lang = LanguageUtils.String_to_Iso639_1(onInitData.lang);
        Debug.LogWarning("YandexBridge OnInitializedSuccess: userId: "+userId + " lang: " + _lang);

        if (!string.IsNullOrEmpty(userId))
        {
            _userId = userId;
        }
        else
        {
            if (PlayerPrefs.HasKey("userId"))
            {
                _userId = PlayerPrefs.GetString("userId");
                Debug.LogWarning("YandexBridge Initialize: PlayerPrefs.HasKey(userId) " + _userId);

            }
            else
            {
                _userId = Guid.NewGuid().ToString();

                Debug.LogWarning("YandexBridge Initialize:generated total new hash: " + _userId );
            }
        }
        
        PlayerPrefs.SetString("userId", _userId);
        PlayerPrefs.Save();
        
        _initialized = true;
    }
    
    //dll
    [DllImport("__Internal")]
    private static extern void InitializeYSDK();

}