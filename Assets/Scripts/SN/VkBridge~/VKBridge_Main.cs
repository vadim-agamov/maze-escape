using Common.Localizations;
public partial class VKBridge : SNBridgeBase
{
	private Language _language;
	public override Language GetLanguage()
	{
		return _language;
	}

	public override void ShowRateUs()
	{
	}

	public override string GetBundlesRootFolderURL()
	{
		return VKConstants.BundlesFolderPath;
	}

	public override string GetUserId()
	{
		return _userId;
	}

	public override void Dispose()
	{
	}
}
