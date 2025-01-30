using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if ANDROID
using Android.Content;
using Android.Views;
using Android.Runtime;
#elif IOS
using UIKit;
using Foundation;
#endif

namespace CLIMOV_BANQUITO_RESTFULL_DOTNET
{
    public class DeviceOrientationService
    {

        public void SeOrientation(DisplayOrientation orientation)
        {
#if ANDROID
            if (DisplayOrientation.Landscape == orientation)
            {
                Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;

            }
            else
            {
                Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;

            }


#elif IOS
            if (UIDevice.CurrentDevice.CheckSystemVersion(16, 0))
            {
                var windowScene = (UIApplication.SharedApplication.ConnectedScenes.ToArray()[0] as UIWindowScene);
                if (windowScene != null)
                {
                    var nav = UIApplication.SharedApplication.KeyWindow?.RootViewController;
                    if (nav != null)
                    {
                        // Tell the os that we changed orientations so it knows to call GetSupportedInterfaceOrientations again
                        nav.SetNeedsUpdateOfSupportedInterfaceOrientations();
                        if (DisplayOrientation.Landscape == orientation)
                        {
                            windowScene.RequestGeometryUpdate(new UIWindowSceneGeometryPreferencesIOS(UIInterfaceOrientationMask.LandscapeRight), error => { });
                        }
                        else
                        {
                            windowScene.RequestGeometryUpdate(new UIWindowSceneGeometryPreferencesIOS(UIInterfaceOrientationMask.Portrait), error => { });
                        }
                    }
                }
            }
            else
            {
                if (DisplayOrientation.Landscape == orientation)
                {
                    UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)UIInterfaceOrientation.LandscapeRight), new NSString("orientation"));
                }
                else
                {
                    UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)UIInterfaceOrientation.Portrait), new NSString("orientation"));
                }
            }
#endif
        }
    }
}
