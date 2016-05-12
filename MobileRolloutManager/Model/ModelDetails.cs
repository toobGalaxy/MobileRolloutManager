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

    public class ConnectivityModel
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public string SimCardNumber { get; set; }
        public string RouterImea { get; set; }
        public string RICA_Officer_Name { get; set; }
        public string Other_RICA_Info { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Created { get; set; }
    }

    public class SiteImageSignOffs {
        public int Id { get; set; }
        public string StoredDocument {get; set;}
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
        public int SignOffTemplateId { get; set; }

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

    public class SignOffs
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public bool Item1Check { get; set; }
        public bool Item2Check { get; set; }
        public bool Item3Check { get; set; }
        public bool Item4Check { get; set; }
        public bool Item5Check { get; set; }
        public bool Item6Check { get; set; }
        public bool Item7Check { get; set; }
        public bool Item8Check { get; set; }
        public bool Item9Check { get; set; }
        public bool Item10Check { get; set; }
        public bool Item11Check { get; set; }
        public bool Item12Check { get; set; }
        public bool Item13Check { get; set; }
        public bool Item14Check { get; set; }
        public bool Item15Check { get; set; }
        public string notes { get; set; }
    }

    public class SignOffsTemplate
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string SignoffType { get; set; }
        public string SignoffWording { get; set; }
              
        public string Item1Active { get; set; }
        public string Item2Active { get; set; }
        public string Item3Active { get; set; }
        public string Item4Active { get; set; }
        public string Item5Active { get; set; }
        public string Item6Active { get; set; }
        public string Item7Active { get; set; }
        public string Item8Active { get; set; }
        public string Item9Active { get; set; }
        public string Item10Active { get; set; }
        public string Item11Active { get; set; }
        public string Item12Active { get; set; }
        public string Item13Active { get; set; }
        public string Item14Active { get; set; }
        public string Item15Active { get; set; }
        public string Item1Description { get; set; }
        public string Item2Description { get; set; }
        public string Item3Description { get; set; }
        public string Item4Description { get; set; }
        public string Item5Description { get; set; }
        public string Item6Description { get; set; }
        public string Item7Description { get; set; }
        public string Item8Description { get; set; }
        public string Item9Description { get; set; }
        public string Item10Description { get; set; }
        public string Item11Description { get; set; }
        public string Item12Description { get; set; }
        public string Item13Description { get; set; }
        public string Item14Description { get; set; }
        public string Item15Description { get; set; }
    }



    public class signaturesMod
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Institution { get; set; }
        public string StoredImage { get; set; }
    }

    public class AttencenceRegister
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public string Created { get; set; }
        public string FullName { get; set; }
        public string ContactNumber { get; set; }
        public string Designation { get; set; }
    }

    public class SiteIssues
    {
        public int Id { get; set; }
        public string Issue { get; set; }
        public string Memo { get; set; }
        public string SchoolID { get; set; }
        public string Status { get; set; }
        public string Created { get; set; }
        public string IssueTracker_School { get; set; }
    }

    public class SiteNotes
    {
        public int Id { get; set; }
        public string SiteId { get; set; }
        public string Notes { get; set; }
        public string CreatedBy { get; set; }
        public string Created { get; set; }
    }

}