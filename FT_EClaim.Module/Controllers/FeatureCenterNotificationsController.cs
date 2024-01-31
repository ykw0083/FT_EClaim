using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Notifications;
using DevExpress.ExpressApp.Scheduler;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FT_EClaim.Module.Notifications
{
    public class FeatureCenterNotificationsController : WindowController
    {
        internal INotificationsProvider schedulerProvider;
        internal INotificationsProvider defaultProvider;
        internal NotificationsController notificationController;

        protected override void OnActivated()
        {
            schedulerProvider = ((SchedulerModuleBase)Application.Modules.LastOrDefault(m => m is SchedulerModuleBase)).NotificationsProvider;
            defaultProvider = Application.Modules.FindModule<NotificationsModule>().DefaultNotificationsProvider;
            notificationController = Window.GetController<NotificationsController>();
            //if (!IsSchedulerNotificationInitialize.Value)
            //{
            //    schedulerProvider.NotificationTypesInfo.Remove(XafTypesInfo.Instance.FindTypeInfo(typeof(SchedulerNotifications)));
            //}
            //if (!IsCustomNotificationInitialize.Value)
            //{
            //    defaultProvider.NotificationTypesInfo.Remove(XafTypesInfo.Instance.FindTypeInfo(typeof(TaskWithNotifications)));
            //}
            if (!IsSchedulerNotificationInitialize.Value && !IsCustomNotificationInitialize.Value && notificationController != null)
            {
                notificationController.Active["ShowNotifications"] = true;
            }
        }
        public FeatureCenterNotificationsController()
        {
            TargetWindowType = WindowType.Main;
        }
        public Boolean? IsSchedulerNotificationInitialize
        {
            get
            {
                return ValueManager.GetValueManager<Boolean?>(nameof(IsSchedulerNotificationInitialize)).Value.HasValue ? ValueManager.GetValueManager<Boolean?>(nameof(IsSchedulerNotificationInitialize)).Value.Value : false;
            }
        }
        public Boolean? IsCustomNotificationInitialize
        {
            get
            {
                return ValueManager.GetValueManager<Boolean?>(nameof(IsCustomNotificationInitialize)).Value.HasValue ? ValueManager.GetValueManager<Boolean?>(nameof(IsCustomNotificationInitialize)).Value.Value : false;
            }
        }
    }

    //public class FeatureCenterSchedulerNotificationsViewController : ObjectViewController<ListView, SchedulerNotifications>
    //{
    //    FeatureCenterNotificationsController controller;
    //    protected override void OnActivated()
    //    {
    //        controller = Application.MainWindow.GetController<FeatureCenterNotificationsController>();
    //        controller.schedulerProvider.NotificationTypesInfo.Add(XafTypesInfo.Instance.FindTypeInfo(typeof(SchedulerNotifications)));
    //        controller.notificationController.Active["ShowNotifications"] = true;
    //        ValueManager.GetValueManager<Boolean?>("IsSchedulerNotificationInitialize").Value = true;
    //    }
    //}

    //public class FeatureCenterCustomNotificationsViewController : ObjectViewController<ListView, TaskWithNotifications>
    //{
    //    FeatureCenterNotificationsController controller;
    //    protected override void OnActivated()
    //    {
    //        controller = Application.MainWindow.GetController<FeatureCenterNotificationsController>();
    //        controller.defaultProvider.NotificationTypesInfo.Add(XafTypesInfo.Instance.FindTypeInfo(typeof(TaskWithNotifications)));
    //        controller.notificationController.Active["ShowNotifications"] = true; ;
    //        ValueManager.GetValueManager<Boolean?>("IsCustomNotificationInitialize").Value = true;
    //    }
    //}
}
