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

using Android.Graphics;
using Java.IO;

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
        public static string sp_MobileSiteAssets = "http://197.189.239.202:8081/api/v2/rolloutman/_proc/sp_MobileSiteAssets?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88";
        public static string sp_getAssetListItems = "http://197.189.239.202:8081/api/v2/rolloutman/_proc/sp_getAssetListItems?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88";
        public static string sp_SaveAssets = "http://197.189.239.202:8081/api/v2/rolloutman/_proc/sp_SaveAssets?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88";
        public static string sp_GetSiteImages = "http://197.189.239.202:8081/api/v2/rolloutman/_proc/sp_GetSiteImages?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88";
        public static string sp_InsertPhotoINstallation = "http://197.189.239.202:8081/api/v2/rolloutman/_proc/sp_InsertPhotoINstallation?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88";
        public static string sp_SaveSiteImages = "http://197.189.239.202:8081/api/v2/rolloutman/_proc/sp_SaveSiteImages?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88";
        public static string sp_SaveSiteImagesTEST = "http://197.189.239.202:8081/api/v2/files/";
        public static string sp_SaveSiteSignature = "http://197.189.239.202:8081/api/v2/rolloutman/_proc/sp_SaveSiteSignature?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88";
        public static string sp_SaveSiteSignOff = "http://197.189.239.202:8081/api/v2/rolloutman/_proc/sp_SaveSiteSignOff?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88";
        public static string sp_LoadSignOffsTemplate = "http://197.189.239.202:8081/api/v2/rolloutman/_proc/sp_LoadSignOffsTemplate?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88";
        public static string sp_GetAttendenceRegister = "http://197.189.239.202:8081/api/v2/rolloutman/_proc/sp_GetAttendenceRegister?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88";
        public static string sp_SaveSiteAttendSignature = "http://197.189.239.202:8081/api/v2/rolloutman/_proc/sp_SaveSiteAttendSignature?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88";
        public static string sp_GetConnectivityInfo = "http://197.189.239.202:8081/api/v2/rolloutman/_proc/sp_GetConnectivityInfo?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88";
        public static string sp_InsertConnectivityInfo = "http://197.189.239.202:8081/api/v2/rolloutman/_proc/sp_InsertConnectivityInfo?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88";
        public static string sp_InsertIssueInfo = "http://197.189.239.202:8081/api/v2/rolloutman/_proc/sp_InsertIssueInfo?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88";
        public static string sp_GetIssueInfo = "http://197.189.239.202:8081/api/v2/rolloutman/_proc/sp_GetIssueInfo?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88";
        public static string sp_InsertSiteNotes = "http://197.189.239.202:8081/api/v2/rolloutman/_proc/sp_InsertSiteNotes?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88";
        public static string sp_GetSiteNotes = "http://197.189.239.202:8081/api/v2/rolloutman/_proc/sp_GetSiteNotes?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88";
        public static string sp_GetChangeTypes = "http://197.189.239.202:8081/api/v2/rolloutman/_proc/sp_GetChangeTypes?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88";
        public static string sp_GetChangeById = "http://197.189.239.202:8081/api/v2/rolloutman/_proc/sp_GetChangeById?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88";
        public static string sp_GetInsertChanges = "http://197.189.239.202:8081/api/v2/rolloutman/_proc/sp_GetInsertChanges?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88";
        public static string sp_SearchSites = "http://197.189.239.202:8081/api/v2/rolloutman/_proc/sp_SearchSites?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88";
        public static string sp_FinishSite = "http://197.189.239.202:8081/api/v2/rolloutman/_proc/sp_FinishSite?api_key=d72ee527465032b6e510ee14f0e0cedf27110f17d8824dd06be4cf4127363b88";

        // Credentials that are hard coded into the REST service
        public static string Username = "app@itgalaxy.co.za";
        public static string Password = "Internet1@#";
        public static string UserIdd = "";
        public static int CurrentSiteId = 0;
        public static int ShowOnceImg = 0;
        public static int SignOffTemplate = 0;
       
    }
}