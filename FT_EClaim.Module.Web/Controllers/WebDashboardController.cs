using System;
using DevExpress.DashboardWeb;
using DevExpress.DataAccess.Web;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Dashboards.Web;
using DevExpress.Persistent.Base;

namespace FT_EClaim.Module.Web.Controllers
{
    public class WebDashboardController : ObjectViewController<DetailView, IDashboardData>
    {
        private WebDashboardViewerViewItem dashboardViewerViewItem;
        protected override void OnActivated()
        {
            base.OnActivated();
            dashboardViewerViewItem = View.FindItem("DashboardViewer") as WebDashboardViewerViewItem;
            if (dashboardViewerViewItem != null)
            {
                if (dashboardViewerViewItem.DashboardControl != null)
                {
                    ConfigureDashboard(dashboardViewerViewItem.DashboardControl);
                    SetHeight(dashboardViewerViewItem.DashboardControl);
                }
                else
                {
                    dashboardViewerViewItem.ControlCreated += DashboardViewerViewItem_ControlCreated;
                }
                //dashboardViewerViewItem.Refresh();
            }
        }
        private void DashboardViewerViewItem_ControlCreated(object sender, EventArgs e)
        {
            ConfigureDashboard(dashboardViewerViewItem.DashboardControl);
            SetHeight(((WebDashboardViewerViewItem)sender).DashboardControl);
            dashboardViewerViewItem.DashboardDesigner.ConfigureDataReloadingTimeout += DashboardDesigner_ConfigureDataReloadingTimeout;
        }
        private void DashboardDesigner_ConfigureDataReloadingTimeout(object sender, ConfigureDataReloadingTimeoutWebEventArgs e)
        {
            //if (e.DashboardId == "YOUR_DASHBOARD_ID")  
            e.DataReloadingTimeout = new TimeSpan(0, 1, 0);
        }
        private void SetHeight(ASPxDashboard dashboardControl)
        {
            dashboardControl.Height = 760;
        }
        protected override void OnDeactivated()
        {
            if (dashboardViewerViewItem != null)
            {
                dashboardViewerViewItem.ControlCreated -= DashboardViewerViewItem_ControlCreated;
                dashboardViewerViewItem = null;
            }
            base.OnDeactivated();
        }

        private void ConfigureDashboard(ASPxDashboard dashboardControl)
        {
            IDataSourceWizardConnectionStringsProvider provider = new ConfigFileConnectionStringsProvider();
            dashboardControl.SetConnectionStringsProvider(provider);
        }
    }
}