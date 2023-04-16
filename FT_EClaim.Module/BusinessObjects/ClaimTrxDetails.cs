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
    [Appearance("ClaimTrxDetails1", AppearanceItemType = "Action", TargetItems = "Link", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("ClaimTrxDetails2", AppearanceItemType = "Action", TargetItems = "Unlink", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [XafDisplayName("Claim Template")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class ClaimTrxDetails : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public ClaimTrxDetails(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            CreateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
            CreateDate = DateTime.Now;
            Currency = Session.FindObject<Currencies>(new BinaryOperator("BoCode", GeneralSettings.LocalCurrency));
            //AdditionalInfo = new ClaimAdditionalInfos(Session);
            Companies company = Session.FindObject<Companies>(new BinaryOperator("Oid", CreateUser.Company.Oid, BinaryOperatorType.Equal));
            IsHideTax = company.IsHideTax;
            IsHideBrand = company.IsHideBrand;
            IsHideProject = company.IsHideProject;
        }
        protected override void OnLoaded()
        {
            base.OnLoaded();
            Companies company = Session.FindObject<Companies>(new BinaryOperator("Oid", CreateUser.Company.Oid, BinaryOperatorType.Equal));
            IsHideTax = company.IsHideTax;
            IsHideBrand = company.IsHideBrand;
            IsHideProject = company.IsHideProject;
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

        private SystemUsers _CreateUser;
        [XafDisplayName("Create User")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(300), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
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
        public DateTime? UpdateDate
        {
            get { return _UpdateDate; }
            set
            {
                SetPropertyValue("UpdateDate", ref _UpdateDate, value);
            }
        }

        [Index(1), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Category")]
        public Regions Region
        {
            get { return ClaimType == null ? null : ClaimType.Region; }
        }

        private ClaimTypes _ClaimType;
        [Index(1), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [Appearance("ClaimType", Enabled = false)]
        [RuleRequiredField(DefaultContexts.Save)]
        public ClaimTypes ClaimType
        {
            get { return _ClaimType; }
            set
            {
                SetPropertyValue("ClaimType", ref _ClaimType, value);
            }
        }

        [Index(2), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[Appearance("RefNo", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
        public Accounts Account
        {
            get { return ClaimType == null? null: ClaimType.Account; }
        }

        private Departments _Department;
        [Browsable(false)]
        [Index(3), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[Appearance("Department", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
        public Departments Department
        {
            get { return _Department; }
            set
            {
                SetPropertyValue("Department", ref _Department, value);
            }
        }


        private Projects _Project;
        [Index(4), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[Appearance("HideProject", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "IsHideProject")]
        [Appearance("HideProject2", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
        //[Appearance("Project", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
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
        [Index(5), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
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
        [Index(6), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
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
        [Index(9), VisibleInListView(false), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Date"), ToolTip("Enter Date")]
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
        //[RuleRequiredField(DefaultContexts.Save, TargetCriteria = "Amount > 0 && IsNote")]
        [Appearance("Remarks", Enabled = false, Criteria = "IsMileage")]
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
        [Index(20), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        //[Appearance("Amount2", FontColor = "White", Criteria = "(ClaimType.IsDetail or ClaimType.IsNote) and Region.BoCode='001'")]
        //[Appearance("Amount3", FontColor = "Yellow", Criteria = "(ClaimType.IsDetail or ClaimType.IsNote) and Region.BoCode='002'")]
        [Appearance("Amount", Enabled = false, Criteria = "IsMileage")]
        [Appearance("Amount2", Enabled = false, Criteria = "IsFC")]
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
        [Index(22), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        //[Appearance("Amount2", FontColor = "White", Criteria = "(ClaimType.IsDetail or ClaimType.IsNote) and Region.BoCode='001'")]
        //[Appearance("Amount3", FontColor = "Yellow", Criteria = "(ClaimType.IsDetail or ClaimType.IsNote) and Region.BoCode='002'")]
        [Appearance("TaxAmount", Enabled = false, Criteria = "IsMileage")]
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
        public decimal KMAmount
        {
            get
            {
                decimal temp = 0;
                if (ClaimDetailKM is null)
                    ClaimDetailKM = new ClaimKMs(Session);
                ClaimDetailKM.AddClaimTrxKM(ClaimTrxDetailMileage, ref temp);
                return temp;
            }
        }

        [XafDisplayName("After Tax Amount")]
        [Index(23), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("HideItemAmount", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "IsHideTax")]
        public decimal LineAmount
        {
            get { return Amount + TaxAmount; }
        }

        [XafDisplayName("Total Amount")]
        [Index(24), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        public decimal ItemAmount
        {
            get
            {
                decimal rtn = LineAmount + KMAmount;

                if (ClaimTrxDetailNote == null)
                    return rtn;
                else
                    rtn += ClaimTrxDetailNote.Sum(p => p.ItemAmount);

                return rtn;
            }
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
        [Appearance("Currency2", Enabled = false, Criteria = "IsMileage")]
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
        [Appearance("FCRate2", Enabled = false, Criteria = "IsMileage")]
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
        [Appearance("FCAmount2", Enabled = false, Criteria = "IsMileage")]
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

        private ClaimAdditionalInfos _AdditionalInfo;
        [Browsable(false)]
        [Index(60), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        public ClaimAdditionalInfos AdditionalInfo
        {
            get { return _AdditionalInfo; }
            set
            {
                SetPropertyValue("_AdditionalInfo", ref _AdditionalInfo, value);
            }
        }

        [Browsable(false)]
        public bool IsNote
        {
            get { return ClaimType == null ? false : ClaimType.IsNote; }
        }

        [Browsable(false)]
        public bool IsMileage
        {
            get { return ClaimType == null ? false : ClaimType.IsMileage; }
        }

        private ClaimTrxs _ClaimTrx;
        [Association("ClaimTrxs-ClaimTrxDetails")]
        [Index(99), VisibleInListView(false), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("ClaimTrx", Enabled = false)]
        public ClaimTrxs ClaimTrx
        {
            get { return _ClaimTrx; }
            set { SetPropertyValue("ClaimTrx", ref _ClaimTrx, value); }
        }

        [Association("ClaimTrxDetails-ClaimTrxDetailNotes"), DevExpress.Xpo.Aggregated]
        [XafDisplayName("Notes")]
        [Appearance("ClaimTrxDetailNote", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsNote")]
        public XPCollection<ClaimTrxDetailNotes> ClaimTrxDetailNote
        {
            get { return GetCollection<ClaimTrxDetailNotes>("ClaimTrxDetailNote"); }
        }

        [Appearance("ClaimTrxDetailMileage", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsMileage")]
        [Association("ClaimTrxDetails-ClaimTrxMileages"), DevExpress.Xpo.Aggregated]
        [XafDisplayName("Mileages")]
        public XPCollection<ClaimTrxMileages> ClaimTrxDetailMileage
        {
            get { return GetCollection<ClaimTrxMileages>("ClaimTrxDetailMileage"); }
        }

        [Appearance("ClaimTrxDetailKM", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsMileage")]
        //[Association("ClaimTrxDetails-ClaimTrxDetailMileages", typeof(ClaimTrxDetailKMs))]
        [XafDisplayName("Mileages Summary")]
        public ICollection<ClaimTrxKMs> ClaimTrxDetailKM
        {
            get
            {
                decimal temp = 0;
                if (ClaimDetailKM is null)
                    ClaimDetailKM = new ClaimKMs(Session);
                return ClaimDetailKM.AddClaimTrxKM(ClaimTrxDetailMileage, ref temp);
            }
        }

        [Association("ClaimTrxDetails-ClaimTrxAttachments"), DevExpress.Xpo.Aggregated]
        [XafDisplayName("Attachment")]
        [Appearance("ClaimTrxAttachment", Enabled = false, Criteria = "IsNew")]
        public XPCollection<ClaimTrxAttachments> ClaimTrxAttachment
        {
            get { return GetCollection<ClaimTrxAttachments>("ClaimTrxAttachment"); }
        }

        [NonPersistent]
        [Browsable(false)]
        public ClaimKMs ClaimDetailKM
        {
            get; set;
        }
        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
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