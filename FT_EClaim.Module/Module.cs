﻿using System;
using System.Text;
using System.Linq;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Notifications;
using DevExpress.Persistent.Base.General;
using FT_EClaim.Module.BusinessObjects;

namespace FT_EClaim.Module {
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppModuleBasetopic.aspx.
    public sealed partial class FT_EClaimModule : ModuleBase {
        public FT_EClaimModule() {
            InitializeComponent();
			BaseObject.OidInitializationMode = OidInitializationMode.AfterConstruction;
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            return new ModuleUpdater[] { updater };
        }
        public override void Setup(XafApplication application) {
            base.Setup(application);
            // Manage various aspects of the application UI and behavior at the module level.
            application.LoggedOn += new EventHandler<LogonEventArgs>(application_LoggedOn);
        }
        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            CalculatedPersistentAliasHelper.CustomizeTypesInfo(typesInfo);

            #region hide Oid
            var xpObjectTypeInfo = typesInfo.FindTypeInfo(typeof(DevExpress.Xpo.XPObject)) as TypeInfo;
            if (xpObjectTypeInfo != null)
            {
                var oidMemberInfo = xpObjectTypeInfo.FindMember(nameof(DevExpress.Xpo.XPObject.Oid));
                oidMemberInfo.AddAttribute(new VisibleInDetailViewAttribute(false));
                oidMemberInfo.AddAttribute(new VisibleInListViewAttribute(false));
                oidMemberInfo.AddAttribute(new VisibleInLookupListViewAttribute(false));
                ((XafMemberInfo)oidMemberInfo).Refresh();
            }
            #endregion
        }
        #region notification
        void application_LoggedOn(object sender, LogonEventArgs e)
        {
            NotificationsModule notificationsModule = Application.Modules.FindModule<NotificationsModule>();
            DefaultNotificationsProvider notificationsProvider = notificationsModule.DefaultNotificationsProvider;
            notificationsProvider.CustomizeNotificationCollectionCriteria += notificationsProvider_CustomizeNotificationCollectionCriteria;
        }
        void notificationsProvider_CustomizeNotificationCollectionCriteria(
            object sender, CustomizeCollectionCriteriaEventArgs e)
        {
            if (e.Type == typeof(MyNotifications))
            {
                e.Criteria = CriteriaOperator.Parse("AssignedTo is null || AssignedTo.Oid == CurrentUserId()");
            }
        }
        #endregion
    }
}
