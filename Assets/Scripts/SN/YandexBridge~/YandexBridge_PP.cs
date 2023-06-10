using Cysharp.Threading.Tasks;
using PP;
using SN.HostingerUtils;
using UnityEngine;

public partial class YandexBridge : SNBridgeBase
{
    public override async UniTask<PlayerProgress> LoadPP()
    {
        if (!_initialized)
        {
            Debug.LogError("Trying to load PP before initialize YandexBridge");
            await Initialize();
        }

        return await HostingerUtils.LoadPP(YANDEXConstants.ServerFolderPath, _userId);
    }

    public override async UniTask<bool> SavePP(PlayerProgress pp)
    {
        return await HostingerUtils.SavePP(YANDEXConstants.ServerFolderPath, pp.UserId, pp);
    }

}


