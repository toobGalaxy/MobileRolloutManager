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

namespace MobileRolloutManager
{
    class Constants
    {
        public static string RestUrl2 = "http://developer.xamarin.com:8081/api/todoitems";
        public static string RestUrl = "http://197.189.239.202:8081/api/v2/rolloutman/_proc/sp_LoginMobileUser?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88";
        public static string sp_LoadMobileUserSitesAll = "http://197.189.239.202:8081/api/v2/rolloutman/_proc/sp_LoadMobileUserSitesAll?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88";
        public static string sp_LoadMobileUserAssignedSites = "http://197.189.239.202:8081/api/v2/rolloutman/_proc/sp_LoadMobileUserAssignedSites?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88";
        public static string sp_LoadMobileUserAssignedIssueSites = "http://197.189.239.202:8081/api/v2/rolloutman/_proc/sp_LoadMobileUserAssignedIssueSites?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88";
        public static string get_SchoolById = "http://197.189.239.202:8081/api/v2/rolloutman/_table/schools?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88?ids=";
        public static string sp_MobileMapAssignedSites = "http://197.189.239.202:8081/api/v2/rolloutman/_proc/sp_MobileMapAssignedSites?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88";
        public static string sp_LoadMobileAssignedSitesMarkers = "http://197.189.239.202:8081/api/v2/rolloutman/_proc/sp_LoadMobileAssignedSitesMarkers?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88";
        public static string sp_MobilSiteDetails = "http://197.189.239.202:8081/api/v2/rolloutman/_proc/sp_MobilSiteDetails?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88";
     
        // Credentials that are hard coded into the REST service
        public static string Username = "app@itgalaxy.co.za";
        public static string Password = "Internet1@#";
        public static string UserIdd = "";
    }
}