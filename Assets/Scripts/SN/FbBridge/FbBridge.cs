using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Services;
using UnityEngine;

namespace SN.FbBridge
{
    public partial class FbBridge : MonoBehaviour, ISnBridge
    {
        [DllImport("__Internal")]
        private static extern string FbGetUserId();
        
        [DllImport("__Internal")]
        private static extern void FBGetData();
    
        [DllImport("__Internal")]
        private static extern void FBSetData(string data);

        private string _userId;

        private UniTaskCompletionSource<PlayerData> _loadPlayerProgressCompletionSource;

        UniTask ISnBridge.Initialize()
        {
            _userId = FbGetUserId();
            FbPreloadInterstitial();
            FbPreloadRewarded();
            return UniTask.CompletedTask;
        }

        string ISnBridge.GetUserId()
        {
            return FbGetUserId();
        }

        #region Player Progress

        UniTask<PlayerData> ISnBridge.LoadPlayerProgress()
        {
            _loadPlayerProgressCompletionSource = new UniTaskCompletionSource<PlayerData>();
            FBGetData();
            return _loadPlayerProgressCompletionSource.Task;
        }

        public void OnPlayerProgressLoaded(string dataStr)
        {
            Debug.Log($"FbBridge: OnPlayerProgressLoaded: {dataStr}");
            var data = JsonConvert.DeserializeObject<PlayerData>(dataStr) ?? new PlayerData();
            _loadPlayerProgressCompletionSource.TrySetResult(data);
            _loadPlayerProgressCompletionSource = null;
        }

        UniTask ISnBridge.SavePlayerProgress(PlayerData data)
        {
            var dataStr = JsonConvert.SerializeObject(data); 
            Debug.Log($"FbBridge: SavePlayerProgress: {dataStr}");
            FBSetData(dataStr);
            return UniTask.CompletedTask;
        }        

        #endregion

        
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