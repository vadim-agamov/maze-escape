using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Utils;

public partial class VKBridge : SNBridgeBase
{
	public override async UniTask Initialize()
	{
		_language = LanguageUtils.GetSystemLanguage();
		
		Init();
		
		await UniTask.WaitUntil(() => _userId != null);
	}

	public void OnVkInited(string userId)
	{
		_userId = userId;
		Debug.Log("_onVkInitSuccess: " + userId + " gameObject: " + gameObject.name + " userId " + _userId);
	}

	public void onFail()
	{
		Debug.Log("_onVkFailed");
		Init();
	}
	
	//dll
	[DllImport("__Internal")]
	public static extern string Init();
}
