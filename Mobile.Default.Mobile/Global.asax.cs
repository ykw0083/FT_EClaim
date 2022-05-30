using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using DevExpress.ExpressApp.ReportsV2.Mobile;
using DevExpress.XtraReports.Web.WebDocumentViewer;

namespace Mobile.Default.Mobile {
    public class Global : System.Web.HttpApplication {
        protected void Application_Start(Object sender, EventArgs e) {
            DevExpress.ExpressApp.FrameworkSettings.DefaultSettingsCompatibilityMode = DevExpress.ExpressApp.FrameworkSettingsCompatibilityMode.v20_1;
            DefaultWebDocumentViewerContainer.Register<IWebDocumentViewerReportResolver, XafReportsResolver<DefaultMobileApplication>>();
        }
		protected void Application_BeginRequest(object sender, EventArgs e) {
            CorsSupport.HandlePreflightRequest(HttpContext.Current);
        }
    }
}