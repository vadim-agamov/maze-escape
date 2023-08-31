using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace Modules.SnBridge.FbBridge
{
    // https://docs.unity3d.com/Manual/webgl-interactingwithbrowserscripting.html
    public partial class FbBridge : MonoBehaviour, ISnBridge
    {
        [DllImport("__Internal")]
        private static extern string FbGetUserId();
        
        [DllImport("__Internal")]
        private static extern void FBGetData();
    
        [DllImport("__Internal")]
        private static extern void FBSetData(string data);
        
        [DllImport("__Internal")]
        private static extern void FBLogEvent(string eventName);
        
        private ISnBridge This => this;
        
        private UniTaskCompletionSource<string> _loadPlayerProgressCompletionSource;

        void ISnBridge.Initialize()
        {
            FbPreloadInterstitial();
            FbPreloadRewarded();
        }

        string ISnBridge.GetUserId() => FbGetUserId();

        #region Player Progress

        UniTask<string> ISnBridge.LoadPlayerProgress()
        {
            _loadPlayerProgressCompletionSource = new UniTaskCompletionSource<string>();
            FBGetData();
            return _loadPlayerProgressCompletionSource.Task;
        }

        [UsedImplicitly]
        public void OnPlayerProgressLoaded(string dataStr)
        {
            Debug.Log($"FbBridge: OnPlayerProgressLoaded: {dataStr}");
            _loadPlayerProgressCompletionSource.TrySetResult(dataStr);
            _loadPlayerProgressCompletionSource = null;
        }

        UniTask ISnBridge.SavePlayerProgress(string data)
        {
            Debug.Log($"FbBridge: SavePlayerProgress: {data}");
            FBSetData(data);
            return UniTask.CompletedTask;
        }

        #endregion

        public void LogEvent(string eventName, Dictionary<string, object> _)
        {
            Debug.Log($"FbBridge: LogEvent: {eventName}");
            FBLogEvent(eventName);
        }
        
        // public override void Purchase(string productId, Action onSuccess, Action onOrderFail)
        // {
        //     throw new NotImplementedException();
        // }
    
        // public void OnPaymentsInited(string productsData)
        // {
        //     Debug.LogWarning("OnPaymentsInited: " + productsData);
        //     var fbProducts = JsonUtility.FromJson<FbProduct[]>(productsData);
        //     _products = fbProducts;
        //
        //     Debug.LogWarning("OnPaymentsInited parsed: " + _products.Length);
        //
        //     //ReopenBank().Forget();
        // }

        // public override IEnumerable<string> GetProductsIds()
        // {
        //     return _products.Select(x => x.id);
        // }
        //
        // public override string GetPriceOfProduct(string productId)
        // {
        //     if (_products?.Length == 0)
        //     {
        //         Debug.LogWarning("Trying to get product's price before Products Inited");
        //         return "";
        //     }
        //
        //     var product = _products.FirstOrDefault(p => p.id == productId);
        //
        //     if(product!= null)
        //     {
        //         return product.price;
        //     }
        //
        //     Debug.LogError("Price not found for product: " + productId);
        //     return "";
        // }

        // public override Language GetLanguage()
        // {
        //     throw new NotImplementedException();
        // }

        // public override void ShowRateUs()
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public override string GetBundlesRootFolderURL()
        // {
        //     throw new NotImplementedException();
        // }
    }
}