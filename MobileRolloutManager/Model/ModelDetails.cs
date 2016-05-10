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
using Android.Media;

namespace MobileRolloutManager
{
   public class SiteMarkers
    {
       
        public string lats { get; set; }
        public string longs { get; set; }
        public string MarkTitle { get; set; }
        public int markerColor { get; set; }

    }
    public class ResultSet
    {

        public string result { get; set; }
    }



        public class SiteMarkersList
    {
        public List<SiteMarkers> Sitemark { get; set; }

    }
    public class paramss
    {
       
        public string name { get; set; }
        public string param_type { get; set; }
        public string value { get; set; }
        public string type { get; set; }
        public string lenght { get; set; }
    }

    public class SiteList
    {
        public int Id { get; set; }
        public string SiteName { get; set; }
        public string EmisNumber { get; set; }
        public string ContactNumber { get; set; }
        public string PrincipalName { get; set; }
        public string SiteStatus { get; set; }
    }

    public class LoginDetails
    {
        public string UserId { get; set; }
        

    }

    public class SchoolAssetRegisters
    {
       public int Id { get; set; }
        public string Item { get; set; }
        public string ItemDescription { get; set; }
        public string SerialNumber { get; set; }
        public int Quantity { get; set; }
        public int SiteId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Created { get; set; }
    }

    public class SiteImageSignOffs {
        public int Id { get; set; }
        public string DocumentName { get; set; }
        public string Memo { get; set; }
        public int SchoolId { get; set; }
        public string CreatedBy { get; set; }
    }

    public class LoginList
    {
        public List<LoginDetails> Logins { get; set; }

    }


    public class Sites
    {
       public int Ids { get; set; }
        public string Client { get; set; }
        public string Project { get; set; }
        public string Site { get; set; }
        public string SiteStatus { get; set; }
       public string SiteId  { get; set; }
     
       public string SiteTel   { get; set; }
      
        public string ContactPerson   { get; set; }
        public string PersonTel { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public DateTime ScheduleDeliveryDate   { get; set; }
        public DateTime ScheduleInstallationDate { get; set; }
        public int ProjectNumber { get; set; }
    }

    public class SiteMaps
    {
        public int Id { get; set; }
        public string SchoolName { get; set; }
        public string EmisNumber { get; set; }
        public string ContactNumber { get; set; }
        public string PrincipalName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string SiteStatus { get; set; }
    }
}