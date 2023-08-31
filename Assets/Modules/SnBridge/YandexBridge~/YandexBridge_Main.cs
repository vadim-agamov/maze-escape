using System.Runtime.InteropServices;
using Common.Localizations;
using UnityEngine;

public partial class YandexBridge : SNBridgeBase
{
    public override string GetBundlesRootFolderURL()
    {
        return YANDEXConstants.BundlesFolderPath;
    }

    public override void ShowRateUs()
    {
        ShowRateUsPopup();
    }
    
    public override string GetUserId()
    {
        return _userId;
    }
    
    public override Language GetLanguage()
    {
        return _lang;
    }
    public void OnClose() 
    {
        Debug.LogWarning("Unity: Browser tab closed");
    }

    public override void Dispose()
    {
    }
    
    [DllImport("__Internal")]
    private static extern void ShowRateUsPopup();
  
}
