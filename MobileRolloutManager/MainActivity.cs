using System;
using Android.App;
using Android.Widget;
using Android.OS;
using System.Threading.Tasks;
using System.Json;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using Android.Util;
using System.Net.Http.Headers;
using RadialProgress;
using Android.Views;
using System.Threading;
using Android.Content.Res;
using System.Collections.Generic;
using Android.Content;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;
using Android.Graphics;


namespace MobileRolloutManager
{
    [Activity(Label = "MobileRolloutManager", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {

        CancellationTokenSource tokenSource;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
           
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            // Get our button from the layout resource,
            Button button = FindViewById<Button>(Resource.Id.MyButton);
            userText = FindViewById<EditText>(Resource.Id.user);
            passwordText = FindViewById<EditText>(Resource.Id.password);
            text = FindViewById<TextView>(Resource.Id.Text);
            text.SetTextColor(Android.Graphics.Color.Red);
            
            ActionBar.Title ="Mobile Rollout Manager";
        
            button.Click += async (sender, e) =>
            {


                tokenSource = new CancellationTokenSource();
                var progressDialog = ProgressDialog.Show(this, "", "Logging In...", true);
                progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                progressDialog.Indeterminate = true;
                try
                {
                  await OnLogin2(userText.Text, passwordText.Text);
                  progressDialog.Dismiss();
                }
                catch (Exception ex)
                {
                    progressDialog.Dismiss();
                    AlertDialog dialog = null;

                    var pop = new AlertDialog.Builder(this);
                    pop.SetMessage("Login Unsuccessfull" + ex.Message);
                    pop.SetTitle("Message");
                    pop.SetNeutralButton("OK", delegate { OnDismissAlert(dialog); });
                    dialog = pop.Create();

                    dialog.Show();
                    //Console.WriteLine(userText.Text + " " + passwordText.Text);
                    Log.Info("Debugginggg", @"				ERROR {0}", ex.Message);
                    
                }
                
                if (replies == "1") {
                  

                    Intent intent = new Intent(this,typeof(LoadSites)); // need to set your Intent View here

                     StartActivity(intent);
                   // this.FinishAndRemoveTask();
                 
                }

               
            };
        }
        private readonly TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        private TextView text;
        private EditText userText;
        private EditText passwordText;
        private string replies;

        private void OnDismissAlert(AlertDialog a)
        {
            a.Dismiss();
        }

        private async Task OnLogin2(string usernamea, string password)
        {
            
            try
            {
                await Task.Run(() => 
                {
                    var client = new RestClient(Constants.RestUrl);
                string userName = "app@itgalaxy.co.za";
                string passWord = "Internet1@#";
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/zip");
                client.Authenticator = new HttpBasicAuthenticator(userName, passWord);
            
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("Accept", "application/json");
                  request.AddParameter("application/json", "{ \"params\": [    {      \"name\": \"username\",      \"param_type\": \"IN\",      \"value\": \"" + usernamea + "\",      \"type\": \"string\"    }, {      \"name\": \"password\",      \"param_type\": \"IN\",      \"value\": \"" + password + "\",      \"type\": \"string\"    }  ]}", ParameterType.RequestBody);

                var response = client.Execute(request);
              

                Log.Info("Re", @"				ERROR {0}", response.Content.ToString());

                var jsonDoc = response.Content;
                JsonValue jsonDoc1 = response.Content;
                List<LoginDetails> jj = JsonConvert.DeserializeObject<List<LoginDetails>>(response.Content);

                if (jsonDoc.Length < 10)
               {
                    replies = "0";
                      

                  
               }
               else
               {
                   replies = "1";
                    Constants.UserIdd = jj[0].UserId.ToString();
                        Constants.Username = usernamea;
                }
          
                });
               
               
            }
            catch(Exception ex)
            {
               
                Log.Info("Debugginggg", @"				ERROR {0}", ex.Message);
           
                Console.WriteLine(@"				ERROR {0}", ex.Message);
               
                
            }
           

}

        

    }
}

