using System;
using System.Collections.Generic;

using Android.Widget;

using System.Threading.Tasks;
using Newtonsoft.Json;

using Android.Util;
using RestSharp;

using RestSharp.Authenticators;
using Android.App;
using Android.OS;
using Android.Gms.Maps;
using Android.Content;
using Android.Gms.Maps.Model;
using System.Globalization;
using Android.Locations;
using System.Threading;
using ZXing.Mobile;
using Android.Graphics;
using System.IO;
using Media.Plugin;
using Media.Plugin.Abstractions;
using Android.Views;
using SignaturePad;
using Java.Nio;
using System.Text.RegularExpressions;

namespace MobileRolloutManager
{
    [Activity(Label = "LoadSites")]
    public class LoadSites : Activity, IOnMapReadyCallback, ILocationListener
    {
        private string UserId = Constants.UserIdd;
        private ListView lis;
        private ListView lis2;
        private ListView lis3;
        private Button buttonAll;
        private List<SiteList> items = new List<SiteList>();
        private List<Sites> Siteitems = new List<Sites>();
        private Button btn_map;
        private Button ASiteBack;
        private Button mapBack;
        private int disable;
        private int selectSite = 0;
        private GoogleMap _map;
        private LatLng DirectionsLat;
        private Sites directi;
        private LocationManager _locMan;
        private Location MyLocation;
        private Button btnDirections;
        System.Drawing.PointF[] points;
        string signedName = null;
        string signedInstatution = null;
        List<Bitmap> signatures = new List<Bitmap>();
        List<string> signedNames = new List<string>();
        List<string> instNames = new List<string>();

        List<Bitmap> Attendencesignatures = new List<Bitmap>();
        List<string> AttendsignedNames = new List<string>();
        List<string> AttendDesig = new List<string>();
        List<string> AttendNumbers = new List<string>();
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
     
         
            var progressDialog = ProgressDialog.Show(this, "", "Loading Sites...", true);
            progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            progressDialog.Indeterminate = true;
            _locMan = GetSystemService(Context.LocationService) as LocationManager;
            //You can now use and reference the ActionBar

            //Do whatever  
            await load();
            
            progressDialog.Dismiss();
          

            // Create your application here
        }

        public void OnLocationChanged(Location location)
        {
            
            new Thread(new ThreadStart(() => {
                var geocdr = new Geocoder(this);
                var addresses = geocdr.GetFromLocation(location.Latitude, location.Longitude, 5);

                RunOnUiThread(() => {
                    MyLocation = location;
                });

            })).Start();
        }

        public void OnProviderDisabled(string provider)
        {
        }

        public void OnProviderEnabled(string provider)
        {
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
        }

        private void SetUpMapIfNeeded()
        {
            if (null != _map) return;

            var frag = FragmentManager.FindFragmentById<MapFragment>(Resource.Id.mapF);
            frag.GetMapAsync(this);         

        }

        public async void OnMapReady(GoogleMap _map2)
        {
             _map = _map2;
            if (_map != null)
            {
                _map.TrafficEnabled = true;
                _map.UiSettings.CompassEnabled = true;
                _map.UiSettings.MyLocationButtonEnabled = true;
                _map.UiSettings.SetAllGesturesEnabled(true);
                _map.MyLocationEnabled = true;
                _map.UiSettings.ZoomGesturesEnabled = true;
                DirectionsLat = new LatLng(double.Parse(directi.Latitude, CultureInfo.InvariantCulture), double.Parse(directi.Longitude, CultureInfo.InvariantCulture));
                LatLngBounds.Builder builder = new LatLngBounds.Builder();
                MarkerOptions marker = new MarkerOptions();
                marker.SetPosition(DirectionsLat);
                marker.SetTitle(directi.Site);
                marker.SetSnippet("This is the site you want to go to. Click on device gps to start Navigation");
                marker.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed));
                _map.AddMarker(marker);
                builder.Include(DirectionsLat);
                LatLng Mypos;
                if (MyLocation == null) {  Mypos = new LatLng(_map.MyLocation.Latitude, _map.MyLocation.Longitude); }
                else
                {
                     Mypos = new LatLng(MyLocation.Latitude, MyLocation.Longitude);
                }
                MarkerOptions MyLocationM = new MarkerOptions();
                MyLocationM.SetPosition(Mypos);
                
