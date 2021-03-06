﻿using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Java.IO;

using Nutiteq.Ui;
using Nutiteq.Utils;
using Nutiteq.Layers;
using HelloMap;

namespace NutiteqSample
{
	[Activity (Label = "Hellomap", MainLauncher = true, Icon = "@drawable/icon")]
	public class BaseMapActivity : Activity
	{
		protected MapView mapView;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Register license
			Nutiteq.Utils.Log.ShowError = true;
			Nutiteq.Utils.Log.ShowWarn = true;
			MapView.RegisterLicense("XTUMwQ0ZEeFpTU0piNCs0M05lcHM2eVdKSFd3SWNQdHBBaFVBbzk0K1VuUjNMWG9vc1JsOGthbHNBYjJ5RnBVPQoKcHJvZHVjdHM9c2RrLXhhbWFyaW4tYW5kcm9pZC0zLioKcGFja2FnZU5hbWU9Y29tLm51dGl0ZXEuaGVsbG9tYXAueGFtYXJpbgp3YXRlcm1hcms9bnV0aXRlcQp1c2VyS2V5PTE1Y2Q5MTMxMDcyZDZkZjY4YjhhNTRmZWRhNWIwNDk2Cg==", ApplicationContext);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			mapView = (MapView)FindViewById (Resource.Id.mapView);
		}
	}
}
