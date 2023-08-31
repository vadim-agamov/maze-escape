using System.Runtime.InteropServices;

public partial class YandexBridge : SNBridgeBase
{
    public void Authenticate() 
    {
        AuthenticateUser();
    }

    public void OnAuthOk()
    {
        //GlobalEventManager.Publish(new YandexAuthOkEvent());
    }
    
    public void OnAuthCanceled()
    {
        //GlobalEventManager.Publish(new YandexAuthCanceledEvent());
        
        //ReopenBank().Forget();

    }
    
    //dll
    
    [DllImport("__Internal")]
    private static extern void AuthenticateUser();
}

