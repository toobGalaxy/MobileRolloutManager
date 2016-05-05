`SatelliteMenu` is an unobtrusive button that smoothly expands to a radial 'satellite' menu when tapped. This control was popularized in Path's iOS app.

## iOS Example

To setup a `SatelliteMenu` on iOS (be sure to add your own menu image resources first):

```csharp
using SatelliteMenu;
using System.Drawing;
...

public override void ViewDidLoad ()
{
	base.ViewDidLoad ();

	var image = UIImage.FromFile ("menu.png");
	var yPos = View.Frame.Height - image.Size.Height - 10;
	var frame = new RectangleF (10, yPos, image.Size.Width, image.Size.Height);

	var items = new [] { 
		new SatelliteMenuButtonItem (UIImage.FromFile ("icon1.png"), 1, "Search"),
		new SatelliteMenuButtonItem (UIImage.FromFile ("icon2.png"), 2, "Update"),
		new SatelliteMenuButtonItem (UIImage.FromFile ("icon3.png"), 3, "Share"),
		new SatelliteMenuButtonItem (UIImage.FromFile ("icon4.png"), 4, "Post"),
		new SatelliteMenuButtonItem (UIImage.FromFile ("icon5.png"), 5, "Reload"),
		new SatelliteMenuButtonItem (UIImage.FromFile ("icon6.png"), 6, "Settingd")
	};

	var button = new SatelliteMenuButton (View, image, items, frame);

	button.MenuItemClick += (_, args) => {
		Console.WriteLine ("{0} was clicked!", args.MenuItem.Name);
	};

	View.AddSubview (button);
}
```

## Android Example

To setup a `SatelliteMenu` on Android (be sure to add your own menu image resources first):

```csharp
using SatelliteMenu;
...

protected override void OnCreate (Bundle bundle)
{
    base.OnCreate (bundle);
    SetContentView (Resource.Layout.Main);

    var button = FindViewById<SatelliteMenuButton> (Resource.Id.menu);

    button.AddItems (new [] {
        new SatelliteMenuButtonItem (1, Resource.Drawable.icon1),
        new SatelliteMenuButtonItem (2, Resource.Drawable.icon2),
        new SatelliteMenuButtonItem (3, Resource.Drawable.icon3),
        new SatelliteMenuButtonItem (4, Resource.Drawable.icon4),
        new SatelliteMenuButtonItem (5, Resource.Drawable.icon5),
        new SatelliteMenuButtonItem (6, Resource.Drawable.icon6)
    });

    button.MenuItemClick += (sender, e) => {
        Console.WriteLine ("{0} selected", e.MenuItem);
    };
}
```

And in the layot file

```xml
...
<satellitemenu.SatelliteMenuButton
    pop:speed="250"
    pop:radius="200dp"
    android:id="@+id/menu"
    android:layout_width="wrap_content"
    android:layout_height="wrap_content"
    android:layout_gravity="bottom|left"
    android:layout_margin="8dp">
    <ImageView
        android:id="@+id/popoutMenu"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:src="@drawable/menu"
        android:layout_gravity="bottom|left"
        android:contentDescription="@string/empty" />
</satellitemenu.SatelliteMenuButton>
...
```

## Customization

`SatelliteMenuButton` has several customizable properties:

* `Radius`: distance of button items from the main menu button.
* `Speed`: speed of the expand/collapse animation.
* `Bounce`: the value of bounce effect.
* `BounceSpeed`: bounce speed.
* `RotateToSide`: whether to rotate to side when pressed. Useful for round menu buttons--for example, "+" becomes "x" when opened.
* `CloseItemsOnClick`: whether to close items when clicked.
