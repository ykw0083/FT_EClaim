﻿using System;
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
    [NavigationItem("Claim Setup")]
    //[ImageName("BO_Contact")]
    [DefaultProperty("BoFullName")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    [RuleCriteria("MileagesDeleteRule", DefaultContexts.Delete, "1=0", "Cannot Delete.")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Mileages : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Mileages(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            IsActive = true;
            ByLine = false;
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
        [Appearance("BoCode", Enabled = false, Criteria = "not IsNew")]
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

        private Accounts _Account;
        [XafDisplayName("Account")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)

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
        private bool _ByLine;
        [XafDisplayName("By Line")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(20)]
        public bool ByLine
        {
            get { return _ByLine; }
            set
            {
                SetPropertyValue("ByLine", ref _ByLine, value);
            }
        }
        [Association("Mileages-MileageDetails", typeof(MileageDetails))]
        //[Appearance("CompanyDoc", Enabled = false)]
        public XPCollection<MileageDetails> MileageDetail
        {
            get { return GetCollection<MileageDetails>("MileageDetail"); }
        }
        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }

    }
}