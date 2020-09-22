using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Notifications;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.Validation;
using FT_EClaim.Module.BusinessObjects;

namespace FT_EClaim.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class DeleteOnDismissControllers : ObjectViewController<DetailView, NotificationsObject>
    {
        private NotificationsService service;
        public DeleteOnDismissControllers()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            service = Application.Modules.FindModule<NotificationsModule>().NotificationsService;
            NotificationsDialogViewController notificationsDialogViewController =
    Frame.GetController<NotificationsDialogViewController>();
            if (service != null && notificationsDialogViewController != null)
            {
                notificationsDialogViewController.Dismiss.Executing += Dismiss_Executing;
                notificationsDialogViewController.Dismiss.Executed += Dismiss_Executed;
            }
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            NotificationsDialogViewController notificationsDialogViewController = Frame.GetController<NotificationsDialogViewController>();
            if (notificationsDialogViewController != null)
            {
                notificationsDialogViewController.Dismiss.Executing -= Dismiss_Executing;
                notificationsDialogViewController.Dismiss.Executed -= Dismiss_Executed;
            }
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
        private void Dismiss_Executing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            service.ItemsProcessed += Service_ItemsProcessed;
        }
        private void Service_ItemsProcessed(object sender, DevExpress.Persistent.Base.General.NotificationItemsEventArgs e)
        {
            IObjectSpace space = Application.CreateObjectSpace(typeof(MyNotifications));
            foreach (INotificationItem item in e.NotificationItems)
            {
                if (item.NotificationSource is MyNotifications)
                {
                    space.Delete(space.GetObject(item.NotificationSource));
                }
            }
            space.CommitChanges();
        }
        private void Dismiss_Executed(object sender, DevExpress.ExpressApp.Actions.ActionBaseEventArgs e)
        {
            service.ItemsProcessed -= Service_ItemsProcessed;
        }
    }
}
