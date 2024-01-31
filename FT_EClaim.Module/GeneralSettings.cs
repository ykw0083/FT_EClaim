using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_EClaim.Module
{
    public class GeneralSettings
    {
        public static SAPbobsCOM.Company oCompany;
        public static string appurl = "";

        public const string defmileage = "M01";
        public const string hq = "HQ";
        
        public const string defclaimdoc = "CLAIM";
        public const string claimsuperrole = "ClaimSuperRole";
        public const string claimrole = "ClaimUserRole";
        public const string verifyrole = "VerifyUserRole";
        public const string Acceptancerole = "AcceptanceUserRole";
        public const string postrole = "PostUserRole";
        public const string ApprovalRole = "ApprovalUserRole";
        public const string ApprovalListRole = "ApprovalListUserRole";
        public const string RejectApproveRole = "RejectApproveRole";
        public static string defaulttax = "X0";
        public static string defaultmileagetax = "X0";
        public static string defemptyproject = "";
        public static string defemptydepartment = "";
        public static string defemptydivision = "";
        public static string defemptybrand = "";
        public static string defaultregion = "01";

        public static string LocalCurrency = "MYR";
        public static bool EmailSend;
        public static string EmailHost = "";
        public static string EmailHostDomain = "";
        public static string EmailPort = "";
        public static string Email = "";
        public static string EmailPassword = "";
        public static string EmailName = "";
        public static bool EmailSSL;
        public static bool EmailUseDefaultCredential;
        public static string DeliveryMethod = "";
        public static string SecurityProtocol = "";

        public static bool B1Post;
        public static string B1UserName = "";
        public static string B1Password = "";
        public static string B1Server = "";
        public static string B1CompanyDB = "";
        public static string B1License = "";
        public static string SLDServer = "";
        public static string B1Language = "";
        public static string B1DbServerType = "";
        public static string B1DbUserName = "";
        public static string B1DbPassword = "";
        public static string B1AttachmentPath = "";

        //public static int B1APIVseries = 0;
        //public static int B1JEseries = 0;
        //public static string B1RefCol = "";
        public static int B1DeptDimension = 0;
        public static int B1DivDimension = 0;
        public static int B1BrandDimension = 0;
    }
}
