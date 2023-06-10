using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using Utils;

public partial class YandexBridge : SNBridgeBase
{
    private Action onPurchaseSuccessCallback;
    private  Action onPurchaseFailedCallback;

    private YandexProductData[] _productsData = new YandexProductData[0]{};
   
    public void OnPaymentsInited(string productsData)
    {
        Debug.LogWarning("OnPaymentsInited: " + productsData);
        var productsDataRoot = JsonUtility.FromJson<YandexProductRoot>("{\"products\":" + productsData + "}");
        _productsData = productsDataRoot.products;
        
        Debug.LogWarning("OnPaymentsInited parsed: " + _productsData.Length);

        //ReopenBank().Forget();
    }
    
    /*private async UniTask ReopenBank()
    {
        var ui = GameSession.UiManager.GetUi<BankUI>();
        if (ui != null)
        {
            ui.CloseUI();
            GlobalEventManager.Publish(new ShowBlockUIEvent(false));

            await UniTask.WaitUntil(() => UiManager.GetUI<BankUI>() == null);

            GlobalEventManager.Publish(new OpenBankClickedEvent());
        }
    }*/

    public override string GetPriceOfProduct(string productId)
    {
        if (_productsData?.Length == 0)
        {
            Debug.LogWarning("Trying to get product's price before Products Inited");
            return "";
        }
        
        var product = _productsData.FirstOrDefault(p => p.id == productId);
        
        if(product!= null)
        {
            return product.price;
        }
        
        Debug.LogError("Price not found for product: " + productId);
        return "";
    }


    public override void Purchase(string productId, Action onSuccess, Action onOrderFail)
    {
        onPurchaseSuccessCallback = onSuccess;
        onPurchaseFailedCallback = onOrderFail;
        
        Purchase(productId);
    }
    
    // Callbacks from index.html
    public void OnPurchaseSuccess(string id) {
        
        onPurchaseSuccessCallback.InvokeSafe();

        onPurchaseSuccessCallback = null;
        onPurchaseFailedCallback = null;
    }
    public void OnPurchaseFailed(string error)
    {
        Debug.LogWarning("Unity: OnPurchaseFailed: " + error);
        onPurchaseFailedCallback.InvokeSafe();
        
        onPurchaseSuccessCallback = null;
        onPurchaseFailedCallback = null;
    }

    public override IEnumerable<string> GetProductsIds() => Array.Empty<string>();
    
//dll
    [DllImport("__Internal")]
    private static extern void InitPurchases();
    
    [DllImport("__Internal")]
    private static extern void Purchase(string id);
}

[Serializable]
public class YandexProductRoot
{
    public YandexProductData[] products;

}
[Serializable]
public class YandexProductData
{
    public string id;
    public string title;
    public string description;
    public string imageURI;
    public string price;
    public string priceValue;
    public string priceCurrencyCode;
}

