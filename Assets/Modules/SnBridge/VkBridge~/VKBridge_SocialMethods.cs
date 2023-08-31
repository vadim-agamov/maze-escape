using System.Runtime.InteropServices;

public partial class VKBridge : SNBridgeBase
{
	[DllImport("__Internal")]
	public static extern void ShowSettingsBoxQuickLabel();

	[DllImport("__Internal")]
	public static extern void ShowInviteFriends();
}
