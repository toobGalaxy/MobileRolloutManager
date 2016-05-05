#if __UNIFIED__
using UIKit;
using CoreGraphics;
#else
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using CGRect = System.Drawing.RectangleF;
#endif

using SatelliteMenu;

namespace SatelliteMenuSample
{
	public class MainViewController : UIViewController
	{
		const int BUTTON_SIZE = 44;
		const int MARGIN = 10;
		SatelliteMenuButton menu;
	
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			View.BackgroundColor = UIColor.Gray;

			// create the menu button
			var frame = new CGRect (MARGIN, View.Frame.Height - BUTTON_SIZE - MARGIN, BUTTON_SIZE, BUTTON_SIZE);
			menu = new SatelliteMenuButton (View, UIImage.FromBundle ("Img/menu.png"), frame);
			menu.MenuItemClick += (_, args) => {
				new UIAlertView ("", "Selected item: " + args.MenuItem.Name, null, "OK", null).Show ();
			};
			// add the items
			menu.AddItems(new []{ 
				new SatelliteMenuButtonItem (UIImage.FromBundle ("Img/icon1.png"), "Search"), 
				new SatelliteMenuButtonItem (UIImage.FromBundle ("Img/icon2.png"), "Tag"),
				new SatelliteMenuButtonItem (UIImage.FromBundle ("Img/icon3.png"), "Upload"),
				new SatelliteMenuButtonItem (UIImage.FromBundle ("Img/icon4.png"), "Locate"),
				new SatelliteMenuButtonItem (UIImage.FromBundle ("Img/icon5.png"), "Magic"),
				new SatelliteMenuButtonItem (UIImage.FromBundle ("Img/icon6.png"), "Refresh")
			});
			// add to the view
			View.Add (menu);
		}

		public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate (fromInterfaceOrientation);
			var frame = new CGRect (MARGIN, View.Frame.Height - BUTTON_SIZE - MARGIN, BUTTON_SIZE, BUTTON_SIZE);
			menu.Frame = frame;
		}
	}
}
	