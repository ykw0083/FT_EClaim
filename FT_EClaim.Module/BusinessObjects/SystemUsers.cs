using System;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.ExpressApp.ConditionalAppearance;

namespace FT_EClaim.Module.BusinessObjects
{
    [DefaultClassOptions]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class SystemUsers : PermissionPolicyUser
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public SystemUsers(Session session)
            : base(session)
        {
            session.IsObjectModifiedOnNonPersistentPropertyChange = true;
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            Company = Session.FindObject<Companies>(new BinaryOperator("BoCode", GeneralSettings.hq, BinaryOperatorType.Equal));
        }
        protected override void OnLoaded()
        {
            base.OnLoaded();
        }
        //private string _PersistentProperty;
        //[XafDisplayName("My display name"), ToolTip("My hint message")]
        //[ModelDefault("EditMask", "(000)-00"), Index(0), VisibleInListView(false)]
        //[Persistent("DatabaseColumnName"), RuleRequiredField(DefaultContexts.Save)]
        //public string PersistentProperty {
        //    get { return _PersistentProperty; }
        //    set { SetPropertyValue("PersistentProperty", ref _PersistentProperty, value); }
        //}

        //[Action(Caption = "My UI Action", ConfirmationMessage = "Are you sure?", ImageName = "Attention", AutoCommit = true)]
        //public void ActionMethod() {
        //    // Trigger a custom business logic for the current record in the UI (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112619.aspx).
        //    this.PersistentProperty = "Paid";
        //}

        private string _FullName;
        [XafDisplayName("User Full Name")]
        public string FullName
        {
            get { return _FullName; }
            set
            {
                SetPropertyValue("FullName", ref _FullName, value);
            }
        }

        private string _UserEmail;
        [XafDisplayName("User Email")]
        public string UserEmail
        {
            get { return _UserEmail; }
            set
            {
                SetPropertyValue("UserEmail", ref _UserEmail, value);
            }
        }


        private string _SAPUserID;
        [XafDisplayName("SAP User ID")]
        public string SAPUserID
        {
            get { return _SAPUserID; }
            set
            {
                SetPropertyValue("SAPUserID", ref _SAPUserID, value);
            }
        }

        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Browsable(false)]
        public string SAPPassword
        {
            get; set;
        }
        private string _TypePassword;
        [XafDisplayName("SAP User Password")]
        [NonPersistent]
        public string TypePassword
        {
            get { return _TypePassword; }
            set
            {
                SetPropertyValue("TypePassword", ref _TypePassword, value);
            }
        }

        private Companies _Company;
        [XafDisplayName("Company")]
        [RuleRequiredField(DefaultContexts.Save)]
        public Companies Company
        {
            get { return _Company; }
            set
            {
                SetPropertyValue("Company", ref _Company, value);
            }
        }

        [Browsable(false)]
        [Association("ApprovalTriggers")]
        [XafDisplayName("Trigger User")]
        public XPCollection<Approvals> TriggerApproval
        {
            get { return GetCollection<Approvals>("TriggerApproval"); }
        }
        [Browsable(false)]
        [Association("ApprovalUsers")]
        [XafDisplayName("Approve User")]
        public XPCollection<Approvals> UserApproval
        {
            get { return GetCollection<Approvals>("UserApproval"); }
        }

        [Association("SystemUsers-MyNotification")]
        [Appearance("MyNotification", Enabled = false)]
        public XPCollection<MyNotifications> MyNotification
        {
            get { return GetCollection<MyNotifications>("MyNotification"); }
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            if (!(Session is NestedUnitOfWork)
                && (Session.DataLayer != null)
                    && (Session.ObjectLayer is SimpleObjectLayer)
                        )
            {
            }

            if (!string.IsNullOrEmpty(TypePassword))
                SAPPassword = TypePassword;

        }
    }
}