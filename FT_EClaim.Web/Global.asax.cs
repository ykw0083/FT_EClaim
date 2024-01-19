using System;
using System.Configuration;
using System.Web.Configuration;
using System.Web;

using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Web;
using DevExpress.Web;

namespace FT_EClaim.Web {
    public class Global : System.Web.HttpApplication {
        public Global() {
            InitializeComponent();
        }
        protected void Application_Start(Object sender, EventArgs e) {
            DevExpress.ExpressApp.FrameworkSettings.DefaultSettingsCompatibilityMode = DevExpress.ExpressApp.FrameworkSettingsCompatibilityMode.v20_1;
            #region adjust popupwindow behaviour
            //XafPopupWindowControl.DefaultHeight = Unit.Percentage(100);
            //XafPopupWindowControl.DefaultWidth = Unit.Percentage(100);
            //XafPopupWindowControl.PopupTemplateType = PopupTemplateType.FindDialog;
            //XafPopupWindowControl.ShowPopupMode = ShowPopupMode.Centered;
            #endregion

            SecurityAdapterHelper.Enable();
            ASPxWebControl.CallbackError += new EventHandler(Application_Error);
#if EASYTEST
            DevExpress.ExpressApp.Web.TestScripts.TestScriptsManager.EasyTestEnabled = true;
#endif
        }
        protected void Session_Start(Object sender, EventArgs e) {
            Tracing.Initialize();
            WebApplication.SetInstance(Session, new FT_EClaimAspNetApplication());
            DevExpress.ExpressApp.Web.Templates.DefaultVerticalTemplateContentNew.ClearSizeLimit();
            WebApplication.Instance.SwitchToNewStyle();
            #region listview column size adjustable
            WebApplication.OptimizationSettings.LockRecoverViewStateOnNavigationCallback = false;
            #endregion

            #region GeneralSettings
            string temp = "";

            temp = ConfigurationManager.AppSettings["EmailSend"].ToString();
            FT_EClaim.Module.GeneralSettings.EmailSend = false;
            if (temp.ToUpper() == "Y" || temp.ToUpper() == "YES" || temp.ToUpper() == "TRUE" || temp == "1")
                FT_EClaim.Module.GeneralSettings.EmailSend = true;

            FT_EClaim.Module.GeneralSettings.EmailHost = ConfigurationManager.AppSettings["EmailHost"].ToString();
            FT_EClaim.Module.GeneralSettings.EmailHostDomain = ConfigurationManager.AppSettings["EmailHostDomain"].ToString();
            FT_EClaim.Module.GeneralSettings.EmailPort = ConfigurationManager.AppSettings["EmailPort"].ToString();
            FT_EClaim.Module.GeneralSettings.Email = ConfigurationManager.AppSettings["Email"].ToString();
            FT_EClaim.Module.GeneralSettings.EmailPassword = ConfigurationManager.AppSettings["EmailPassword"].ToString();
            FT_EClaim.Module.GeneralSettings.EmailName = ConfigurationManager.AppSettings["EmailName"].ToString();

            temp = ConfigurationManager.AppSettings["EmailSSL"].ToString();
            FT_EClaim.Module.GeneralSettings.EmailSSL = false;
            if (temp.ToUpper() == "Y" || temp.ToUpper() == "YES" || temp.ToUpper() == "TRUE" || temp == "1")
                FT_EClaim.Module.GeneralSettings.EmailSSL = true;

            temp = ConfigurationManager.AppSettings["EmailUseDefaultCredential"].ToString();
            FT_EClaim.Module.GeneralSettings.EmailUseDefaultCredential = false;
            if (temp.ToUpper() == "Y" || temp.ToUpper() == "YES" || temp.ToUpper() == "TRUE" || temp == "1")
                FT_EClaim.Module.GeneralSettings.EmailUseDefaultCredential = true;

            FT_EClaim.Module.GeneralSettings.DeliveryMethod = ConfigurationManager.AppSettings["DeliveryMethod"].ToString();

            FT_EClaim.Module.GeneralSettings.B1Post = false; // no more real time posting due to diapi error
            temp = ConfigurationManager.AppSettings["B1Post"].ToString();
            FT_EClaim.Module.GeneralSettings.B1Post = false;
            if (temp.ToUpper() == "Y" || temp.ToUpper() == "YES" || temp.ToUpper() == "TRUE" || temp == "1")
                FT_EClaim.Module.GeneralSettings.B1Post = true;

            FT_EClaim.Module.GeneralSettings.LocalCurrency = ConfigurationManager.AppSettings["LocalCurrency"].ToString();

            FT_EClaim.Module.GeneralSettings.B1UserName = ConfigurationManager.AppSettings["B1UserName"].ToString();
            FT_EClaim.Module.GeneralSettings.B1Password = ConfigurationManager.AppSettings["B1Password"].ToString();
            FT_EClaim.Module.GeneralSettings.B1Server = ConfigurationManager.AppSettings["B1Server"].ToString();
            FT_EClaim.Module.GeneralSettings.B1CompanyDB = ConfigurationManager.AppSettings["B1CompanyDB"].ToString();
            FT_EClaim.Module.GeneralSettings.B1License = ConfigurationManager.AppSettings["B1License"].ToString();
            FT_EClaim.Module.GeneralSettings.SLDServer = ConfigurationManager.AppSettings["SLDServer"].ToString();
            FT_EClaim.Module.GeneralSettings.B1DbServerType = ConfigurationManager.AppSettings["B1DbServerType"].ToString();
            FT_EClaim.Module.GeneralSettings.B1Language = ConfigurationManager.AppSettings["B1Language"].ToString();
            FT_EClaim.Module.GeneralSettings.B1DbUserName = ConfigurationManager.AppSettings["B1DbUserName"].ToString();
            FT_EClaim.Module.GeneralSettings.B1DbPassword = ConfigurationManager.AppSettings["B1DbPassword"].ToString();
            FT_EClaim.Module.GeneralSettings.B1AttachmentPath = ConfigurationManager.AppSettings["B1AttachmentPath"].ToString();

            //FT_EClaim.Module.GeneralSettings.B1APIVseries = int.Parse(ConfigurationManager.AppSettings["B1APIVseries"].ToString());
            //FT_EClaim.Module.GeneralSettings.B1RefCol = ConfigurationManager.AppSettings["B1RefCol"].ToString();
            FT_EClaim.Module.GeneralSettings.B1DeptDimension = int.Parse(ConfigurationManager.AppSettings["B1DeptDimension"].ToString());
            FT_EClaim.Module.GeneralSettings.B1DivDimension = int.Parse(ConfigurationManager.AppSettings["B1DivDimension"].ToString());
            FT_EClaim.Module.GeneralSettings.B1BrandDimension = int.Parse(ConfigurationManager.AppSettings["B1BrandDimension"].ToString());

            FT_EClaim.Module.GeneralSettings.defaulttax = ConfigurationManager.AppSettings["DefaultTax"].ToString();
            FT_EClaim.Module.GeneralSettings.defaultmileagetax = ConfigurationManager.AppSettings["DefaultMileageTax"].ToString();
            FT_EClaim.Module.GeneralSettings.defemptyproject = ConfigurationManager.AppSettings["DefaultEmptyProject"].ToString();
            FT_EClaim.Module.GeneralSettings.defemptydepartment = ConfigurationManager.AppSettings["DefaultEmptyDepartment"].ToString();
            FT_EClaim.Module.GeneralSettings.defemptydivision = ConfigurationManager.AppSettings["DefaultEmptyDivision"].ToString();
            FT_EClaim.Module.GeneralSettings.defemptybrand = ConfigurationManager.AppSettings["DefaultEmptyBrand"].ToString();
            FT_EClaim.Module.GeneralSettings.defaultregion = ConfigurationManager.AppSettings["DefaultRegion"].ToString();

            FT_EClaim.Module.GeneralSettings.appurl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri; // + requestManager.GetQueryString(shortcut)
            #endregion
            WebApplication.Instance.CustomizeFormattingCulture += Instance_CustomizeFormattingCulture;
            WebApplication.Instance.LoggedOn += Instance_LoggedOn;

            if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null) {
                WebApplication.Instance.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            }
#if EASYTEST
            if(ConfigurationManager.ConnectionStrings["EasyTestConnectionString"] != null) {
                WebApplication.Instance.ConnectionString = ConfigurationManager.ConnectionStrings["EasyTestConnectionString"].ConnectionString;
            }
#endif
#if DEBUG
            if(System.Diagnostics.Debugger.IsAttached && WebApplication.Instance.CheckCompatibilityType == CheckCompatibilityType.DatabaseSchema) {
                WebApplication.Instance.DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
            }
#endif
            WebApplication.Instance.Setup();
            WebApplication.Instance.Start();
        }
        private void Instance_CustomizeFormattingCulture(object sender, CustomizeFormattingCultureEventArgs e)
        {

            e.FormattingCulture.NumberFormat.CurrencySymbol = "RM";
            e.FormattingCulture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
        }

