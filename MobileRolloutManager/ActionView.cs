using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Util;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json;
using System.Globalization;

namespace MobileRolloutManager
{
    
    [Activity(Label = "ActionView")]
    public class ActionView : Activity, IOnMapReadyCallback
    {
        private List<String> items = new List<String>();
        private List<SiteMarkers> itemMarkerss = new List<SiteMarkers>();
        private Button mapBack;
        private GoogleMap _map;
        private MapFragment _mapFragment;
        private static readonly LatLng Passchendaele = new LatLng(50.897778, 3.013333);
        private static readonly LatLng VimyRidge = new LatLng(50.379444, 2.773611);
        private SupportMapFragment mapFragment;
        private string UserId = Constants.UserIdd;

        public async void OnMapReady(GoogleMap _map2)
        {
            _map = _map2;
            if (_map != null)
            {
                LatLngBounds.Builder builder = new LatLngBounds.Builder();
                itemMarkerss = await FetchSitesAsync(UserId);
               
                foreach (var site in itemMarkerss)
                {
                    MarkerOptions marker = new MarkerOptions();
                    LatLng pos = new LatLng(double.Parse(site.lats, CultureInfo.InvariantCulture), double.Parse(site.longs, CultureInfo.InvariantCulture));
                    marker.SetPosition(pos);
                    marker.SetTitle(site.MarkTitle);
                    if (site.markerColor == 2) { marker.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueGreen)); }
                    else
                    {
                        marker.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed));
                    }
                    _map.AddMarker(marker);
                    builder.Include(pos);
                }
                LatLngBounds bounds = builder.Build();
                // We create an instance of CameraUpdate, and move the map to it.
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngBounds(bounds, 25);
                _map.MoveCamera(cameraUpdate);
            }
        }




        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MappedAssignedSites);
            mapBack = FindViewById<Button>(Resource.Id.mapback);
            // Create your application here


            mapBack.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(LoadSites)); // need to set your Intent View here

                StartActivity(intent);
            };

            SetUpMapIfNeeded();
         
        }

        protected override void OnResume()
        {
            base.OnResume();
            SetUpMapIfNeeded();
        }


        private void SetUpMapIfNeeded()
        {
            if (null != _map) return;

            var frag = FragmentManager.FindFragmentById<MapFragment>(Resource.Id.mapF);
             frag.GetMapAsync(this);
         
        }

        private async Task<List<SiteMarkers>> FetchSitesAsync(string userId)
        {

            try
            {
                itemMarkerss = new List<SiteMarkers>();
                await Task.Run(() =>

                {
                    var client = new RestClient(Constants.sp_LoadMobileAssignedSitesMarkers);
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

                    List<SiteMarkers> jj = JsonConvert.DeserializeObject<List<SiteMarkers>>(response.Content);

                    itemMarkerss = jj;



                });
                return itemMarkerss;

            }
            catch (Exception ex)
            {


                Log.Info("FetchSitesAsync", @"				ERROR {0}", ex.Message);

                Console.WriteLine(@"				ERROR {0}", ex.Message);
                return null;

            }
        }


    }
}