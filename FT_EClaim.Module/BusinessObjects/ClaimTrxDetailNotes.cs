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
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    [Appearance("ClaimTrxDetailNotes1", AppearanceItemType = "Action", TargetItems = "Link", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("ClaimTrxDetailNotes2", AppearanceItemType = "Action", TargetItems = "Unlink", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("ClaimTrxDetailNotesFontColor", TargetItems = "*", FontColor = "Red", Criteria = "IsDuplicated")]
    [XafDisplayName("Claim Notes")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class ClaimTrxDetailNotes : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public ClaimTrxDetailNotes(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            //Oid = Convert.ToInt32(Session.Evaluate<ClaimTrxDetails>(DevExpress.Data.Filtering.CriteriaOperator.Parse("Max(Oid)"), null)) + 1;
            CreateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
            CreateDate = DateTime.Now;

            Tax = Session.FindObject<Taxes>(new BinaryOperator("BoCode", GeneralSettings.defaulttax, BinaryOperatorType.Equal));
            if (ClaimTrxDetail != null)
                if (ClaimTrxDetail.Tax != null)
                    Tax = Session.FindObject<Taxes>(new BinaryOperator("Oid", ClaimTrxDetail.Tax.Oid, BinaryOperatorType.Equal));
            Currency = Session.FindObject<Currencies>(new BinaryOperator("BoCode", GeneralSettings.LocalCurrency));
            Companies company = Session.FindObject<Companies>(new BinaryOperator("Oid", CreateUser.Company.Oid, BinaryOperatorType.Equal));
            IsHideTax = company.IsHideTax;
            IsHideBrand = company.IsHideBrand;
            IsHideProject = company.IsHideProject;
            IsDuplicated = false;
        }
        protected override void OnLoaded()
        {
            base.OnLoaded();
            Companies company = Session.FindObject<Companies>(new BinaryOperator("Oid", CreateUser.Company.Oid, BinaryOperatorType.Equal));
            IsHideTax = company.IsHideTax;
            IsHideBrand = company.IsHideBrand;
            IsHideProject = company.IsHideProject;
            IsDuplicated = false;
        }
        [Browsable(false)]
        [NonPersistent]
        public bool IsHideTax { get; set; }

        [Browsable(false)]
        [NonPersistent]
        public bool IsHideBrand { get; set; }

        [Browsable(false)]
        [NonPersistent]
        public bool IsHideProject { get; set; }
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
        [NonPersistent]
        [Index(999)]
        [Appearance("IsDuplicated", Enabled = false)]
        public bool IsDuplicated { get; set; }

        private SystemUsers _CreateUser;
        [XafDisplayName("Create User")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(300), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [NonCloneable]
        public SystemUsers CreateUser
        {
            get { return _CreateUser; }
            set
            {
                SetPropertyValue("CreateUser", ref _CreateUser, value);
            }
        }

        private DateTime? _CreateDate;
        [Index(301), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [NonCloneable]
        public DateTime? CreateDate
        {
            get { return _CreateDate; }
            set
            {
                SetPropertyValue("CreateDate", ref _CreateDate, value);
            }
        }

        private SystemUsers _UpdateUser;
        [XafDisplayName("Update User"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(302), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [NonCloneable]
        public SystemUsers UpdateUser
        {
            get { return _UpdateUser; }
            set
            {
                SetPropertyValue("UpdateUser", ref _UpdateUser, value);
            }
        }

        private DateTime? _UpdateDate;
        [Index(303), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [NonCloneable]
        public DateTime? UpdateDate
        {
            get { return _UpdateDate; }
            set
            {
                SetPropertyValue("UpdateDate", ref _UpdateDate, value);
            }
        }

        private Departments _Department;
        [Browsable(false)]
        [Index(2), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[Appearance("RefNo", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
        public Departments Department
        {
            get { return _Department; }
            set
            {
                SetPropertyValue("Department", ref _Department, value);
            }
        }


        private Projects _Project;
        [Index(3), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[Appearance("HideProject", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "IsHideProject")]
        [Appearance("HideProject2", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
        //[Appearance("RefNo", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
        //[RuleRequiredField(DefaultContexts.Save)]
        public Projects Project
        {
            get { return _Project; }
            set
            {
                SetPropertyValue("Project", ref _Project, value);
            }
        }
        private Divisions _Division;
        [Browsable(false)]
        [Index(4), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[Appearance("RefNo", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
        public Divisions Division
        {
            get { return _Division; }
            set
            {
                SetPropertyValue("Division", ref _Division, value);
            }
        }
        private Brands _Brand;
        [Index(5), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [Appearance("HideBrand", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "IsHideBrand")]
        //[Appearance("RefNo", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
        public Brands Brand
        {
            get { return _Brand; }
            set
            {
                SetPropertyValue("Brand", ref _Brand, value);
            }
        }

        private DateTime? _DocDate;
        [Index(9), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Date"), ToolTip("Enter Date")]
        [Appearance("HideDocDate", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
        public DateTime? DocDate
        {
            get { return _DocDate; }
            set
            {
                SetPropertyValue("DocDate", ref _DocDate, value);

            }
        }

        private string _RefNo;
        [Index(10), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[Appearance("RefNo", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
        public string RefNo
        {
            get { return _RefNo; }
            set
            {
                SetPropertyValue("RefNo", ref _RefNo, value);
            }
        }

        private string _Remarks;
        [Index(11), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[Appearance("RefNo", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
        //[RuleRequiredField(DefaultContexts.Save, TargetCriteria = "Amount > 0")]
        public string Remarks
        {
            get { return _Remarks; }
            set
            {
                SetPropertyValue("Remarks", ref _Remarks, value);
            }
        }

        [Browsable(false)]
        public bool IsAmountValid
        {
            get
            {
                if (Math.Round(TaxAmount, 2) != 0)
                    if (Math.Round(Amount, 2) == 0)
                        return false;

                return true;
            }
        }
        private decimal _Amount;
        [ImmediatePostData]
        [Index(20), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        //[Appearance("Amount2", FontColor = "White", Criteria = "(ClaimType.IsDetail or ClaimType.IsNote) and Region.BoCode='001'")]
        //[Appearance("Amount3", FontColor = "Yellow", Criteria = "(ClaimType.IsDetail or ClaimType.IsNote) and Region.BoCode='002'")]
        [Appearance("Amount", Enabled = false, Criteria = "IsFC")]
        [XafDisplayName("Before Tax Amount")]
        public decimal Amount
        {
            get { return _Amount; }
            set
            {
                SetPropertyValue("Amount", ref _Amount, Math.Round(value, 2));
            }
        }

        private Taxes _Tax;
        [Index(21), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[Appearance("RefNo", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
        [RuleRequiredField(DefaultContexts.Save, TargetCriteria = "Amount > 0")]
        [Appearance("HideTax", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "IsHideTax")]
        public Taxes Tax
        {
            get { return _Tax; }
            set
            {
                SetPropertyValue("Tax", ref _Tax, value);
            }
        }

        private decimal _TaxAmount;
        [ImmediatePostData]
        [Index(22), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        //[Appearance("Amount2", FontColor = "White", Criteria = "(ClaimType.IsDetail or ClaimType.IsNote) and Region.BoCode='001'")]
        //[Appearance("Amount3", FontColor = "Yellow", Criteria = "(ClaimType.IsDetail or ClaimType.IsNote) and Region.BoCode='002'")]
        [Appearance("HideTaxAmount", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "IsHideTax")]
        [XafDisplayName("Tax Amount")]
        public decimal TaxAmount
        {
            get { return _TaxAmount; }
            set
            {
                SetPropertyValue("TaxAmount", ref _TaxAmount, Math.Round(value, 2));
            }
        }

        [XafDisplayName("After Tax Amount")]
        [Index(23), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("HideItemAmount", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "IsHideTax")]
        public decimal ItemAmount
        {
            get { return Amount + TaxAmount; }
        }
        [Browsable(false)]
        public bool IsFC
        {
            get
            {
                bool rtn = false;
                if (_Currency != null)
                    if (_Currency.BoCode != GeneralSettings.LocalCurrency)
                        rtn = true;
                return rtn;
            }
        }
        private Currencies _Currency;
        [ImmediatePostData]
        [Index(50), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        [XafDisplayName("Currency")]
        public Currencies Currency
        {
            get { return _Currency; }
            set
            {
                if (SetPropertyValue("Currency", ref _Currency, value))
                {
                    if (!IsLoading)
                    {
                        _FCRate = 0;
                        _FCAmount = 0;
                    }
                }
            }
        }
        private double _FCRate;
        [Browsable(false)]
        [ImmediatePostData]
        [Index(51), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("FCRate", Enabled = false, Criteria = "not IsFC")]
        [XafDisplayName("FC Rate")]
        public double FCRate
        {
            get { return _FCRate; }
            set
            {
                if (SetPropertyValue("FCRate", ref _FCRate, Math.Round(value, 6)))
                {
                    if (!IsLoading)
                    {
                        if (IsFC)
                        {
                            _Amount = Math.Round(Convert.ToDecimal(_FCRate) * FCAmount, 2);
                        }
                    }
                }
            }
        }
        private decimal _FCAmount;
        [Browsable(false)]
        [ImmediatePostData]
        [Index(52), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("FCAmount", Enabled = false, Criteria = "not IsFC")]
        [XafDisplayName("FC Amount")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        public decimal FCAmount
        {
            get { return _FCAmount; }
            set
            {
                if (SetPropertyValue("FCAmount", ref _FCAmount, Math.Round(value, 2)))
                {
                    if (!IsLoading)
                    {
                        if (IsFC)
                        {
                            _Amount = Math.Round(Convert.ToDecimal(FCRate) * _FCAmount, 2);
                        }
                    }
                }
            }
        }

        private DateTime? _DateFrom;
        [XafDisplayName("Date From")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(53)]
        public DateTime? DateFrom
        {
            get { return _DateFrom; }
            set
            {
                SetPropertyValue("DateFrom", ref _DateFrom, value);
            }
        }
        private DateTime? _DateTo;
        [XafDisplayName("Date To")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(53)]
        public DateTime? DateTo
        {
            get { return _DateTo; }
            set
            {
                SetPropertyValue("DateTo", ref _DateTo, value);
            }
        }

        private ClaimAdditionalInfos _AdditionalInfo;
        [Browsable(false)]
        [Index(60), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        [NonCloneable]
        public ClaimAdditionalInfos AdditionalInfo
        {
            get { return _AdditionalInfo; }
            set
            {
                SetPropertyValue("_AdditionalInfo", ref _AdditionalInfo, value);
            }
        }

        private ClaimTrxDetails _ClaimTrxDetail;
        [Association("ClaimTrxDetails-ClaimTrxDetailNotes")]
        [Index(99), VisibleInListView(false), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("ClaimTrxDetail", Enabled = false)]
        public ClaimTrxDetails ClaimTrxDetail
        {
            get { return _ClaimTrxDetail; }
            set { SetPropertyValue("ClaimTrxDetail", ref _ClaimTrxDetail, value); }
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            if (!(Session is NestedUnitOfWork)
                && (Session.DataLayer != null)
                    && (Session.ObjectLayer is SimpleObjectLayer)
                        )
            {
                UpdateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
                UpdateDate = DateTime.Now;

                if (Session.IsNewObject(this))
                {

                }
            }
        }

    }
}