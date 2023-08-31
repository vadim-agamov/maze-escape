using Cysharp.Threading.Tasks;
using PP;
using SN.HostingerUtils;

public partial class VKBridge : SNBridgeBase
{
	public override async UniTask<PlayerProgress> LoadPP()
	{
		return await HostingerUtils.LoadPP(VKConstants.ServerFolderPath, _userId);	        
	}

	public override async UniTask<bool> SavePP(PlayerProgress pp)
	{
		return await HostingerUtils.SavePP(VKConstants.ServerFolderPath, pp.UserId, pp);
	}
}
