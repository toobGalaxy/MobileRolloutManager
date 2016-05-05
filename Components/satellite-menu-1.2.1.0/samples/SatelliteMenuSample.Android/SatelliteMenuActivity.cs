using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

using System;
using System.Collections.Generic;

using SatelliteMenu;

namespace SatelliteMenuSample
{
	[Activity(Label = "Satellite Menu", MainLauncher = true)]
	public class MainActivity : Activity
	{
		protected override void OnCreate(Bundle bundle)
		{
			Window.RequestFeature(WindowFeatures.NoTitle);

			base.OnCreate(bundle);

			// load the main layout, it is a gray background with frame layout and out menu control on it, please note in the axml how we change the default speed of the animation from 400ms to 250ms with the markup
			SetContentView(Resource.Layout.Main);

			// get the actual menu object from the layout
			var menu = FindViewById<SatelliteMenuButton>(Resource.Id.menu);

			// register for the menu item selection event here
			menu.MenuItemClick += delegate(object sender, SatelliteMenuItemEventArgs e)
			{
				// parse the enum value from int back to the enum here
				MenuItemType mit = (MenuItemType) Enum.Parse(typeof (MenuItemType), e.MenuItem.Tag.ToString());

				// just show the menu item selected toast, in the app we would probably fire new activity or similar
				Toast.MakeText(this, string.Format("Menu item selected: {0}", mit), ToastLength.Short).Show();
			};

			// array of items
			var items = new List<SatelliteMenuButtonItem>();

			// just add one by one
			items.Add(new SatelliteMenuButtonItem((int) MenuItemType.Search, Resource.Drawable.icon1));
			items.Add(new SatelliteMenuButtonItem((int) MenuItemType.NewEntry, Resource.Drawable.icon2));
			items.Add(new SatelliteMenuButtonItem((int) MenuItemType.Share, Resource.Drawable.icon3));
			items.Add(new SatelliteMenuButtonItem((int) MenuItemType.Locate, Resource.Drawable.icon4));
			items.Add(new SatelliteMenuButtonItem((int) MenuItemType.Wizard, Resource.Drawable.icon5));
			items.Add(new SatelliteMenuButtonItem((int) MenuItemType.Reload, Resource.Drawable.icon6));

			// now add all to the menus
			menu.AddItems(items.ToArray());
		}
	}
}
