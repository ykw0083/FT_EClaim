using System;
using DevExpress.DashboardWin;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Dashboards.Win;
using DevExpress.Persistent.Base;

namespace FT_EClaim.Module.Win.Controllers
{

    public class WinDashboardController : ObjectViewController<DetailView, IDashboardData>
    {
        private WinDashboardViewerViewItem dashboardViewerViewItem;
        protected override void OnActivated()
        {
            base.OnActivated();
            dashboardViewerViewItem = View.FindItem("DashboardViewer") as WinDashboardViewerViewItem;
            if (dashboardViewerViewItem != null)
            {
                if (dashboardViewerViewItem.Viewer != null)
                {
                    CustomizeDashboardViewer(dashboardViewerViewItem.Viewer);
                }
                else
                {
                    dashboardViewerViewItem.ControlCreated += DashboardViewerViewItem_ControlCreated;
                }
            }
        }
        private void DashboardViewerViewItem_ControlCreated(object sender, EventArgs e)
        {
            CustomizeDashboardViewer(((WinDashboardViewerViewItem)sender).Viewer);
        }
        private void CustomizeDashboardViewer(DashboardViewer dashboardViewer)
        {
            dashboardViewer.AllowPrintDashboardItems = true;
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
    }
}