        private void Instance_LoggedOn(object sender, LogonEventArgs e)
        {
            try
            {
                //XafPosV2.Module.BusinessObjects.SystemUsers user = (XafPosV2.Module.BusinessObjects.SystemUsers)SecuritySystem.CurrentUser;
                //user.CurOutlet = "";
                //user.CurOutlet = user.DefOutlet == null ? "" : user.DefOutlet.Code;
                //user.TrxDate = DateTime.Today;
                //user.DocDate = user.TrxDate;
                //user.DocOutlet = user.CurOutlet;

                //if (user.CurOutlet == "")
                //{
                //    WebApplication.LogOff(Session);
                //    WebApplication.DisposeInstance(Session);
                //}
            }
            catch (Exception ex)
            {
                WebApplication.LogOff(Session);
                WebApplication.DisposeInstance(Session);
            }
        }

        protected void Application_BeginRequest(Object sender, EventArgs e) {
        }
        protected void Application_EndRequest(Object sender, EventArgs e) {
        }
        protected void Application_AuthenticateRequest(Object sender, EventArgs e) {
        }
        protected void Application_Error(Object sender, EventArgs e) {
            ErrorHandling.Instance.ProcessApplicationError();
        }
        protected void Session_End(Object sender, EventArgs e) {
            WebApplication.LogOff(Session);
            WebApplication.DisposeInstance(Session);
        }
        protected void Application_End(Object sender, EventArgs e) {
        }
        #region Web Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
        }
        #endregion
    }
}
