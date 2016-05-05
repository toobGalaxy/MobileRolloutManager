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


namespace MobileRolloutManager
{
    [Activity(Label = "LoadSites")]
    public class LoadSites : Activity
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

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
     
         
            var progressDialog = ProgressDialog.Show(this, "", "Loading Sites...", true);
            progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            progressDialog.Indeterminate = true;
            
            //You can now use and reference the ActionBar
           
            //Do whatever  
            await load();
            
            progressDialog.Dismiss();
          

            // Create your application here
        }

        protected override void OnResume()
        {
            base.OnResume();
           
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
                 foreach (var itemL in listitems) {
                        AdapterList.Add("Id " + itemL.EmisNumber + " - " + itemL.SiteName);
                    }
                if (listitems2.Count > 0)
                {
                    foreach (var itemL in listitems2)
                    {
                        AdapterList2.Add("Id " + itemL.EmisNumber + " - " + itemL.SiteName);
                    }
                    ArrayAdapter adapters2 = new ArrayAdapter(
                                    this, //Context, typically the Activity
                                    Android.Resource.Layout.SimpleListItem1, //The layout. How the data will be presented 
                                    listitems2 //The enumerable data
                                );
                    lis2.Adapter = adapters2;
                    lis2.ItemClick += async (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
                    {
                        selectSite = listitems[e.Position].Id;
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

        private async Task loadSite(string ids)
        {

            try
            {

                var progressDialog = ProgressDialog.Show(this, "", "Loading ...", true);
                progressDialog.Indeterminate = true;
                progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                SetContentView(Resource.Layout.SiteDetails);

                ASiteBack = FindViewById<Button>(Resource.Id.allsitesBack);

                ASiteBack.Click += async (sender, e) =>

                {
                    await load();
                    // SetContentView(Resource.Layout.Sites);
                    
                };

                
                ActionBar.Title = "Site Details";
                List<Sites> listitems = await FetchSiteById(Constants.UserIdd);

                if (listitems.Count >0) {



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

                    List<Sites> jj = JsonConvert.DeserializeObject<List<Sites>>(response.Content);


                    Siteitems = jj;

                });
                return Siteitems;

            }
            catch (Exception ex)
            {


                Log.Info("FetchAllSitesAsync", @"				Exception {0}", ex.Message);

                Console.WriteLine(@"				ERROR {0}", ex.Message);
                return null;

            }
        }

        
      





    }

}