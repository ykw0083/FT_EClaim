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
using DevExpress.ExpressApp.ConditionalAppearance;

namespace FT_EClaim.Module.BusinessObjects
{
    [DefaultClassOptions]
    //[ImageName("BO_Contact")]
    [NavigationItem("Claim Setup")]
    [DefaultProperty("BoFullName")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    [RuleCriteria("ClaimTypesDeleteRule", DefaultContexts.Delete, "1=0", "Cannot Delete.")]
    [RuleCriteria("ClaimTypesSaveRule", DefaultContexts.Save, "IsValid", "Either Note and Mileage need to be selected.")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class ClaimTypes : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public ClaimTypes(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            IsActive = true;
            IsNote = true;
            IsMileage = false;
            BoCode = "";
            BoName = "";
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

        private string _BoCode;
        [XafDisplayName("Code"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [RuleUniqueValue]
        //[Appearance("BoCode", Enabled = false, Criteria = "not IsNew")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(0)]
        public string BoCode
        {
            get { return _BoCode; }
            set
            {                
                SetPropertyValue("BoCode", ref _BoCode, value);
            }
        }

        private string _BoName;
        [XafDisplayName("Name"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(1)]
        public string BoName
        {
            get { return _BoName; }
            set
            {
                SetPropertyValue("BoName", ref _BoName, value);
            }
        }

        [PersistentAlias("concat(BoCode, '::', BoName)")]
        [Index(2), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public string BoFullName
        {
            get { return EvaluateAlias("BoFullName").ToString(); }
        }

        private bool _IsActive;
        [XafDisplayName("Active")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(10)]
        public bool IsActive
        {
            get { return _IsActive; }
            set
            {
                SetPropertyValue("IsActive", ref _IsActive, value);
            }
        }

        private Regions _Region;
        [XafDisplayName("Region")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(11)]
        public Regions Region
        {
            get { return _Region; }
            set
            {
                SetPropertyValue("Region", ref _Region, value);
            }
        }

        private Accounts _Account;
        [XafDisplayName("Account")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(11)]
        public Accounts Account
        {
            get { return _Account; }
            set
            {
                SetPropertyValue("Account", ref _Account, value);
            }
        }

        private bool _IsNote;
        [XafDisplayName("Note?")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Appearance("IsNote", Enabled = false, Criteria = "not IsNew")]
        [Index(20)]
        public bool IsNote
        {
            get { return _IsNote; }
            set
            {
                SetPropertyValue("IsNote", ref _IsNote, value);
            }
        }

        private bool _IsMileage;
        [XafDisplayName("Mileage?")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Appearance("IsMileage", Enabled = false, Criteria = "not IsNew")]
        [Index(21)]
        public bool IsMileage
        {
            get { return _IsMileage; }
            set
            {
                SetPropertyValue("IsMileage", ref _IsMileage, value);
            }
        }

        private bool _IsCombineAmount;
        [XafDisplayName("Combine Amount?")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(22)]
        public bool IsCombineAmount
        {
            get { return _IsCombineAmount; }
            set
            {
                SetPropertyValue("IsCombineAmount", ref _IsCombineAmount, value);
            }
        }
        private bool _IsLineJERemarks;
        [XafDisplayName("JE Remarks in line?")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(23)]
        public bool IsLineJERemarks
        {
            get { return _IsLineJERemarks; }
            set
            {
                SetPropertyValue("IsLineJERemarks", ref _IsLineJERemarks, value);
            }
        }

        [Browsable(false)]
        public bool IsValid
        {
            get
            {
                bool rtn = false;
                int temp = 0;

                if (IsNote)
                    temp += 1;
                if (IsMileage)
                    temp += 1;

                if (temp == 1)
                    rtn = true;

                return rtn;
            }
        }

        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }

        [Association("ClaimType-Role"), DevExpress.Xpo.Aggregated]
        public XPCollection<ClaimTypeRoles> Roles
        {
            get { return GetCollection<ClaimTypeRoles>("Roles"); }
        }
    }

    [Appearance("LinkRecord", AppearanceItemType = "Action", TargetItems = "Link", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("UnlinkRecord", AppearanceItemType = "Action", TargetItems = "Unlink", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    public class ClaimTypeRoles : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public ClaimTypeRoles(Session session)
            : base(session)
        {
        }

        [Browsable(false)]
        [Association("ClaimType-Role")]
        public ClaimTypes ClaimType
        { get; set; }

        public DevExpress.Persistent.BaseImpl.PermissionPolicy.PermissionPolicyRole FilterRole
        {
            get { return GetPropertyValue<DevExpress.Persistent.BaseImpl.PermissionPolicy.PermissionPolicyRole>("FilterRole"); }
            set { SetPropertyValue<DevExpress.Persistent.BaseImpl.PermissionPolicy.PermissionPolicyRole>("FilterRole", value); }
        }

        public string Description
        {
            get { return GetPropertyValue<string>("Description"); }
            set { SetPropertyValue<string>("Description", value); }
        }

    }

}