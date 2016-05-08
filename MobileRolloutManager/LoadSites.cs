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
using static Android.Widget.AdapterView;
using Android.Gms.Maps.Model;
using System.Globalization;
using Android.Locations;
using System.Threading;
using ZXing.Mobile;

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
                lis = FindViewById<ListView>(Resource.Id.list);
                lis2 = FindViewById<ListView>(Resource.Id.list2);
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
            Button AddAsset = FindViewById<Button>(Resource.Id.Assets);
            AddAsset.Click += async (sender, e) =>

            {
                await LoadAssets(ids, SiteNames);
                // SetContentView(Resource.Layout.Sites);

            };

            Button Back = FindViewById<Button>(Resource.Id.InstBack);
            Back.Click += async (sender, e) =>

            {
                await loadSite(ids);
                // SetContentView(Resource.Layout.Sites);

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
                    Serial.Text = result.Text;
                    var pop = new AlertDialog.Builder(this);
                    pop.SetMessage("Serial Scan will return an approximate match, edit the values if the item is not the same. If the model only returns the serial it means you will need to add the rest of the details manually in the add asset screen. ");
                    pop.SetTitle("Scanner Message");
                    AlertDialog dialog = pop.Create();

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

                string result = await LoadSaveAsset(ss);

                if (result == "1")
                {
                    var pop = new AlertDialog.Builder(this);
                    pop.SetMessage("Save was a Success!");
                    pop.SetTitle("Save Message");
                    AlertDialog dialog = pop.Create();
                    await LoadAssetsAdd(ids, SiteName);
                }
                else {
                    var pop = new AlertDialog.Builder(this);
                    pop.SetMessage("Save was a unsuccessfull! Try again or check your values.");
                    pop.SetTitle("Save Message");
                    AlertDialog dialog = pop.Create();
                }
                // SetContentView(Resource.Layout.Sites);
            };
        }

        private async Task<string> LoadSaveAsset(SchoolAssetRegisters nn)
        { string retu = "0";
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

                    string ParText = "{      \"name\": \"Item\",      \"param_type\": \"IN\",      \"value\": \"" + nn.Item + "\",      \"type\": \"string\"    }";
                    ParText += "{      \"name\": \"ItemDescription\",      \"param_type\": \"IN\",      \"value\": \"" + nn.ItemDescription + "\",      \"type\": \"string\"    }";
                    ParText += "{      \"name\": \"SerialCode\",      \"param_type\": \"IN\",      \"value\": \"" + nn.SerialNumber + "\",      \"type\": \"string\"    }";
                    ParText += "{      \"name\": \"Quantity\",      \"param_type\": \"IN\",      \"value\": \"" + nn.Quantity + "\",      \"type\": \"string\"    }";


                    request.AddParameter("application/json", "{ \"params\": ["+ParText+"]}", ParameterType.RequestBody);

                    var response = client.Execute(request);

                    Log.Info("Re", @"				LoadSaveAsset {0}", response.Content.ToString());

                    List<ResultSet> jj = JsonConvert.DeserializeObject<List<ResultSet>>(response.Content);

                    if (jj.Count > 0) {
                        retu = jj[0].result;
                    }
                    return retu;
                });

                return retu;
            }
            catch (Exception ex)
            {


                Log.Info("FetchAssetsBySiteId", @"				ERROR {0}", ex.Message);

                Console.WriteLine(@"				ERROR {0}", ex.Message);
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
                    request.AddParameter("application/json", "{ \"params\": [    {      \"name\": \"SiteId\",      \"param_type\": \"IN\",      \"value\": \"" + ids + "\",      \"type\": \"string\"    }]}", ParameterType.RequestBody);

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
                    await load();
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

        
      





    }

}