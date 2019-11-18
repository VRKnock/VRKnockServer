using SteamVR_HUDCenter.Elements;
using Valve.VR;

namespace KnockServer
{
    public class TestOverlay : Overlay
    {
        
        public TestOverlay(string FriendlyName, float Width, VROverlayInputMethod InputMethod = VROverlayInputMethod.None) : base(FriendlyName, Width, InputMethod)
        {
        }

        public TestOverlay(string FriendlyName, string ThumbnailPath, float Width, VROverlayInputMethod InputMethod = VROverlayInputMethod.None) : base(FriendlyName, ThumbnailPath, Width, InputMethod)
        {
        }

        public void showMessage(string message)
        {
            Controller.DisplayNotification(message, this,EVRNotificationType.Transient, EVRNotificationStyle.Contact_Active,new NotificationBitmap_t());
        }
    }
}