                MyLocationM.SetTitle("You are here");
                MyLocationM.SetSnippet("This is your location!");
                MyLocationM.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueCyan));
                _map.AddMarker(MyLocationM);
                
                builder.Include(Mypos);
                
                LatLngBounds bounds = builder.Build();
                
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngBounds(bounds, 100);
                _map.MoveCamera(cameraUpdate);
            }
        }
        
        protected override void OnResume()
        {
            base.OnResume();
            var locationCriteria = new Criteria();
            locationCriteria.Accuracy = Accuracy.NoRequirement;
            locationCriteria.PowerRequirement = Power.NoRequirement;
            string locationProvider = _locMan.GetBestProvider(locationCriteria, true);
            _locMan.RequestLocationUpdates(locationProvider, 2000, 1, this);
        }

        private async Task loadMap()
        {
            try
            {
                Intent intent = new Intent(this, typeof(ActionView)); // need to set your Intent View here
                
                StartActivity(intent);
           
            }
            catch (Exception ex)
            {


                Log.Info("Debug Adapters", @"				ERROR {0}", ex.Message);

                Console.WriteLine(@"				ERROR {0}", ex.Message);

            }
        }

        private async Task load() {

            try
            {
                disable = 0;
                SetContentView(Resource.Layout.Sites);
                ActionBar.Title = "My Sites";
                lis = FindViewById<ListView>(Resource.Id.listing);
                lis2 = FindViewById<ListView>(Resource.Id.listing22);
                btn_map = FindViewById<Button>(Resource.Id.map);
                buttonAll = FindViewById<Button>(Resource.Id.btn_allSites);
                mapBack = FindViewById<Button>(Resource.Id.mapback);

                buttonAll.Click += async (sender, e) =>
                {

                    SetContentView(Resource.Layout.AllSites);
                    await loadAllSites();

                };

                btn_map.Click += async (sender, e) =>
                {
                    await loadMap();

                };
                items = new List<SiteList>();

                List<SiteList> listitems = await FetchSitesAsync(Constants.UserIdd);
                List<string> AdapterList = new List<string>();
                if (listitems.Count > 0) {
                    disable += 1;
                    foreach (var itemL in listitems) {
                        AdapterList.Add("Id " + itemL.EmisNumber + " - " + itemL.SiteName);
                    }
                ArrayAdapter adapters = new ArrayAdapter(
                                    this, //Context, typically the Activity
                                    Android.Resource.Layout.SimpleListItem1, //The layout. How the data will be presented 
                                    AdapterList //The enumerable data
                                );
                    lis.Adapter = adapters;
                    lis.ItemClick += async (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
                    {
                        selectSite = listitems[e.Position].Id;
                        await loadSite(selectSite.ToString());
                    };
                }

                items = new List<SiteList>();
                disable += 1;
                List<SiteList> listitems2 = await FetchIssueSitesAsync(Constants.UserIdd);
                List<string> AdapterList2 = new List<string>();
                if (listitems2.Count > 0)
                {
                    disable += 1;
                    foreach (var itemL in listitems2)
                    {
                        AdapterList2.Add("Id " + itemL.EmisNumber + " - " + itemL.SiteName);
                    }
                    ArrayAdapter adapters2 = new ArrayAdapter(
                                        this, //Context, typically the Activity
                                        Android.Resource.Layout.SimpleListItem1, //The layout. How the data will be presented 
                                        AdapterList2 //The enumerable data
                                    );
                    lis2.Adapter = adapters2;
                    lis2.ItemClick += async (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
                    {
                        selectSite = listitems2[e.Position].Id;
                        await loadSite(selectSite.ToString());
                    };
                }


                if (disable == 0) { btn_map.Activated = false; }
        
            }
            catch (Exception ex)
            {


                Log.Info("Debug Adapters", @"				ERROR {0}", ex.Message);

                Console.WriteLine(@"				ERROR {0}", ex.Message);
            

            }
            // lis.SetAdapter(new ArrayAdapter(this, Resource.Id.list, listitems));
            //  lis.Adapter = 

        }

        private async Task loadAllSites()
        {
          
            try
            {
            
                    var progressDialog = ProgressDialog.Show(this, "", "Loading All Your Sites...", true);
                    progressDialog.Indeterminate = true;
                    progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                    SetContentView(Resource.Layout.AllSites);
                    ASiteBack = FindViewById<Button>(Resource.Id.allsitesBack);

                    ASiteBack.Click += async (sender, e) =>
                  
                    {
                        await load();
                       // SetContentView(Resource.Layout.Sites);
                   

                    };

                    lis3 = FindViewById<ListView>(Resource.Id.list3);
                    ActionBar.Title = "All My Sites";
                    List<SiteList> listitems = await FetchAllSitesAsync(Constants.UserIdd);
                    List<string> AdapterList = new List<string>();
                    ArrayAdapter adapter = new ArrayAdapter(
                                        this, //Context, typically the Activity
                                        Android.Resource.Layout.SimpleListItem1, //The layout. How the data will be presented 
                                        listitems //The enumerable data
                                    );
                    lis3.Adapter = adapter;
                lis3.ItemClick += async (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
                {
                    selectSite = listitems[e.Position].Id;
                    await loadSite(selectSite.ToString());
                };

                progressDialog.Dismiss();
                    // lis.SetAdapter(new ArrayAdapter(this, Resource.Id.list, listitems));
                    //  lis.Adapter = 
             }catch (System.Exception ex)
            {


                Log.Info("FetchSitesAsync", @"				ERROR {0}", ex.Message);

                Console.WriteLine(@"				ERROR {0}", ex.Message);
              
            }

          

        }

        public async Task LoadDirections()
        {

            SetContentView(Resource.Layout.Direction);
            ASiteBack = FindViewById<Button>(Resource.Id.allsitesBack3);
            btnDirections = FindViewById<Button>(Resource.Id.btngps);
            ASiteBack.Click += async (sender, e) =>
            {
                await loadSite(directi.Ids.ToString());
                // SetContentView(Resource.Layout.Sites);

            };

            btnDirections.Click += (sender, e) =>

            {
                Android.Net.Uri gmmIntentUri = Android.Net.Uri.Parse("google.navigation:q=" + directi.Latitude + "," + directi.Longitude + "");
                Intent mapIntent = new Intent(Intent.ActionView, gmmIntentUri);
                mapIntent.SetFlags(ActivityFlags.BroughtToFront);
                mapIntent.SetFlags(ActivityFlags.NewTask);
                mapIntent.SetPackage("com.google.android.apps.maps");
                StartActivity(mapIntent);

            };


            SetUpMapIfNeeded();




        }

        private async Task LoadInstall(string ids, string SiteNames)
        {
            SetContentView(Resource.Layout.Installation);
            ActionBar.Title = "Start Installation Process";
            TextView SN = FindViewById<TextView>(Resource.Id.SiteName);
            SN.Text = SiteNames;
          

            Button Back = FindViewById<Button>(Resource.Id.InstBack);
            Back.Click += async (sender, e) =>

            {
                await loadSite(ids);
                // SetContentView(Resource.Layout.Sites);

            };
            Button AddAsset = FindViewById<Button>(Resource.Id.Assets);
            AddAsset.Click += async (sender, e) =>

            {
                await LoadAssets(ids, SiteNames);
                // SetContentView(Resource.Layout.Sites);

            };
            Button AddPhoto = FindViewById<Button>(Resource.Id.photoProof);
            AddPhoto.Click += async (sender, e) =>

            {
                await LoadPhotos(ids, SiteNames);
                // SetContentView(Resource.Layout.Sites);

            };
            Button SignOff = FindViewById<Button>(Resource.Id.SignOff);
            SignOff.Click += async (sender, e) =>

            {
                await LoadSignOffs(ids, SiteNames);
                // SetContentView(Resource.Layout.Sites);

            };
            Button train = FindViewById<Button>(Resource.Id.Training);
            train.Click += async (sender, e) =>
            {
                await LoadAttendence(ids, SiteNames);
            };
            Button Aconnec = FindViewById<Button>(Resource.Id.AddSim);
            Aconnec.Click += async (sender, e) =>
            {
                await LoadConnectivity(ids, SiteNames);
            };
            Button Issue = FindViewById<Button>(Resource.Id.issue);
            Issue.Click += async (sender, e) =>
            {
                await LoadIssues(ids, SiteNames);
            };
            Button notes = FindViewById<Button>(Resource.Id.Notes);
            notes.Click += async (sender, e) =>
            {
                await LoadSiteNotes(ids, SiteNames);
            };
            

        }       

        private async Task LoadSignOffs(string ids, string SiteNames) {
            var progressDialog = ProgressDialog.Show(this, "", "Loading...", true);
            progressDialog.Indeterminate = true;
            progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
          
            SetContentView(Resource.Layout.SignOff);
            TextView SignOfText = FindViewById<TextView>(Resource.Id.SignOffText);
            TextView SignOfHeader = FindViewById<TextView>(Resource.Id.SignOffHeader);
            
            CheckBox ch1 = FindViewById<CheckBox>(Resource.Id.checkBox1);
            CheckBox ch2 = FindViewById<CheckBox>(Resource.Id.checkBox2);
            CheckBox ch3 = FindViewById<CheckBox>(Resource.Id.checkBox3);
            CheckBox ch4 = FindViewById<CheckBox>(Resource.Id.checkBox4);
            CheckBox ch5 = FindViewById<CheckBox>(Resource.Id.checkBox5);
            CheckBox ch6 = FindViewById<CheckBox>(Resource.Id.checkBox6);
            CheckBox ch7 = FindViewById<CheckBox>(Resource.Id.checkBox7);
            CheckBox ch8 = FindViewById<CheckBox>(Resource.Id.checkBox8);
            CheckBox ch9 = FindViewById<CheckBox>(Resource.Id.checkBox9);
            CheckBox ch10 = FindViewById<CheckBox>(Resource.Id.checkBox10);
            CheckBox ch11 = FindViewById<CheckBox>(Resource.Id.checkBox11);
            CheckBox ch12 = FindViewById<CheckBox>(Resource.Id.checkBox12);
            CheckBox ch13 = FindViewById<CheckBox>(Resource.Id.checkBox13);
            CheckBox ch14 = FindViewById<CheckBox>(Resource.Id.checkBox14);
            CheckBox ch15 = FindViewById<CheckBox>(Resource.Id.checkBox15);
            TextView tx1 = FindViewById<TextView>(Resource.Id.item1Text);
            TextView tx2 = FindViewById<TextView>(Resource.Id.item2Text);
            TextView tx3 = FindViewById<TextView>(Resource.Id.item3Text);
            TextView tx4 = FindViewById<TextView>(Resource.Id.item4Text);
            TextView tx5 = FindViewById<TextView>(Resource.Id.item5Text);
            TextView tx6 = FindViewById<TextView>(Resource.Id.item6Text);
            TextView tx7 = FindViewById<TextView>(Resource.Id.item7Text);
            TextView tx8 = FindViewById<TextView>(Resource.Id.item8Text);
            TextView tx9 = FindViewById<TextView>(Resource.Id.item9Text);
            TextView tx10 = FindViewById<TextView>(Resource.Id.item10Text);
            TextView tx11 = FindViewById<TextView>(Resource.Id.item11Text);
            TextView tx12 = FindViewById<TextView>(Resource.Id.item12Text);
            TextView tx13 = FindViewById<TextView>(Resource.Id.item13Text);
            TextView tx14 = FindViewById<TextView>(Resource.Id.item14Text);
            TextView tx15 = FindViewById<TextView>(Resource.Id.item15Text);

            List<SignOffsTemplate> siteSignOffTemplate = new List<SignOffsTemplate>();
            siteSignOffTemplate = await FetchSiteTemplateById(Constants.SignOffTemplate.ToString());
            if (siteSignOffTemplate.Count < 1)
            {
                AlertDialog dialog = null;
                var pop = new AlertDialog.Builder(this);
                pop.SetMessage("No template assigned to project, contact administrator");
                pop.SetTitle("Message");
                pop.SetNeutralButton("OK", delegate { OnDismissNoSignOffsAlert(dialog, ids, SiteNames); });
                dialog = pop.Create();
                dialog.Show();

            }
            else {
                SignOfText.Text = siteSignOffTemplate[0].SignoffWording;
                SignOfHeader.Text = siteSignOffTemplate[0].SignoffType;
                if (siteSignOffTemplate[0].Item1Active == "1")
                {
                    tx1.Text = siteSignOffTemplate[0].Item1Description;

                }
                else {
                    tx1.Visibility = ViewStates.Invisible;
                    ch1.Visibility = ViewStates.Invisible;
                }
                if (siteSignOffTemplate[0].Item2Active == "1")
                {
                    tx2.Text = siteSignOffTemplate[0].Item2Description;

                }
                else
                {
                    tx2.Visibility = ViewStates.Invisible;
                    ch2.Visibility = ViewStates.Invisible;
                }
                if (siteSignOffTemplate[0].Item3Active == "1")
                {
                    tx3.Text = siteSignOffTemplate[0].Item3Description;

                }
                else
                {
                    tx3.Visibility = ViewStates.Invisible;
                    ch3.Visibility = ViewStates.Invisible;
                }
                if (siteSignOffTemplate[0].Item4Active == "1")
                {
                    tx4.Text = siteSignOffTemplate[0].Item4Description;

                }
                else
                {
                    tx4.Visibility = ViewStates.Invisible;
                    ch4.Visibility = ViewStates.Invisible;
                }
                if (siteSignOffTemplate[0].Item5Active == "1")
                {
                    tx5.Text = siteSignOffTemplate[0].Item5Description;

                }
                else
                {
                    tx5.Visibility = ViewStates.Invisible;
                    ch5.Visibility = ViewStates.Invisible;
                }
                if (siteSignOffTemplate[0].Item6Active == "1")
                {
                    tx6.Text = siteSignOffTemplate[0].Item6Description;

                }
                else
                {
                    tx6.Visibility = ViewStates.Invisible;
                    ch6.Visibility = ViewStates.Invisible;
                }
                if (siteSignOffTemplate[0].Item7Active == "1")
                {
                    tx7.Text = siteSignOffTemplate[0].Item7Description;

                }
                else
                {
                    tx7.Visibility = ViewStates.Invisible;
                    ch7.Visibility = ViewStates.Invisible;
                }
                if (siteSignOffTemplate[0].Item8Active == "1")
                {
                    tx8.Text = siteSignOffTemplate[0].Item8Description;

                }
                else
                {
                    tx8.Visibility = ViewStates.Invisible;
                    ch8.Visibility = ViewStates.Invisible;
                }
                if (siteSignOffTemplate[0].Item9Active == "1")
                {
                    tx9.Text = siteSignOffTemplate[0].Item9Description;

                }
                else
                {
                    tx9.Visibility = ViewStates.Invisible;
                    ch9.Visibility = ViewStates.Invisible;
                }
                if (siteSignOffTemplate[0].Item10Active == "1")
                {
                    tx10.Text = siteSignOffTemplate[0].Item10Description;

                }
                else
                {
                    tx10.Visibility = ViewStates.Invisible;
                    ch10.Visibility = ViewStates.Invisible;
                }
                if (siteSignOffTemplate[0].Item11Active == "1")
                {
                    tx11.Text = siteSignOffTemplate[0].Item11Description;

                }
                else
                {
                    tx11.Visibility = ViewStates.Invisible;
                    ch11.Visibility = ViewStates.Invisible;
                }
                if (siteSignOffTemplate[0].Item12Active == "1")
                {
                    tx12.Text = siteSignOffTemplate[0].Item12Description;

                }
                else
                {
                    tx12.Visibility = ViewStates.Invisible;
                    ch12.Visibility = ViewStates.Invisible;
                }
                if (siteSignOffTemplate[0].Item13Active == "1")
                {
                    tx13.Text = siteSignOffTemplate[0].Item13Description;

                }
                else
                {
                    tx13.Visibility = ViewStates.Invisible;
                    ch13.Visibility = ViewStates.Invisible;
                }
                if (siteSignOffTemplate[0].Item14Active == "1")
                {
                    tx14.Text = siteSignOffTemplate[0].Item14Description;

                }
                else
                {
                    tx14.Visibility = ViewStates.Invisible;
                    ch14.Visibility = ViewStates.Invisible;
                }
                if (siteSignOffTemplate[0].Item15Active == "1")
                {
                    tx15.Text = siteSignOffTemplate[0].Item15Description;

                }
                else
                {
                    tx15.Visibility = ViewStates.Invisible;
                    ch15.Visibility = ViewStates.Invisible;
                }

            }
            int addedSigns = 0;
            TextView SigsAdded = FindViewById<TextView>(Resource.Id.addedSignatures);
            Button SignOff = FindViewById<Button>(Resource.Id.sigBtn);


            SigsAdded.Text = "Signatures Added = " + signatures.Count.ToString();
           
            SignOff.Click += async (sender, e) =>
            {
                if (signatures.Count > 2)
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetMessage("You already have 3 signatures loaded for this signoff!");
                    alert.SetPositiveButton("Add Another", async delegate { await loadSignScreen(ids); });
                    alert.SetNegativeButton("Cancel", delegate { });
                    alert.Create().Show();
                }
                else { await loadSignScreen(ids); }

            };
            
            Button back = FindViewById<Button>(Resource.Id.SignOffBack);
            back.Click += async (sender, e) =>
            {
                await LoadInstall(ids, SiteNames);
            };

           Button Save = FindViewById<Button>(Resource.Id.Save);
            Save.Click +=  (sender, e) =>
            {
                progressDialog = ProgressDialog.Show(this, "", "Loading...", true);
                progressDialog.Indeterminate = true;
                progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);

                if (signatures.Count < 1)
                {
                    progressDialog.Cancel();
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetMessage("No signatures loaded, click on ADD Signature.");
                    alert.SetNeutralButton("Okay", delegate { });
                    alert.Create().Show();
                }
                else
                {
                    
                    foreach (var images in signatures)
                    {
                        foreach (var fill in signedNames)
                        {
                            foreach (var inst in instNames)
                            {
                                signaturesMod Signs = new signaturesMod();
                                string fileName = "Sig" + fill + inst + DateTime.Now.Millisecond.ToString() + ".jpg";
                                Signs.FullName = fill;
                                Signs.Institution = inst;
                                var client = new RestClient(Constants.sp_SaveSiteImagesTEST + fileName + "?api_key=b5cb82af7b5d4130f36149f90aa2746782e59a872ac70454ac188743cb55b0ba");
                                string userName = "app@itgalaxy.co.za";
                                string passWord = "Internet1@#";
                                var request = new RestRequest(Method.POST);
                                request.AddHeader("Content-Type", "image/jpg");
                                client.Authenticator = new HttpBasicAuthenticator(userName, passWord);
                               

                                try
                                {
                                    int bytes = images.ByteCount;
                   
                                    ByteBuffer buffer = ByteBuffer.Allocate(bytes); //Create a new buffer
                                    images.CopyPixelsToBuffer(buffer); //Move the byte data to the buffer

                                    byte[] array = new byte[buffer.Remaining()];
                                    buffer.Get(array);

                                    request.AddParameter("body", bytes, ParameterType.RequestBody);
                                    var response = client.Execute(request);
                                    if (response.StatusDescription == "Created")
                                    {

                                        client = new RestClient(Constants.sp_SaveSiteSignature);
                                        request = new RestRequest(Method.POST);
                                        request.AddHeader("Content-Type", "application/zip");
                                        client.Authenticator = new HttpBasicAuthenticator(userName, passWord);

                                        request.RequestFormat = DataFormat.Json;
                                        request.AddHeader("Accept", "application/json");
                                        request.AddParameter("application/json", "{ \"params\": [    { \"name\": \"DocumentName\",      \"param_type\": \"IN\",      \"value\": \"" + fileName + "\",      \"type\": \"string\"    },{      \"name\": \"FullName\",      \"param_type\": \"IN\",      \"value\": \"" + fill + "\",      \"type\": \"string\"    },{      \"name\": \"Institution\",      \"param_type\": \"IN\",      \"value\": \"" + inst + "\",      \"type\": \"string\"    },{      \"name\": \"SiteId\",      \"param_type\": \"IN\",      \"value\": \"" + ids + "\",      \"type\": \"int\"    }]}", ParameterType.RequestBody);
                                        response = client.Execute(request);
  
                                    }

                                }
                                catch (Exception ex)
                                {

                                    progressDialog.Cancel();
                                    Log.Info("SignatureSave", @"				Exception {0}", ex.Message);

                                    Console.WriteLine(@"				ERROR {0}", ex.Message);


                                }
                            }
                        }

                    }
                    



                SignOffs signsa = new SignOffs();
              
                EditText Notes = FindViewById<EditText>(Resource.Id.notes);
                signsa.notes = Notes.Text;
                signsa.Item1Check = ch1.Selected;
                signsa.Item2Check = ch2.Selected;
                signsa.Item3Check = ch3.Selected;
                signsa.Item4Check = ch4.Selected;
                signsa.Item5Check = ch5.Selected;
                signsa.Item6Check = ch6.Selected;
                signsa.Item7Check = ch7.Selected;
                signsa.Item8Check = ch8.Selected;
                signsa.Item9Check = ch9.Selected;
                signsa.Item10Check = ch10.Selected;
                signsa.Item11Check = ch11.Selected;
                signsa.Item12Check = ch12.Selected;
                signsa.Item13Check = ch13.Selected;
                signsa.Item14Check = ch14.Selected;
                signsa.Item15Check = ch15.Selected;
                try {
                                     
                    var client = new RestClient(Constants.sp_SaveSiteSignOff);
                    var request = new RestRequest(Method.POST);
                    string userName = "app@itgalaxy.co.za";
                    string passWord = "Internet1@#";
                    request.AddHeader("Content-Type", "application/zip");
                    client.Authenticator = new HttpBasicAuthenticator(userName, passWord);

                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("Accept", "application/json");
                    request.AddParameter("application/json", "{ \"params\": [    { \"name\": \"SiteId\",      \"param_type\": \"IN\",      \"value\": \"" + ids + "\",      \"type\": \"int\"    },{      \"name\": \"SignOffTemplateId\",      \"param_type\": \"IN\",      \"value\": \"" + Constants.SignOffTemplate.ToString() + "\",      \"type\": \"int\"    },{      \"name\": \"CreatedBy\",      \"param_type\": \"IN\",      \"value\": \"" + Constants.Username + "\",      \"type\": \"string\"    },{      \"name\": \"Item1Check\",      \"param_type\": \"IN\",      \"value\": \"" + signsa.Item1Check.ToString() + "\",      \"type\": \"boolean\"    },{      \"name\": \"Item2Check\",      \"param_type\": \"IN\",      \"value\": \"" + signsa.Item2Check.ToString() + "\",      \"type\": \"boolean\"    },{      \"name\": \"Item3Check\",      \"param_type\": \"IN\",      \"value\": \"" + signsa.Item3Check.ToString() + "\",      \"type\": \"boolean\"    },{      \"name\": \"Item4Check\",      \"param_type\": \"IN\",      \"value\": \"" + signsa.Item4Check.ToString() + "\",      \"type\": \"boolean\"    },{      \"name\": \"Item5Check\",      \"param_type\": \"IN\",      \"value\": \"" + signsa.Item5Check.ToString() + "\",      \"type\": \"boolean\"    },{      \"name\": \"Item6Check\",      \"param_type\": \"IN\",      \"value\": \"" + signsa.Item6Check.ToString() + "\",      \"type\": \"boolean\"    },{      \"name\": \"Item7Check\",      \"param_type\": \"IN\",      \"value\": \"" + signsa.Item7Check.ToString() + "\",      \"type\": \"boolean\"    },{      \"name\": \"Item8Check\",      \"param_type\": \"IN\",      \"value\": \"" + signsa.Item8Check.ToString() + "\",      \"type\": \"boolean\"    },{      \"name\": \"Item9Check\",      \"param_type\": \"IN\",      \"value\": \"" + signsa.Item9Check.ToString() + "\",      \"type\": \"boolean\"    },{      \"name\": \"Item10Check\",      \"param_type\": \"IN\",      \"value\": \"" + signsa.Item10Check.ToString() + "\",      \"type\": \"boolean\"    },{      \"name\": \"Item11Check\",      \"param_type\": \"IN\",      \"value\": \"" + signsa.Item11Check.ToString() + "\",      \"type\": \"boolean\"    },{      \"name\": \"Item12Check\",      \"param_type\": \"IN\",      \"value\": \"" + signsa.Item12Check.ToString() + "\",      \"type\": \"boolean\"    },{      \"name\": \"Item13Check\",      \"param_type\": \"IN\",      \"value\": \"" + signsa.Item13Check.ToString() + "\",      \"type\": \"boolean\"    },{      \"name\": \"Item14Check\",      \"param_type\": \"IN\",      \"value\": \"" + signsa.Item14Check.ToString() + "\",      \"type\": \"boolean\"    },{      \"name\": \"Item15Check\",      \"param_type\": \"IN\",      \"value\": \"" + signsa.Item15Check.ToString() + "\",      \"type\": \"boolean\"    },{      \"name\": \"Notes\",      \"param_type\": \"IN\",      \"value\": \"" + signsa.notes+ "\",      \"type\": \"string\"    }]}", ParameterType.RequestBody);

                    var response = client.Execute(request);

                    List<ResultSet> jj = JsonConvert.DeserializeObject<List<ResultSet>>(response.Content);
                    if (jj.Count > 0)
                    {
                            progressDialog.Cancel();
                            AlertDialog dialog = null;
                        var pop = new AlertDialog.Builder(this);
                        pop.SetMessage("Saved Successfull.");
                        pop.SetTitle("Message");
                        pop.SetNeutralButton("OK", delegate { OnDismissSuccessSignOffsAlert(dialog, ids, ""); });
                        dialog = pop.Create();
                        dialog.Show();

                    }
                    else
                    {
                            progressDialog.Cancel();
                            AlertDialog dialog = null;
                        var pop = new AlertDialog.Builder(this);
                        pop.SetMessage("Save unsuccessfull - try again.");
                        pop.SetTitle("Message");
                        pop.SetNeutralButton("OK", delegate { OnDismissSuccessSignOffsAlert(dialog, ids, ""); });
                        dialog = pop.Create();
                        dialog.Show();
                    }
                    }
                catch (Exception ex)
                {

                        progressDialog.Cancel();
                        Log.Info("SignatureSave", @"				Exception {0}", ex.Message);

                    Console.WriteLine(@"				ERROR {0}", ex.Message);
                    AlertDialog dialog = null;
                    var pop = new AlertDialog.Builder(this);
                    pop.SetMessage("Save unsuccessfull - try again.");
                    pop.SetTitle("Message");
                    pop.SetNeutralButton("OK", delegate { OnDismissSuccessSignOffsAlert(dialog, ids, ""); });
                    dialog = pop.Create();
                    dialog.Show();

                }
                }


            };
            progressDialog.Cancel();
            AlertDialog.Builder alert2 = new AlertDialog.Builder(this);
            alert2.SetMessage("You can scroll for more check boxes in the checkbox area.");
            alert2.SetNeutralButton("Okay", delegate { });
            alert2.Create().Show();
        }

        private async Task loadSignScreen(string ids) {
       SetContentView(Resource.Layout.signature);
       SignaturePadView signature = FindViewById<SignaturePadView>(Resource.Id.signatureView);
        // Customization activated
        View root = FindViewById<View>(Resource.Id.rootView);
        //root.SetBackgroundColor(Color.White);

        // Activate this to internally use a bitmap to store the strokes
        // (good for frequent-redraw situations, bad for memory footprint)
        // signature.UseBitmapBuffer = true;

        signature.Caption.Text = "Authorization Signature";
        signature.Caption.SetTypeface(Typeface.Serif, TypefaceStyle.BoldItalic);
        signature.Caption.SetTextSize(global::Android.Util.ComplexUnitType.Sp, 16f);
        signature.SignaturePrompt.Text = ">>";
        signature.SignaturePrompt.SetTypeface(Typeface.SansSerif, TypefaceStyle.Normal);
        signature.SignaturePrompt.SetTextSize(global::Android.Util.ComplexUnitType.Sp, 32f);
        //signature.BackgroundColor = Color.Rgb(255, 255, 200); // a light yellow.
        signature.StrokeColor = Color.White;

        signature.BackgroundImageView.SetImageResource(Resource.Drawable.logo_galaxy_black_64);
        signature.BackgroundImageView.SetAlpha(16);
        signature.BackgroundImageView.SetAdjustViewBounds(true);
        var layout = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.FillParent, RelativeLayout.LayoutParams.FillParent);
        layout.AddRule(LayoutRules.CenterInParent);
        layout.SetMargins(20, 20, 20, 20);
        signature.BackgroundImageView.LayoutParameters = layout;

        // You can change paddings for positioning...
        var caption = signature.Caption;
        caption.SetPadding(caption.PaddingLeft, 1, caption.PaddingRight, 25);

        Button btnSave = FindViewById<Button>(Resource.Id.btnSave);
        EditText sigNameS = FindViewById<EditText>(Resource.Id.sigName);
        EditText sigInstS = FindViewById<EditText>(Resource.Id.sigInst);

        btnSave.Click += async delegate
        {
            if (signature.IsBlank)
            {//Display the base line for the user to sign on.
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetMessage("No signature to save.");
                alert.SetNeutralButton("Okay", delegate { });
                alert.Create().Show();
            }
            else
            {
                if (sigNameS.Text.Length < 1 | sigInstS.Text.Length < 1)
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetMessage("No name or instatution provided.");
                    alert.SetNeutralButton("Okay", delegate { });
                    alert.Create().Show();
                }
                else
                {
                    signedNames.Add(sigNameS.Text);
                    instNames.Add(sigInstS.Text);
                    points = signature.Points;
                    signatures.Add(signature.GetImage());
                    await LoadSignOffs(ids, "");


                }
            }
        };
    
}

        private async Task LoadAssets(string ids, string SiteName)
        {
            SetContentView(Resource.Layout.Asets);
            ActionBar.Title = "Site Assets";
            List<SchoolAssetRegisters> listitems = await FetchAssetsBySiteId(ids);
            List<string> Displaylistitems = new List<string>();
            ListView listV = FindViewById<ListView>(Resource.Id.listAssets);
            if (listitems.Count > 0)
            {
                List<string> AdapterList = new List<string>();
                foreach(var item in listitems)
                {
                    Displaylistitems.Add("Item: "+item.Item + " \n Serial: " + item.SerialNumber + " \n Quantity: " + item.Quantity);
                }


                ArrayAdapter adapter = new ArrayAdapter(
                                    this, //Context, typically the Activity
                                    Android.Resource.Layout.SimpleListItem1, //The layout. How the data will be presented 
                                    Displaylistitems //The enumerable data
                                );
                listV.Adapter = adapter;               

            }


            Button Back = FindViewById<Button>(Resource.Id.AssetBack);
            Back.Click += async (sender, e) =>

            {
                await LoadInstall(ids,SiteName);
                // SetContentView(Resource.Layout.Sites);
            };

            Button Add = FindViewById<Button>(Resource.Id.Add);
            Add.Click += async (sender, e) =>

            {
                await LoadAssetsAdd(ids, SiteName);
               
                
            };

        }
        private async Task LoadConnectivity(string ids, string SiteName)
        {
            var progressDialog = ProgressDialog.Show(this, "", "Loading ...", true);
            progressDialog.Indeterminate = true;
            progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);

            SetContentView(Resource.Layout.Connectivity);
            ActionBar.Title = "Site Connectivity";
            List<ConnectivityModel> listitems = await FetchConnectivityBySiteId(ids);
            List<string> Displaylistitems = new List<string>();
            ListView listV = FindViewById<ListView>(Resource.Id.listAssets);
            if (listitems.Count >0)
            {
                List<string> AdapterList = new List<string>();
                foreach (var item in listitems)
                {
                    Displaylistitems.Add("Sim Card Number: " + item.SimCardNumber + " \n Router IMEA: " + item.RouterImea + " \n Rica Officer: " + item.RICA_Officer_Name + "\n Other Rica Info" + item.Other_RICA_Info);
                }


                ArrayAdapter adapter = new ArrayAdapter(
                                    this, //Context, typically the Activity
                                    Android.Resource.Layout.SimpleListItem1, //The layout. How the data will be presented 
                                    Displaylistitems //The enumerable data
                                );
                listV.Adapter = adapter;

            }


            Button Back = FindViewById<Button>(Resource.Id.ConnBack);
            Back.Click += async (sender, e) =>

            {
                await LoadInstall(ids, SiteName);
                // SetContentView(Resource.Layout.Sites);
            };

            Button Add = FindViewById<Button>(Resource.Id.AddConn);
            Add.Click += async (sender, e) =>

            {
                await LoadConnectivityAdd(ids, SiteName);


            };
            progressDialog.Dismiss();
        }
        private async Task LoadConnectivityAdd(string ids, string SiteName)
        {
            SetContentView(Resource.Layout.AddConnectivity);
            ActionBar.Title = "New Connectivity";
            Button Back = FindViewById<Button>(Resource.Id.ConnBack);
            Button save = FindViewById<Button>(Resource.Id.ConnsSave);
            EditText SimCardNumbers = FindViewById<EditText>(Resource.Id.SimCardNumbers);
            EditText RouterIMEA = FindViewById<EditText>(Resource.Id.RouterIMEA);
            EditText RiceOfficer = FindViewById<EditText>(Resource.Id.RiceOfficer);
            EditText OtherRica = FindViewById<EditText>(Resource.Id.OtherRica);



            Back.Click += async (sender, e) =>

            {

                await LoadInstall(ids, SiteName);

            };

          
            save.Click += async (sender, e) =>

            {

                ConnectivityModel ss = new ConnectivityModel();
                ss.SimCardNumber = SimCardNumbers.Text;
                ss.RouterImea = RouterIMEA.Text;
                ss.RICA_Officer_Name = RiceOfficer.Text;
                ss.SiteId = Constants.CurrentSiteId;
                ss.Other_RICA_Info = OtherRica.Text;
                ss.CreatedBy = Constants.Username;
                string result = await LoadSaveConnectivity(ss);

                if (result == "1")
                {
                    var pop = new AlertDialog.Builder(this);
                    AlertDialog dialog = null;
                    pop.SetMessage("Save was a Success!");
                    pop.SetTitle("Save Message");

                    pop.SetNeutralButton("OK", delegate { OnDismissAlert(dialog); });
                    dialog = pop.Create();
                    dialog.Show();
                    await LoadConnectivity(ids, SiteName);
                }
                else
                {

                    var pop = new AlertDialog.Builder(this);
                    AlertDialog dialog = null;
                    pop.SetMessage("Save was a unsuccessfull! Try again or check your values.");
                    pop.SetTitle("Save Message");
                    pop.SetNeutralButton("OK", delegate { OnDismissAlert(dialog); });
                    dialog = pop.Create();
                    dialog.Show();
                }
                // SetContentView(Resource.Layout.Sites);
            };
        }
        private async Task<string> LoadSaveConnectivity(ConnectivityModel nn)
        {
            string retu = "0";
            try
            {
                var progressDialog = ProgressDialog.Show(this, "", "Loading ...", true);
                progressDialog.Indeterminate = true;
                progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);

                await Task.Run(() =>

                {

                    var client = new RestClient(Constants.sp_InsertConnectivityInfo);
                    string userName = "app@itgalaxy.co.za";
                    string passWord = "Internet1@#";
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/zip");
                    client.Authenticator = new HttpBasicAuthenticator(userName, passWord);

                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("Accept", "application/json");

                    string ParText = "{      \"name\": \"SiteId\",      \"param_type\": \"IN\",      \"value\": \"" + nn.SiteId + "\",      \"type\": \"int\"    },";
                    ParText += "{      \"name\": \"CreatedBy\",      \"param_type\": \"IN\",      \"value\": \"" + Constants.Username + "\",      \"type\": \"string\"    },";
                    ParText += "{      \"name\": \"SimCardNumber\",      \"param_type\": \"IN\",      \"value\": \"" + nn.SimCardNumber + "\",      \"type\": \"string\"    },";
                    ParText += "{      \"name\": \"RouterImea\",      \"param_type\": \"IN\",      \"value\": \"" + nn.RouterImea + "\",      \"type\": \"string\"    },";
                    ParText += "{      \"name\": \"RICA_Officer_Name\",      \"param_type\": \"IN\",      \"value\": \"" + nn.RICA_Officer_Name + "\",      \"type\": \"string\"    },";
                    ParText += "{      \"name\": \"Other_RICA_Info\",      \"param_type\": \"IN\",      \"value\": \"" + nn.Other_RICA_Info + "\",      \"type\": \"string\"    }";

                    request.AddParameter("application/json", "{ \"params\": [" + ParText + "]}", ParameterType.RequestBody);

                    var response = client.Execute(request);

                    Log.Info("Re", @"				LoadSaveConnectivity {0}", response.Content.ToString());

                    List<ResultSet> jj = JsonConvert.DeserializeObject<List<ResultSet>>(response.Content);

                    if (jj.Count > 0)
                    {
                        retu = jj[0].result;
                    }
                    progressDialog.Dismiss();
                    return retu;
                });
                progressDialog.Dismiss();
                return retu;
            }
            catch (Exception ex)
            {

                Log.Info("LoadSaveConnectivity", @"				ERROR {0}", ex.Message);

                Console.WriteLine(@"				ERROR {0}", ex.Message);
                return null;

            }

        }
        private async Task<List<ConnectivityModel>> FetchConnectivityBySiteId(string ids)
        {
            List<ConnectivityModel> items = new List<ConnectivityModel>();
            try
            {
                await Task.Run(() =>

                {
                    var client = new RestClient(Constants.sp_GetConnectivityInfo);
                    string userName = "app@itgalaxy.co.za";
                    string passWord = "Internet1@#";
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/zip");
                    client.Authenticator = new HttpBasicAuthenticator(userName, passWord);

                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("Accept", "application/json");
                    request.AddParameter("application/json", "{ \"params\": [    {      \"name\": \"SiteId\",      \"param_type\": \"IN\",      \"value\": \"" + ids + "\",      \"type\": \"int\"    }]}", ParameterType.RequestBody);

                    var response = client.Execute(request);

                    Log.Info("Re", @"				FetchAssetsBySiteId {0}", response.Content.ToString());
                    if (!response.Content.Contains("[]"))
                    {
                        Log.Info("Re", @"				FetchAssetsBySiteId {0}", response.Content.ToString());

                        List<ConnectivityModel> jj = JsonConvert.DeserializeObject<List<ConnectivityModel>>(response.Content);


                        items = jj;
                    }

                });

                if (items != null)
                {
                    return items;
                }


                return null;

            }
            catch (Exception ex)
            {


                Log.Info("FetchAssetsBySiteId", @"				ERROR {0}", ex.Message);

                Console.WriteLine(@"				ERROR {0}", ex.Message);
                return null;

            }
        }

        private async Task LoadAttendence(string ids, string SiteName)
        {
            
            SetContentView(Resource.Layout.AttendenceRegister);

            ActionBar.Title = "Site Attendence";
            var progressDialog = ProgressDialog.Show(this, "", "Loading attendence...", true);
            progressDialog.Indeterminate = true;
            progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            try
            {
                List<AttencenceRegister> listitems = await FetchAttendenceBySiteId(ids);

                List<string> Displaylistitems = new List<string>();
                ListView listV = FindViewById<ListView>(Resource.Id.listAttendence);
                if (listitems != null)
                {
                    List<string> AdapterList = new List<string>();
                    foreach (var item in listitems)
                    {
                        Displaylistitems.Add("Name: " + item.FullName + " \n Designation: " + item.Designation + " \n Contact: " + item.ContactNumber);
                    }


                    ArrayAdapter adapter = new ArrayAdapter(
                                        this, //Context, typically the Activity
                                        Android.Resource.Layout.SimpleListItem1, //The layout. How the data will be presented 
                                        Displaylistitems //The enumerable data
                                    );
                    listV.Adapter = adapter;

                }
            }
            catch (Exception ex) { }

            Button Back = FindViewById<Button>(Resource.Id.AttendBack);
            Back.Click += async (sender, e) =>

            {
                await LoadInstall(ids, SiteName);
                // SetContentView(Resource.Layout.Sites);
            };

            Button Add = FindViewById<Button>(Resource.Id.AddAttendence);
            Add.Click +=  (sender, e) =>
            {
                 LoadAttendenceAdd(ids, SiteName);

            };
            progressDialog.Dismiss();
        }
       
        private async Task LoadPhotos(string ids, string SiteName)
        {
            var progressDialog = ProgressDialog.Show(this, "", "Loading ...", true);
            progressDialog.Indeterminate = true;
            progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);

            SetContentView(Resource.Layout.LayoutSPhotos);
            ActionBar.Title = "Site Photos";
            AlertDialog dialog = null;
            List<SiteImageSignOffs> listitems = await FetchSitePicksBySiteId(ids);
            List<string> Displaylistitems = new List<string>();
            ListView listV = FindViewById<ListView>(Resource.Id.ImagesList);

            if (listitems.Count > 0)
            {

                foreach (var item in listitems)
                {
                    //  var imageBitmap = BitmapFactory.DecodeByteArray(item.StoredDocument, 0, item.StoredDocument.Length);
                    Displaylistitems.Add(item.DocumentName);
                }


                ArrayAdapter adapter = new ArrayAdapter(
                                    this, //Context, typically the Activity
                                    Android.Resource.Layout.SimpleListItem1, //The layout. How the data will be presented 
                                    Displaylistitems //The enumerable data
                                );

                listV.Adapter = adapter;
                listV.ItemClick += (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
                {
                  

                    selectSite = listitems[e.Position].Id;

                    List<byte> byteList = new List<byte>();

                    string hexPart = listitems[e.Position].StoredDocument;
                    for (int i = 0; i < hexPart.Length / 2; i++)
                    {
                        string hexNumber = hexPart.Substring(i * 2, 2);
                        byteList.Add((byte)Convert.ToInt32(hexNumber, 16));
                    }

                    byte[] original = byteList.ToArray();
                    Bitmap bmp;
                    using (var ms = new System.IO.MemoryStream(original))
                    {

                        bmp = BitmapFactory.DecodeByteArray(original, 0, original.Length);
                    }

                    var pop = new AlertDialog.Builder(this);
                    pop.SetTitle(listitems[e.Position].DocumentName);
                    pop.SetNeutralButton("Dismiss", delegate { OnDismissAlert(dialog); });
                    LayoutInflater factory = LayoutInflater.From(this);
                    View view = factory.Inflate(Resource.Layout.imageView, null);
                   
                    ImageView img = view.FindViewById<ImageView>(Resource.Id.ImgView1);
                    img.SetImageBitmap(bmp);
                    pop.SetView(view);
                    dialog = pop.Create();
                    dialog.Show();

                };
                }
        


            Button Back = FindViewById<Button>(Resource.Id.imgBack);
            Back.Click += async (sender, e) =>

            {
                await LoadInstall(ids, SiteName);
                // SetContentView(Resource.Layout.Sites);
            };

            Button Add = FindViewById<Button>(Resource.Id.Add);
            dialog = null;
            if (Constants.ShowOnceImg == 0)
            {
                var pop = new AlertDialog.Builder(this);
                pop.SetMessage("If your photo takes to long to upload or you want to reduce the size, go to your device camera settings and reduce the picture quality");
                pop.SetTitle("Message");
                pop.SetNeutralButton("OK", delegate { OnDismissAlert(dialog); });
                dialog = pop.Create();
                dialog.Show();
                Constants.ShowOnceImg = 1;
            }
            Add.Click += async delegate
            {
                

               

                Bitmap i;
                var imgName = "";
                string dd = DateTime.Now.Millisecond.ToString();
                var media = new MediaImplementation();
                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                   
                    Directory = "Sample",
                    Name = SiteName+"SO"+ dd + ".jpg",
                    DefaultCamera = CameraDevice.Front
                });
                if (file == null)
                { return;  dialog = null;
                   
                    var pop = new AlertDialog.Builder(this);
                    pop.SetMessage("Something went wrong, try again");
                    pop.SetTitle("Message");
                    pop.SetNeutralButton("OK", delegate { OnDismissAlert(dialog); });
                    dialog = pop.Create();
                    dialog.Show();
                    await LoadPhotos(ids,SiteName);

                }
                else
                {
                    var path = file.Path;
                    
                    imgName = SiteName + "SO" + dd + ".jpg";
                    SiteImageSignOffs nn = new SiteImageSignOffs();

                    nn.DocumentName = imgName;
                    nn.SchoolId = int.Parse(ids);

                    FileInfo fileInfo = new FileInfo(path);
                    long imageFileLength = fileInfo.Length;
                    FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    byte[] imageData = br.ReadBytes((int)imageFileLength);
                   
                   
                    string reply = await LoadSaveImage2(file.Path, imageData, fileInfo.Name,ids);
                    file.Dispose();
                    if (reply == "1")
                    {
                         dialog = null;

                        var pop = new AlertDialog.Builder(this);
                        pop.SetMessage("Save Successfull");
                        pop.SetTitle("Message");
                        pop.SetNeutralButton("OK", delegate { OnDismissSuccessPicAlert(dialog,ids,SiteName); });
                        dialog = pop.Create();
                        dialog.Show();
                    }
                    else
                    {
                         dialog = null;

                        var pop = new AlertDialog.Builder(this);
                        pop.SetMessage("Something went wrong, try again");
                        pop.SetTitle("Message");
                        pop.SetNeutralButton("OK", delegate { OnDismissAlert(dialog); });
                        dialog = pop.Create();
                        dialog.Show();
                        await LoadPhotos(ids, SiteName);
                    }

                }
            };
            progressDialog.Dismiss();
        }

        private async Task LoadSelectedPhotos(string siteId,string ids, string SiteName)
        {
            var progressDialog = ProgressDialog.Show(this, "", "Loading ...", true);
            progressDialog.Indeterminate = true;
            progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);

            SetContentView(Resource.Layout.ShowImage);
            ActionBar.Title = "Selected Image";
           
            try
            {
                await Task.Run(() =>

                {
                    var client = new RestClient(Constants.sp_GetSiteImages);
                    string userName = "app@itgalaxy.co.za";
                    string passWord = "Internet1@#";
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/zip");
                    client.Authenticator = new HttpBasicAuthenticator(userName, passWord);

                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("Accept", "application/json");
                    request.AddParameter("application/json", "{ \"params\": [    {      \"name\": \"PictureId\",      \"param_type\": \"IN\",      \"value\": \"" + ids + "\",      \"type\": \"int\"    }]}", ParameterType.RequestBody);

                    var response = client.Execute(request);

                    Log.Info("Re", @"				SelectedSite {0}", response.Content.ToString());

                    List<SiteImageSignOffs> jj = JsonConvert.DeserializeObject<List<SiteImageSignOffs>>(response.Content);

                   

                });
              

            }
            catch (Exception ex)
            {


                Log.Info("FetchSitePicksBySiteId", @"				ERROR {0}", ex.Message);

                Console.WriteLine(@"				ERROR {0}", ex.Message);
              

            }



            progressDialog.Cancel();
        }

        private async Task LoadAssetsAdd(string ids, string SiteName) {
            SetContentView(Resource.Layout.NewAsset);
            ZXingScannerFragment scanFragment;
            scanFragment = new ZXingScannerFragment();
            ActionBar.Title = "New Assets";
            Button Back = FindViewById<Button>(Resource.Id.AssetBack);
            Button Scan = FindViewById<Button>(Resource.Id.BarCodeBtn);
            Button save = FindViewById<Button>(Resource.Id.Save);
            EditText itemname = FindViewById<EditText>(Resource.Id.ItemNameT);
            EditText ItemDescription = FindViewById<EditText>(Resource.Id.ItemDescription);
            EditText Serial = FindViewById<EditText>(Resource.Id.Serial);
            EditText Quantity = FindViewById<EditText>(Resource.Id.Quantity);
           


            Back.Click += async (sender, e) =>
            
            {

                await LoadInstall(ids, SiteName);

            };

            Scan.Click += async (sender, e) =>

            {
               
                // SetContentView(Resource.Layout.Sites);
                MobileBarcodeScanner.Initialize(Application);
                var scanner = new MobileBarcodeScanner();
                var result = await scanner.Scan();

                if (result != null)
                {
                    AlertDialog dialog = null;
                    Serial.Text = result.Text;
                    var pop = new AlertDialog.Builder(this);
                    pop.SetMessage("Serial Scan will return an approximate match, edit the values if the item is not the same. If the model only returns the serial it means you will need to add the rest of the details manually in the add asset screen. ");
                    pop.SetTitle("Scanner Message");
                    pop.SetNeutralButton("OK",delegate { OnDismissAlert(dialog); });
                    dialog = pop.Create();
                   
                    dialog.Show();
                   List<SchoolAssetRegisters> returs = await FetchAssetsByAssetMatch(result.Text);
                    if (returs.Count > 0) {
                        itemname.Text = returs[0].Item;
                        ItemDescription.Text = returs[0].ItemDescription;
                   
                    }

                }
            };

            save.Click += async (sender, e) =>

            {
                
                SchoolAssetRegisters ss = new SchoolAssetRegisters();
                ss.Item = itemname.Text;
                ss.ItemDescription = ItemDescription.Text;
                ss.SerialNumber = Serial.Text;
                ss.SiteId = Constants.CurrentSiteId;
                ss.Quantity = int.Parse(Quantity.Text);
                ss.CreatedBy = Constants.Username; 
                string result = await LoadSaveAsset(ss);

                if (result == "1")
                {
                    var pop = new AlertDialog.Builder(this);
                    AlertDialog dialog = null;
                    pop.SetMessage("Save was a Success!");
                    pop.SetTitle("Save Message");
                    
                    pop.SetNeutralButton("OK", delegate { OnDismissAlert(dialog); });
                    dialog = pop.Create();
                    dialog.Show();
                    await LoadAssetsAdd(ids, SiteName);
                }
                else {
                    
                    var pop = new AlertDialog.Builder(this);
                    AlertDialog dialog = null;
                    pop.SetMessage("Save was a unsuccessfull! Try again or check your values.");
                    pop.SetTitle("Save Message");
                    pop.SetNeutralButton("OK", delegate { OnDismissAlert(dialog); });
                    dialog = pop.Create();
                    dialog.Show();
                }
                // SetContentView(Resource.Layout.Sites);
            };
        }
        private void OnDismissAlert(AlertDialog a)
        {
            a.Dismiss();
        }

        private async void OnDismissSuccessPicAlert(AlertDialog a, string ids, string SiteName)
        {
            a.Dismiss();
            await LoadPhotos(ids, SiteName);
        }
        private async void OnDismissSuccessSignOffsAlert(AlertDialog a, string ids, string SiteName)
        {
            a.Dismiss();
            await LoadInstall(ids,SiteName);
        }
        private async void OnDismissSuccessAttendSignOffsAlert(AlertDialog a, string ids, string SiteName)
        {
            a.Dismiss();
            await LoadAttendence(ids, SiteName);
        }
        private async void OnDismissNoSignOffsAlert(AlertDialog a, string ids, string SiteName)
        {
            a.Dismiss();
            await LoadInstall(ids, SiteName);
        }

        private async Task<string> LoadSaveAsset(SchoolAssetRegisters nn)
        { string retu = "0";
            try
            {
                var progressDialog = ProgressDialog.Show(this, "", "Loading ...", true);
                progressDialog.Indeterminate = true;
                progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);

                await Task.Run(() =>

                {
                   
                    var client = new RestClient(Constants.sp_SaveAssets);
                    string userName = "app@itgalaxy.co.za";
                    string passWord = "Internet1@#";
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/zip");
                    client.Authenticator = new HttpBasicAuthenticator(userName, passWord);

                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("Accept", "application/json");

                    string ParText = "{      \"name\": \"Item\",      \"param_type\": \"IN\",      \"value\": \"" + nn.Item + "\",      \"type\": \"string\"    },";
                    ParText += "{      \"name\": \"ItemDescription\",      \"param_type\": \"IN\",      \"value\": \"" + Regex.Replace(nn.ItemDescription, @"\r\n?|\n", " ") + "\",      \"type\": \"string\"    },";
                    ParText += "{      \"name\": \"Serial\",      \"param_type\": \"IN\",      \"value\": \"" + nn.SerialNumber + "\",      \"type\": \"string\"    },";
                    ParText += "{      \"name\": \"Quantity\",      \"param_type\": \"IN\",      \"value\": \"" + nn.Quantity + "\",      \"type\": \"string\"    },";
                    ParText += "{      \"name\": \"SiteId\",      \"param_type\": \"IN\",      \"value\": \"" + nn.SiteId + "\",      \"type\": \"string\"    },";
                    ParText += "{      \"name\": \"CreatedBy\",      \"param_type\": \"IN\",      \"value\": \"" + nn.CreatedBy + "\",      \"type\": \"string\"    }";

                    request.AddParameter("application/json", "{ \"params\": ["+ParText+"]}", ParameterType.RequestBody);

                    var response = client.Execute(request);

                    Log.Info("Re", @"				LoadSaveAsset {0}", response.Content.ToString());

                    List<ResultSet> jj = JsonConvert.DeserializeObject<List<ResultSet>>(response.Content);

                    if (jj.Count > 0) {
                        retu = jj[0].result;
                    }
                    progressDialog.Dismiss();
                    return retu;
                });
                progressDialog.Dismiss();
                return retu;
            }
            catch (Exception ex) { 
 
                Log.Info("FetchAssetsBySiteId", @"				ERROR {0}", ex.Message);
             
                Console.WriteLine(@"				ERROR {0}", ex.Message);
                return null;

            }

        }
        private void LoadAttendenceAdd(string ids,string SiteName)
        {
            var progressDialog = ProgressDialog.Show(this, "", "Loading...", true);
            progressDialog.Indeterminate = true;
            progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            SetContentView(Resource.Layout.AttendenceSignature);
            Button btnSave = FindViewById<Button>(Resource.Id.btnSave);
            EditText sigNameS = FindViewById<EditText>(Resource.Id.sigName);
            EditText sigInstS = FindViewById<EditText>(Resource.Id.sigInst);
            EditText sigDesignS = FindViewById<EditText>(Resource.Id.sigDesign);
            SignaturePadView signature = FindViewById<SignaturePadView>(Resource.Id.signatureView);
            // Customization activated
            View root = FindViewById<View>(Resource.Id.rootView);
            //root.SetBackgroundColor(Color.White);

            // Activate this to internally use a bitmap to store the strokes
            // (good for frequent-redraw situations, bad for memory footprint)
            // signature.UseBitmapBuffer = true;

            signature.Caption.Text = "Authorization Signature";
            signature.Caption.SetTypeface(Typeface.Serif, TypefaceStyle.BoldItalic);
            signature.Caption.SetTextSize(global::Android.Util.ComplexUnitType.Sp, 16f);
            signature.SignaturePrompt.Text = ">>";
            signature.SignaturePrompt.SetTypeface(Typeface.SansSerif, TypefaceStyle.Normal);
            signature.SignaturePrompt.SetTextSize(global::Android.Util.ComplexUnitType.Sp, 32f);
            //signature.BackgroundColor = Color.Rgb(255, 255, 200); // a light yellow.
            signature.StrokeColor = Color.White;

            signature.BackgroundImageView.SetImageResource(Resource.Drawable.logo_galaxy_black_64);
            signature.BackgroundImageView.SetAlpha(16);
            signature.BackgroundImageView.SetAdjustViewBounds(true);
            var layout = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.FillParent, RelativeLayout.LayoutParams.FillParent);
            layout.AddRule(LayoutRules.CenterInParent);
            layout.SetMargins(20, 20, 20, 20);
            signature.BackgroundImageView.LayoutParameters = layout;

            // You can change paddings for positioning...
            var caption = signature.Caption;
            caption.SetPadding(caption.PaddingLeft, 1, caption.PaddingRight, 25);

          

            btnSave.Click += delegate
           {
               progressDialog = ProgressDialog.Show(this, "", "Saving...", true);
               progressDialog.Indeterminate = true;
               progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
               if (signature.IsBlank)
               {//Display the base line for the user to sign on.
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                   alert.SetMessage("No signature to save.");
                   alert.SetNeutralButton("Okay", delegate { });
                   alert.Create().Show();
               }
               else
               {
                   if (sigNameS.Text.Length < 1 | sigDesignS.Text.Length < 1)
                   {
                       progressDialog.Cancel();
                       AlertDialog.Builder alert = new AlertDialog.Builder(this);
                       alert.SetMessage("No name or designation provided.");
                       alert.SetNeutralButton("Okay", delegate { });
                       alert.Create().Show();
                   }
                   else
                   {
                       try
                       {
                           if (sigDesignS.Text.Length < 1) { sigDesignS.Text = " "; }
                           string fileName = "Sig" + sigNameS.Text + sigDesignS.Text + DateTime.Now.Millisecond.ToString() + ".jpg";

                            // points = signature.Points;

                            var client = new RestClient(Constants.sp_SaveSiteImagesTEST + fileName + "?api_key=b5cb82af7b5d4130f36149f90aa2746782e59a872ac70454ac188743cb55b0ba");
                           string userName = "app@itgalaxy.co.za";
                           string passWord = "Internet1@#";
                           var request = new RestRequest(Method.POST);
                           request.AddHeader("Content-Type", "image/jpg");
                           Bitmap images = signature.GetImage();
                          
                           int bytes = images.ByteCount;
                           ByteBuffer buffer = ByteBuffer.Allocate(bytes); //Create a new buffer
                           images.CopyPixelsToBuffer(buffer); //Move the byte data to the buffer
                           byte[] array = new byte[buffer.Remaining()];
                           buffer.Get(array);

                           request.AddParameter("body", bytes, ParameterType.RequestBody);
                           var response = client.Execute(request);
                           if (response.StatusDescription == "Created")
                           {

                               client = new RestClient(Constants.sp_SaveSiteAttendSignature);
                               request = new RestRequest(Method.POST);
                               request.AddHeader("Content-Type", "application/zip");
                               client.Authenticator = new HttpBasicAuthenticator(userName, passWord);

                               request.RequestFormat = DataFormat.Json;
                               request.AddHeader("Accept", "application/json");
                               request.AddParameter("application/json", "{ \"params\": [    { \"name\": \"DocumentName\",      \"param_type\": \"IN\",      \"value\": \"" + fileName + "\",      \"type\": \"string\"    },{      \"name\": \"FullName\",      \"param_type\": \"IN\",      \"value\": \"" + sigNameS.Text + "\",      \"type\": \"string\"    },{      \"name\": \"Designation\",      \"param_type\": \"IN\",      \"value\": \"" + sigDesignS.Text + "\",      \"type\": \"string\"    },{      \"name\": \"ContactNumber\",      \"param_type\": \"IN\",      \"value\": \"" + sigInstS.Text + "\",      \"type\": \"string\"    },{      \"name\": \"CreatedBy\",      \"param_type\": \"IN\",      \"value\": \"" + Constants.Username + "\",      \"type\": \"string\"    },{      \"name\": \"SiteId\",      \"param_type\": \"IN\",      \"value\": \"" + ids + "\",      \"type\": \"int\"    }]}", ParameterType.RequestBody);
                               response = client.Execute(request);
                               List<ResultSet> jj = JsonConvert.DeserializeObject<List<ResultSet>>(response.Content);
                               if (jj.Count > 0)
                               {
                                   progressDialog.Cancel();
                                   AlertDialog dialog = null;
                                   var pop = new AlertDialog.Builder(this);
                                   pop.SetMessage("Saved Successfull.");
                                   pop.SetTitle("Message");
                                   pop.SetNeutralButton("OK", delegate { OnDismissSuccessAttendSignOffsAlert(dialog, ids, ""); });
                                   dialog = pop.Create();
                                   dialog.Show();

                               }
                               else
                               {
                                   progressDialog.Cancel();
                                   AlertDialog dialog = null;
                                   var pop = new AlertDialog.Builder(this);
                                   pop.SetMessage("Save unsuccessfull - try again.");
                                   pop.SetTitle("Message");
                                   pop.SetNeutralButton("OK", delegate { OnDismissSuccessAttendSignOffsAlert(dialog, ids, ""); });
                                   dialog = pop.Create();
                                   dialog.Show();
                               }

                           }
                           else
                           {

                               AlertDialog dialog = null;
                               var pop = new AlertDialog.Builder(this);
                               pop.SetMessage("Save unsuccessfull - try again.");
                               pop.SetTitle("Message");
                               pop.SetNeutralButton("OK", delegate { OnDismissSuccessAttendSignOffsAlert(dialog, ids, ""); });
                               dialog = pop.Create();
                               dialog.Show();
                           }


                       }
                       catch (Exception ex)
                       {
                           progressDialog.Cancel();
                           Log.Info("SignatureSave", @"				Exception {0}", ex.Message);

                           Console.WriteLine(@"				ERROR {0}", ex.Message);
                           AlertDialog dialog = null;
                           var pop = new AlertDialog.Builder(this);
                           pop.SetMessage("Save unsuccessfull - try again.");
                           pop.SetTitle("Message");
                           pop.SetNeutralButton("OK", delegate { OnDismissSuccessAttendSignOffsAlert(dialog, ids, ""); });
                           dialog = pop.Create();
                           dialog.Show();
                       }
                   }
               }
               progressDialog.Cancel();
           };
            progressDialog.Cancel();
        }
        private async Task<string> LoadSaveImage2(String path,byte[] imageData, string fileName,string siteId)
        {
            var progressDialog = ProgressDialog.Show(this, "", "Loading ...", true);
            string retu = "0";
            try
            {
               
                progressDialog.Indeterminate = true;
                progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);

                await Task.Run(() =>

                {

                    var client = new RestClient(Constants.sp_SaveSiteImagesTEST + fileName +"?api_key=b5cb82af7b5d4130f36149f90aa2746782e59a872ac70454ac188743cb55b0ba");
                    string userName = "app@itgalaxy.co.za";
                    string passWord = "Internet1@#";
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "image/jpg");
                    client.Authenticator = new HttpBasicAuthenticator(userName, passWord);

                    request.AddParameter("body", imageData, ParameterType.RequestBody);
                    var response = client.Execute(request);

                    if (response.StatusDescription == "Created")

                    {
                        client = new RestClient(Constants.sp_SaveSiteImages);
                        request = new RestRequest(Method.POST);
                        request.AddHeader("Content-Type", "application/zip");
                        client.Authenticator = new HttpBasicAuthenticator(userName, passWord);

                        request.RequestFormat = DataFormat.Json;
                        request.AddHeader("Accept", "application/json");
                        request.AddParameter("application/json", "{ \"params\": [    {      \"name\": \"SiteId\",      \"param_type\": \"IN\",      \"value\": \"" + siteId + "\",      \"type\": \"int\"    },{      \"name\": \"DocumentName\",      \"param_type\": \"IN\",      \"value\": \"" + fileName + "\",      \"type\": \"string\"    },{      \"name\": \"CreatedBy\",      \"param_type\": \"IN\",      \"value\": \"" + Constants.Username + "\",      \"type\": \"string\"    }]}", ParameterType.RequestBody);

                        response = client.Execute(request);
                        List<ResultSet> jj = JsonConvert.DeserializeObject<List<ResultSet>>(response.Content);
                        if (jj.Count > 0)
                          {
                          retu = jj[0].result;
                          }
                 
                    }
                   // Log.Info("Re", @"				LoadSaveAsset {0}", response.Content.ToString());

                  //  List<ResultSet> jj = JsonConvert.DeserializeObject<List<ResultSet>>(response.Content);

                   // if (jj.Count > 0)
                  //  {
                      //  retu = jj[0].result;
                  //  }
                    progressDialog.Dismiss();
                    return retu;
                });
                progressDialog.Dismiss();
                return retu;
            }
            catch (Exception ex)
            {
                progressDialog.Dismiss();
                Log.Info("LoadSaveImage", @"				ERROR {0}", ex.Message);

                Console.WriteLine(@"			LoadSaveImage	ERROR {0}", ex.Message);
                return null;

            }

        }
        private async Task<List<SchoolAssetRegisters>> FetchAssetsByAssetMatch(string ids)
        {
            List<SchoolAssetRegisters> items = new List<SchoolAssetRegisters>();
            try
            {
                await Task.Run(() =>

                {
                    var client = new RestClient(Constants.sp_getAssetListItems);
                    string userName = "app@itgalaxy.co.za";
                    string passWord = "Internet1@#";
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/zip");
                    client.Authenticator = new HttpBasicAuthenticator(userName, passWord);

                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("Accept", "application/json");
                    request.AddParameter("application/json", "{ \"params\": [    {      \"name\": \"Id\",      \"param_type\": \"IN\",      \"value\": \"" + ids + "\",      \"type\": \"string\"    }]}", ParameterType.RequestBody);

                    var response = client.Execute(request);

                    Log.Info("Re", @"				FetchAssetsBySiteId {0}", response.Content.ToString());

                    List<SchoolAssetRegisters> jj = JsonConvert.DeserializeObject<List<SchoolAssetRegisters>>(response.Content);

                    items = jj;

                });
                return items;

            }
            catch (Exception ex)
            {


                Log.Info("FetchAssetsBySiteId", @"				ERROR {0}", ex.Message);

                Console.WriteLine(@"				ERROR {0}", ex.Message);
                return null;

            }
        }
        private async Task<List<SchoolAssetRegisters>> FetchAssetsBySiteId(string ids) {
            List<SchoolAssetRegisters> items = new List<SchoolAssetRegisters>();
            try
            {
                await Task.Run(() =>

                {
                    var client = new RestClient(Constants.sp_MobileSiteAssets);
                    string userName = "app@itgalaxy.co.za";
                    string passWord = "Internet1@#";
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/zip");
                    client.Authenticator = new HttpBasicAuthenticator(userName, passWord);

                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("Accept", "application/json");
                    request.AddParameter("application/json", "{ \"params\": [    {      \"name\": \"SiteId\",      \"param_type\": \"IN\",      \"value\": \"" + ids + "\",      \"type\": \"string\"    }]}", ParameterType.RequestBody);

                    var response = client.Execute(request);
                    if (!response.Content.Contains("[]"))
                    {
                        Log.Info("Re", @"				FetchAssetsBySiteId {0}", response.Content.ToString());

                        List<SchoolAssetRegisters> jj = JsonConvert.DeserializeObject<List<SchoolAssetRegisters>>(response.Content);

                        items = jj;
                    }
                });
                return items;

            }
            catch (Exception ex)
            {


                Log.Info("FetchAssetsBySiteId", @"				ERROR {0}", ex.Message);

                Console.WriteLine(@"				ERROR {0}", ex.Message);
                return null;

            }
        }
        private async Task<List<AttencenceRegister>> FetchAttendenceBySiteId(string ids)
        {
            List<AttencenceRegister> items = new List<AttencenceRegister>();
            try
            {
                await Task.Run(() =>

                {
                    var client = new RestClient(Constants.sp_GetAttendenceRegister);
                    string userName = "app@itgalaxy.co.za";
                    string passWord = "Internet1@#";
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/zip");
                    client.Authenticator = new HttpBasicAuthenticator(userName, passWord);

                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("Accept", "application/json");
                    request.AddParameter("application/json", "{ \"params\": [    {      \"name\": \"SiteId\",      \"param_type\": \"IN\",      \"value\": \"" + ids + "\",      \"type\": \"string\"    }]}", ParameterType.RequestBody);

                    var response = client.Execute(request);
                    if (!response.Content.Contains("[]") )
                    {
                        Log.Info("Re", @"				FetchAssetsBySiteId {0}", response.Content.ToString());

                    List<AttencenceRegister> jj = JsonConvert.DeserializeObject<List<AttencenceRegister>>(response.Content);
                    
                   
                        items = jj;
                    }
                });
                if (items.Count > 0) {
                    return items;
                }
                return null;

            }
            catch (Exception ex)
            {


                Log.Info("FetchAssetsBySiteId", @"				ERROR {0}", ex.Message);

                Console.WriteLine(@"				ERROR {0}", ex.Message);
                return null;

            }
        }
        private async Task<List<SiteImageSignOffs>> FetchSitePicksBySiteId(string ids)
        {
            List<SiteImageSignOffs> items = new List<SiteImageSignOffs>();
            try
            {
                await Task.Run(() =>

                {
                    var client = new RestClient(Constants.sp_GetSiteImages);
                    string userName = "app@itgalaxy.co.za";
                    string passWord = "Internet1@#";
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/zip");
                    client.Authenticator = new HttpBasicAuthenticator(userName, passWord);

                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("Accept", "application/json");
                    request.AddParameter("application/json", "{ \"params\": [    {      \"name\": \"SiteId\",      \"param_type\": \"IN\",      \"value\": \"" + ids + "\",      \"type\": \"int\"    }]}", ParameterType.RequestBody);

                    var response = client.Execute(request);

                    Log.Info("Re", @"				FetchSitePicksBySiteId {0}", response.Content.ToString());

                    List<SiteImageSignOffs> jj = JsonConvert.DeserializeObject<List<SiteImageSignOffs>>(response.Content);

                    items = jj;

                });
                return items;

            }
            catch (Exception ex)
            {


                Log.Info("FetchSitePicksBySiteId", @"				ERROR {0}", ex.Message);

                Console.WriteLine(@"				ERROR {0}", ex.Message);
                return null;

            }
        }
        private async Task loadSite(string ids)
        {

            try
            {
               
                var progressDialog = ProgressDialog.Show(this, "", "Loading ...", true);
                progressDialog.Indeterminate = true;
                progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                SetContentView(Resource.Layout.SiteDetails);
                List<Sites> listitems = await FetchSiteById(ids);
                TextView ProjectV = FindViewById<TextView>(Resource.Id.Project);
                TextView ClientV = FindViewById<TextView>(Resource.Id.Client);
                TextView SiteV = FindViewById<TextView>(Resource.Id.Site);

                TextView SiteStatusV = FindViewById<TextView>(Resource.Id.SiteStatus);

                TextView SiteIdV = FindViewById<TextView>(Resource.Id.SiteId);

                TextView SiteTelV = FindViewById<TextView>(Resource.Id.SiteTel);

                TextView ContactPersonV = FindViewById<TextView>(Resource.Id.ContactPerson);

                TextView PersonTelV = FindViewById<TextView>(Resource.Id.PersonTel);

                TextView LatV = FindViewById<TextView>(Resource.Id.Latitude);

                TextView LongV = FindViewById<TextView>(Resource.Id.Longitude);
                TextView Add1V = FindViewById<TextView>(Resource.Id.Address1);
                TextView Add2V = FindViewById<TextView>(Resource.Id.Address2);
                TextView Add3V = FindViewById<TextView>(Resource.Id.Address3);
                Button Directions = FindViewById<Button>(Resource.Id.DirectionsToSite);
                Button Install = FindViewById<Button>(Resource.Id.StartInstall);
                Button Edit = FindViewById<Button>(Resource.Id.EditDetails);
                Button Dialing = FindViewById<Button>(Resource.Id.dial);
                Button person = FindViewById<Button>(Resource.Id.person);
                Button notes = FindViewById<Button>(Resource.Id.notes);
                signatures = new List<Bitmap>();
                signedNames = new List<string>();
                instNames = new List<string>();
                ASiteBack = FindViewById<Button>(Resource.Id.allsitesBack);

                ASiteBack.Click += async (sender, e) =>

                {
                    await load();
                    // SetContentView(Resource.Layout.Sites);
                    
                };

                Directions.Click += async (sender, e) =>

                {
                    await LoadDirections();
                    // SetContentView(Resource.Layout.Sites);

                };

                Install.Click += async  (sender, e) =>

                {
                    await LoadInstall(ids, SiteV.Text.ToString());
                  

                };

                Edit.Click += async (sender, e) =>

                {
                    await load();
                    // SetContentView(Resource.Layout.Sites);

                };

                Dialing.Click += async (sender, e) =>

                {
                    await load();
                    // SetContentView(Resource.Layout.Sites);

                };

                person.Click += async (sender, e) =>

                {
                    await load();
                    // SetContentView(Resource.Layout.Sites);

                };

                notes.Click += async (sender, e) =>

                {
                    await LoadSiteNotes(SiteIdV.Text, SiteV.Text);
                    // SetContentView(Resource.Layout.Sites);

                };





                ActionBar.Title = "Site Details";
               

                if (listitems.Count >0) {
                    Constants.CurrentSiteId = listitems[0].Ids;
                    directi = listitems[0];
                    ProjectV.Text = listitems[0].Project;
                    ClientV.Text = listitems[0].Client;
                    SiteV.Text = listitems[0].Site;

                    SiteStatusV.Text = listitems[0].SiteStatus;

                    SiteIdV.Text = listitems[0].SiteId;

                    SiteTelV.Text = listitems[0].SiteTel;
                    ContactPersonV.Text = listitems[0].ContactPerson;

                    PersonTelV.Text = listitems[0].PersonTel;

                    LatV.Text = listitems[0].Latitude;

                    LongV.Text = listitems[0].Longitude;
                    Add1V.Text = listitems[0].Address1;
                    Add2V.Text = listitems[0].Address2;
                    Add3V.Text = listitems[0].Address3;
                    Constants.SignOffTemplate = listitems[0].SignOffTemplateId;
                }

                progressDialog.Dismiss();
                // lis.SetAdapter(new ArrayAdapter(this, Resource.Id.list, listitems));
                //  lis.Adapter = 
            }
            catch (System.Exception ex)
            {


                Log.Info("FetchSitesAsync", @"				ERROR {0}", ex.Message);

                Console.WriteLine(@"				ERROR {0}", ex.Message);

            }



        }
        private async Task<List<SiteList>> FetchSitesAsync(string userId)
        {
           
            try
            {
                await Task.Run(() =>

                {
                 var client = new RestClient(Constants.sp_LoadMobileUserAssignedSites);
                string userName = "app@itgalaxy.co.za";
                string passWord = "Internet1@#";
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/zip");
                client.Authenticator = new HttpBasicAuthenticator(userName, passWord);

                request.RequestFormat = DataFormat.Json;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("application/json", "{ \"params\": [    {      \"name\": \"UserId\",      \"param_type\": \"IN\",      \"value\": \"" + userId + "\",      \"type\": \"string\"    }]}", ParameterType.RequestBody);

                var response = client.Execute(request);
              
                Log.Info("Re", @"				FetchSitesAsync {0}", response.Content.ToString());
               
                List<SiteList> jj = JsonConvert.DeserializeObject<List<SiteList>>(response.Content);

                    items = jj;
   
                });
                return items;

            }
            catch (Exception ex)
            {


                Log.Info("FetchSitesAsync", @"				ERROR {0}", ex.Message);

                Console.WriteLine(@"				ERROR {0}", ex.Message);
                return null;

            }
        }
        private async Task<List<SiteList>> FetchAllSitesAsync(string userId)
        {
            items = new List<SiteList>();
            try
            {
                await Task.Run(() =>

                {
                    var client = new RestClient(Constants.sp_LoadMobileUserSitesAll);
                    string userName = "app@itgalaxy.co.za";
                    string passWord = "Internet1@#";
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/zip");
                    client.Authenticator = new HttpBasicAuthenticator(userName, passWord);

                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("Accept", "application/json");
                    request.AddParameter("application/json", "{ \"params\": [    {      \"name\": \"UserId\",      \"param_type\": \"IN\",      \"value\": \"" + userId + "\",      \"type\": \"string\"    }]}", ParameterType.RequestBody);

                    var response = client.Execute(request);
              
                    List<SiteList> jj = JsonConvert.DeserializeObject<List<SiteList>>(response.Content);


                    items = jj;
   
                });
                return items;

            }
            catch (Exception ex)
            {


                Log.Info("FetchAllSitesAsync", @"				Exception {0}", ex.Message);

                Console.WriteLine(@"				ERROR {0}", ex.Message);
                return null;

            }
        }
        private async Task<List<SiteList>> FetchIssueSitesAsync(string userId)
        {
          
            try
            {
                await Task.Run(() =>

                {
                    var client = new RestClient(Constants.sp_LoadMobileUserAssignedIssueSites);
                    string userName = "app@itgalaxy.co.za";
                    string passWord = "Internet1@#";
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/zip");
                    client.Authenticator = new HttpBasicAuthenticator(userName, passWord);

                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("Accept", "application/json");
                    request.AddParameter("application/json", "{ \"params\": [    {      \"name\": \"UserId\",      \"param_type\": \"IN\",      \"value\": \"" + userId + "\",      \"type\": \"string\"    }]}", ParameterType.RequestBody);

                    var response = client.Execute(request);

                    Log.Info("Re", @"				sp_LoadMobileUserAssignedIssueSites {0}", response.Content.ToString());

                    List<SiteList> jj2 = JsonConvert.DeserializeObject<List<SiteList>>(response.Content);

                    items = jj2;



                });
                return items;

            }
            catch (Exception ex)
            {


                Log.Info("sp_LoadMobileUserAssignedIssueSites", @"				ERROR {0}", ex.Message);

                Console.WriteLine(@"				ERROR {0}", ex.Message);
                return null;

            }
        }
        private async Task<List<Sites>> FetchSiteById(string ids)
        {
            Siteitems = new List<Sites>();
            try
            {
                await Task.Run(() =>

                {
                    var client = new RestClient(Constants.sp_MobilSiteDetails);
                    string userName = "app@itgalaxy.co.za";
                    string passWord = "Internet1@#";
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/zip");
                    client.Authenticator = new HttpBasicAuthenticator(userName, passWord);

                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("Accept", "application/json");
                    request.AddParameter("application/json", "{ \"params\": [    {      \"name\": \"SiteId\",      \"param_type\": \"IN\",      \"value\": \"" + ids + "\",      \"type\": \"string\"    }]}", ParameterType.RequestBody);

                    var response = client.Execute(request);

                    Log.Info("FetchSiteById", @"Output {0}", response.Content);

                    List<Sites> jj = JsonConvert.DeserializeObject<List<Sites>>(response.Content);


                    Siteitems = jj;

                });
                return Siteitems;

            }
            catch (Exception ex)
            {


                Log.Info("FetchSiteById", @"				Exception {0}", ex.Message);

                Console.WriteLine(@"				ERROR {0}", ex.Message);
                return null;

            }
        }
        private async Task<List<SignOffsTemplate>> FetchSiteTemplateById(string ids)
        {
            List<SignOffsTemplate> SiteitemsTemplate = new List<SignOffsTemplate>();
            try
            {
                await Task.Run(() =>

                {
                    var client = new RestClient(Constants.sp_LoadSignOffsTemplate);
                    string userName = "app@itgalaxy.co.za";
                    string passWord = "Internet1@#";
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/zip");
                    client.Authenticator = new HttpBasicAuthenticator(userName, passWord);

                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("Accept", "application/json");
                    request.AddParameter("application/json", "{ \"params\": [    {      \"name\": \"TemplateId\",      \"param_type\": \"IN\",      \"value\": \"" + Constants.SignOffTemplate.ToString() + "\",      \"type\": \"int\"    }]}", ParameterType.RequestBody);

                    var response = client.Execute(request);

                    Log.Info("FetchSiteTemplateById", @"Output {0}", response.Content);

                    List<SignOffsTemplate> jj = JsonConvert.DeserializeObject<List<SignOffsTemplate>>(response.Content);


                    SiteitemsTemplate = jj;

                });
                return SiteitemsTemplate;

            }
            catch (Exception ex)
            {


                Log.Info("FetchSiteTemplateById", @"				Exception {0}", ex.Message);

                Console.WriteLine(@"				ERROR {0}", ex.Message);
                return null;

            }
        }



        private async Task LoadIssues(string ids, string SiteName)
        {
            var progressDialog = ProgressDialog.Show(this, "", "Loading ...", true);
            progressDialog.Indeterminate = true;
            progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);

            SetContentView(Resource.Layout.Issuetracker);
            ActionBar.Title = "Site Connectivity";
            List<SiteIssues> listitems = await FetchIssueBySiteId(ids);
            List<string> Displaylistitems = new List<string>();
            ListView listV = FindViewById<ListView>(Resource.Id.listAssets);
            if (listitems.Count > 0)
            {
                List<string> AdapterList = new List<string>();
                foreach (var item in listitems)
                {
                    Displaylistitems.Add("Issue Subject: " + item.Issue + " \n Issue Description: " + item.Memo );
                }


                ArrayAdapter adapter = new ArrayAdapter(
                                    this, //Context, typically the Activity
                                    Android.Resource.Layout.SimpleListItem1, //The layout. How the data will be presented 
                                    Displaylistitems //The enumerable data
                                );
                listV.Adapter = adapter;

            }


            Button Back = FindViewById<Button>(Resource.Id.IssueBack);
            Back.Click += async (sender, e) =>

            {
                await LoadInstall(ids, SiteName);
                // SetContentView(Resource.Layout.Sites);
            };

            Button Add = FindViewById<Button>(Resource.Id.AddIssue);
            Add.Click += async (sender, e) =>

            {
                await LoadIssueAdd(ids, SiteName);


            };
            progressDialog.Dismiss();
        }
        private async Task LoadIssueAdd(string ids, string SiteName)
        {
            SetContentView(Resource.Layout.AddIssuetracker);
            ActionBar.Title = "New Issue";
            Button Back = FindViewById<Button>(Resource.Id.IssueSaveBack);
            Button save = FindViewById<Button>(Resource.Id.IssueSave);
            EditText IssueSubjects = FindViewById<EditText>(Resource.Id.IssueSubject);
            EditText IssueDetail = FindViewById<EditText>(Resource.Id.IssueDetails);
            

            Back.Click += async (sender, e) =>

            {

                await LoadInstall(ids, SiteName);

            };


            save.Click += async (sender, e) =>

            {

                SiteIssues ss = new SiteIssues();
                ss.Issue = IssueSubjects.Text;
                ss.Memo = IssueDetail.Text;
                ss.SchoolID = ids;
                ss.IssueTracker_School = ids;


                string result = await LoadSaveIssue(ss);

                if (result == "1")
                {
                    var pop = new AlertDialog.Builder(this);
                    AlertDialog dialog = null;
                    pop.SetMessage("Save was a Success!");
                    pop.SetTitle("Save Message");

                    pop.SetNeutralButton("OK", delegate { OnDismissAlert(dialog); });
                    dialog = pop.Create();
                    dialog.Show();
                    await LoadIssues(ids, SiteName);
                }
                else
                {

                    var pop = new AlertDialog.Builder(this);
                    AlertDialog dialog = null;
                    pop.SetMessage("Save was a unsuccessfull! Try again or check your values.");
                    pop.SetTitle("Save Message");
                    pop.SetNeutralButton("OK", delegate { OnDismissAlert(dialog); });
                    dialog = pop.Create();
                    dialog.Show();
                }
                // SetContentView(Resource.Layout.Sites);
            };
        }
        private async Task<string> LoadSaveIssue(SiteIssues nn)
        {
            string retu = "0";
            var progressDialog = ProgressDialog.Show(this, "", "Loading ...", true);
            progressDialog.Indeterminate = true;
            progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            try
            {
               

                await Task.Run(() =>

                {

                    var client = new RestClient(Constants.sp_InsertIssueInfo);
                    string userName = "app@itgalaxy.co.za";
                    string passWord = "Internet1@#";
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/zip");
                    client.Authenticator = new HttpBasicAuthenticator(userName, passWord);

                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("Accept", "application/json");

                    string ParText = "{      \"name\": \"Issue\",      \"param_type\": \"IN\",      \"value\": \"" + nn.Issue + "\",      \"type\": \"string\"    },";
                    ParText += "{      \"name\": \"Memo\",      \"param_type\": \"IN\",      \"value\": \"" + Regex.Replace(nn.Memo, @"\r\n?|\n", " ")+ "\",      \"type\": \"string\"    },";
                    ParText += "{      \"name\": \"SchoolId\",      \"param_type\": \"IN\",      \"value\": \"" + nn.SchoolID + "\",      \"type\": \"string\"    },";
                    ParText += "{      \"name\": \"CreatedBy\",      \"param_type\": \"IN\",      \"value\": \"" + Constants.Username + "\",      \"type\": \"string\"    },";
                    ParText += "{      \"name\": \"IssueTracker_School\",      \"param_type\": \"IN\",      \"value\": \"" + nn.IssueTracker_School + "\",      \"type\": \"string\"    }";
                   
                    request.AddParameter("application/json", "{ \"params\": [" + ParText + "]}", ParameterType.RequestBody);

                    var response = client.Execute(request);

                    Log.Info("Re", @"				LoadSaveIssue {0}", response.Content.ToString());

                    List<ResultSet> jj = JsonConvert.DeserializeObject<List<ResultSet>>(response.Content);

                    if (jj.Count > 0)
                    {
                        retu = jj[0].result;
                    }
                    progressDialog.Dismiss();
                    return retu;
                });
                progressDialog.Dismiss();
                return retu;
            }
            catch (Exception ex)
            {
                progressDialog.Dismiss();
                Log.Info("LoadSaveIssue", @"				ERROR {0}", ex.Message);

                Console.WriteLine(@"				ERROR {0}", ex.Message);
                return null;

            }

        }
        private async Task<List<SiteIssues>> FetchIssueBySiteId(string ids)
        {
            List<SiteIssues> items = new List<SiteIssues>();
            try
            {
                await Task.Run(() =>

                {
                    var client = new RestClient(Constants.sp_GetIssueInfo);
                    string userName = "app@itgalaxy.co.za";
                    string passWord = "Internet1@#";
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/zip");
                    client.Authenticator = new HttpBasicAuthenticator(userName, passWord);

                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("Accept", "application/json");
                    request.AddParameter("application/json", "{ \"params\": [    {      \"name\": \"SiteId\",      \"param_type\": \"IN\",      \"value\": \"" + ids + "\",      \"type\": \"int\"    }]}", ParameterType.RequestBody);

                    var response = client.Execute(request);

                    Log.Info("Re", @"				FetchAssetsBySiteId {0}", response.Content.ToString());
                    if (!response.Content.Contains("[]"))
                    {
                        Log.Info("Re", @"				FetchAssetsBySiteId {0}", response.Content.ToString());

                        List<SiteIssues> jj = JsonConvert.DeserializeObject<List<SiteIssues>>(response.Content);


                        items = jj;
                    }

                });

                if (items != null)
                {
                    return items;
                }


                return null;

            }
            catch (Exception ex)
            {


                Log.Info("FetchAssetsBySiteId", @"				ERROR {0}", ex.Message);

                Console.WriteLine(@"				ERROR {0}", ex.Message);
                return null;

            }
        }

        private async Task LoadSiteNotes(string ids, string SiteName)
        {
            var progressDialog = ProgressDialog.Show(this, "", "Loading ...", true);
            progressDialog.Indeterminate = true;
            progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);

            SetContentView(Resource.Layout.SiteNotes);
            ActionBar.Title = "Site Notes";
            List<SiteNotes> listitems = await FetchSiteNotesById(ids);
            List<string> Displaylistitems = new List<string>();
            ListView listV = FindViewById<ListView>(Resource.Id.listAssets);
            if (listitems.Count > 0)
            {
                List<string> AdapterList = new List<string>();
                foreach (var item in listitems)
                {
                    Displaylistitems.Add("Created By: " + item.CreatedBy +" Created On:"+ item.Created + " \n Note: " + item.Notes);
                }


                ArrayAdapter adapter = new ArrayAdapter(
                                    this, //Context, typically the Activity
                                    Android.Resource.Layout.SimpleListItem1, //The layout. How the data will be presented 
                                    Displaylistitems //The enumerable data
                                );
                listV.Adapter = adapter;

            }


            Button Back = FindViewById<Button>(Resource.Id.SiteNotesBack);
            Back.Click += async (sender, e) =>

            {
                await LoadInstall(ids, SiteName);
                // SetContentView(Resource.Layout.Sites);
            };

            Button Add = FindViewById<Button>(Resource.Id.AddSiteNotes);
            Add.Click += async (sender, e) =>

            {
                await LoadSiteNotesAdd(ids, SiteName);


            };
            progressDialog.Dismiss();
        }
        private async Task LoadSiteNotesAdd(string ids, string SiteName)
        {
            SetContentView(Resource.Layout.AddSiteNotes);
            ActionBar.Title = "New Note";
            Button Back = FindViewById<Button>(Resource.Id.SiteNotesSaveBack);
            Button save = FindViewById<Button>(Resource.Id.SiteNotesSave);
            EditText Notes = FindViewById<EditText>(Resource.Id.SiteNotea);
          


            Back.Click += async (sender, e) =>

            {

                await LoadSiteNotes(ids, SiteName);

            };


            save.Click += async (sender, e) =>

            {

                SiteNotes ss = new SiteNotes();
                ss.Notes = Notes.Text;
                ss.SiteId = ids;
             

                string result = await LoadSaveSiteNotes(ss);

                if (result == "1")
                {
                    var pop = new AlertDialog.Builder(this);
                    AlertDialog dialog = null;
                    pop.SetMessage("Save was a Success!");
                    pop.SetTitle("Save Message");

                    pop.SetNeutralButton("OK", delegate { OnDismissAlert(dialog); });
                    dialog = pop.Create();
                    dialog.Show();
                    await LoadSiteNotes(ids, SiteName);
                }
                else
                {

                    var pop = new AlertDialog.Builder(this);
                    AlertDialog dialog = null;
                    pop.SetMessage("Save was a unsuccessfull! Try again or check your values.");
                    pop.SetTitle("Save Message");
                    pop.SetNeutralButton("OK", delegate { OnDismissAlert(dialog); });
                    dialog = pop.Create();
                    dialog.Show();
                }
                // SetContentView(Resource.Layout.Sites);
            };
        }
        private async Task<string> LoadSaveSiteNotes(SiteNotes nn)
        {
            string retu = "0";
            var progressDialog = ProgressDialog.Show(this, "", "Loading ...", true);
            progressDialog.Indeterminate = true;
            progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            try
            {


                await Task.Run(() =>

                {

                    var client = new RestClient(Constants.sp_InsertSiteNotes);
                    string userName = "app@itgalaxy.co.za";
                    string passWord = "Internet1@#";
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/zip");
                    client.Authenticator = new HttpBasicAuthenticator(userName, passWord);

                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("Accept", "application/json");

                    string ParText = "{      \"name\": \"SiteId\",      \"param_type\": \"IN\",      \"value\": \"" + nn.SiteId + "\",      \"type\": \"string\"    },";
                    ParText += "{      \"name\": \"CreatedBy\",      \"param_type\": \"IN\",      \"value\": \"" + Constants.Username  + "\",      \"type\": \"string\"    },";
                    ParText += "{      \"name\": \"Note\",      \"param_type\": \"IN\",      \"value\": \"" + Regex.Replace(nn.Notes, @"\r\n?|\n", " ") + "\",      \"type\": \"string\"    }";
                   
                    request.AddParameter("application/json", "{ \"params\": [" + ParText + "]}", ParameterType.RequestBody);

                    var response = client.Execute(request);

                    Log.Info("Re", @"				LoadSaveSiteNotes {0}", response.Content.ToString());

                    List<ResultSet> jj = JsonConvert.DeserializeObject<List<ResultSet>>(response.Content);

                    if (jj.Count > 0)
                    {
                        retu = jj[0].result;
                    }
                    progressDialog.Dismiss();
                    return retu;
                });
                progressDialog.Dismiss();
                return retu;
            }
            catch (Exception ex)
            {
                progressDialog.Dismiss();
                Log.Info("SiteNotes", @"				ERROR {0}", ex.Message);

                Console.WriteLine(@"				ERROR {0}", ex.Message);
                return null;

            }

        }
        private async Task<List<SiteNotes>> FetchSiteNotesById(string ids)
        {
            List<SiteNotes> items = new List<SiteNotes>();
            try
            {
                await Task.Run(() =>

                {
                    var client = new RestClient(Constants.sp_GetIssueInfo);
                    string userName = "app@itgalaxy.co.za";
                    string passWord = "Internet1@#";
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/zip");
                    client.Authenticator = new HttpBasicAuthenticator(userName, passWord);

                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("Accept", "application/json");
                    request.AddParameter("application/json", "{ \"params\": [    {      \"name\": \"SiteId\",      \"param_type\": \"IN\",      \"value\": \"" + ids + "\",      \"type\": \"int\"    }]}", ParameterType.RequestBody);

                    var response = client.Execute(request);

                    Log.Info("Re", @"				FetchAssetsBySiteId {0}", response.Content.ToString());
                    if (!response.Content.Contains("[]"))
                    {
                        Log.Info("Re", @"				FetchAssetsBySiteId {0}", response.Content.ToString());

                        List<SiteNotes> jj = JsonConvert.DeserializeObject<List<SiteNotes>>(response.Content);


                        items = jj;
                    }

                });

                if (items != null)
                {
                    return items;
                }


                return null;

            }
            catch (Exception ex)
            {


                Log.Info("FetchSiteNotesById", @"				ERROR {0}", ex.Message);

                Console.WriteLine(@"				ERROR {0}", ex.Message);
                return null;

            }
        }
    }

}