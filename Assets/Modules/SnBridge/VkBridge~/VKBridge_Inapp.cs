using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Utils;

public partial class VKBridge : SNBridgeBase
{
	private  Action _onOrderSuccess;
	private    Action _onOrderFail;

	public override void Purchase(string productId, Action onSuccess, Action onOrderFail)
	{
		Debug.Log("Purchase: _userId " + _userId + " gameObject: " + gameObject.name + " _userId: " + _userId);

		_onOrderSuccess = onSuccess;
		_onOrderFail = onOrderFail;
		
		ShowOrderBox(productId);
	}

	public override string GetPriceOfProduct(string productId)
	{
		return "777";
	}

	public void onOrderSuccessFromJs(string orderId)
	{
		Debug.Log("onOrderSuccessFromJs" + orderId + " -> checking status: " + " _userId: " + _userId);

		CheckOrderSucceed(orderId).Forget();
		
	}
	
	private async UniTask CheckOrderSucceed(string orderId)
	{
		Debug.LogWarning(" _userId: " + _userId) ;
		var webReq = UnityWebRequest.Get(VKConstants.ServerFolderPath + "check_order_status.php?id=" + _userId);
		Debug.LogWarning("webReq url: " + webReq.uri) ;

		try
		{
			var op = await webReq.SendWebRequest();
			
			if (!string.IsNullOrEmpty(op.error))
			{
				Debug.LogError("Checking Order failed: " + op.error);
				await CheckOrderSucceed(orderId);
				return;
			}
			
			var data = op.downloadHandler.data.RemoveBOM();
			var dataText = Encoding.UTF8.GetString(data);
			Debug.LogWarning("data: " + data.Length + " datatext: " + dataText) ;

			var succeed = !string.IsNullOrEmpty(dataText) && dataText.Split(' ').Any(s => s == orderId);
			Debug.LogWarning("data: " + data + " succeed: " + succeed) ;

			if (succeed)
			{
				_onOrderSuccess.Invoke();
			}
			else
			{
				_onOrderFail.Invoke();
			}
		}
		catch (Exception e)
		{
			Debug.LogError("Checking Order failed: " + e.Message + e.Data);
			await CheckOrderSucceed(orderId);
			return;
		}
	}

	public void onOrderFailFromJs(string errorCode )
	{
		Debug.Log("onOrderFailFromJs: " +errorCode );
		_onOrderFail.Invoke();
	}
	
	public void onOrderCancelFromJs()
	{
		Debug.Log("onOrderCancelFromJs");
		_onOrderFail.Invoke();
	}
	
	public override IEnumerable<string> GetProductsIds() => Array.Empty<string>();

	//dll
	[DllImport("__Internal")]
	public static extern string ShowOrderBox(string itemId);
}
