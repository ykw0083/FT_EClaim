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
using DevExpress.ExpressApp.StateMachine;
using DevExpress.ExpressApp.Xpo;

namespace FT_EClaim.Module.BusinessObjects
{
    [DefaultClassOptions]
    [XafDisplayName("Claim")]
    [NavigationItem("Claims")]
    //[Appearance("NewRecord", AppearanceItemType = "Action", TargetItems = "New", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("EditRecord", AppearanceItemType = "Action", TargetItems = "SwitchToEditMode;Edit", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "IsPosted")]
    [Appearance("DeleteRecord", AppearanceItemType = "Action", TargetItems = "Delete", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("LinkRecord", AppearanceItemType = "Action", TargetItems = "Link", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("UnlinkRecord", AppearanceItemType = "Action", TargetItems = "Unlink", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [DefaultProperty("DocNum")]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    [RuleCriteria("ClaimTrxsDeleteRule", DefaultContexts.Delete, "1=0", "Cannot Delete.")]
    public class ClaimTrxs : XPObject//, IStateMachineProvider
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public ClaimTrxs(Session session)
            : base(session)
        {
        }
        public string getRemarks()
        {
            return "";
            DateTime claimdate = DocDate;
            claimdate = claimdate.AddMonths(-1);
            if (Employee != null)
            {
                string name = Employee.BoName;
                return name + " EXP FOR " + claimdate.ToString("MMM").ToUpper() + " " + claimdate.ToString("yy");
            }
            else
                return "EXP FOR " + claimdate.ToString("MMM").ToUpper() + " " + claimdate.ToString("yy");
        
        }
        public string getJERemarks()
        {

            DateTime claimdate = DocDate;
            claimdate = claimdate.AddMonths(-1);
            if (Employee != null)
            {
                string name = Employee.BoName;
                if (name.Length > 35)
                    name = name.Substring(0, 35);
                return name + " EXP FOR " + claimdate.ToString("MMM").ToUpper() + " " + claimdate.ToString("yy");
            }
            else
                return "EXP FOR " + claimdate.ToString("MMM").ToUpper() + " " + claimdate.ToString("yy");

        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            CreateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
            CreateDate = DateTime.Now;
            DocDate = DateTime.Today;
            //ApprovalStatus = ApprovalStatuses.Not_Applicable;

            Company = Session.FindObject<Companies>(new BinaryOperator("Oid", CreateUser.Company.Oid, BinaryOperatorType.Equal));
            IsHideBrand = Company.IsHideBrand;
            IsHideProject = Company.IsHideProject;

            DocType = Session.FindObject<DocTypes>(new BinaryOperator("BoCode", GeneralSettings.defclaimdoc));
            Employee = Session.FindObject<Employees>(new BinaryOperator("SystemUser.Oid", SecuritySystem.CurrentUserId));
            if (Employee != null)
            {
                if (Employee.Department != null)
                    Department = Session.GetObjectByKey<Departments>(Employee.Department.Oid);
                if (Employee.Division != null)
                    Division = Session.GetObjectByKey<Divisions>(Employee.Division.Oid);
                if (Employee.Brand != null)
                    Brand = Session.GetObjectByKey<Brands>(Employee.Brand.Oid);
                if (Employee.Position != null)
                    Position = Session.GetObjectByKey<Positions>(Employee.Position.Oid);
            }
            Remarks = getRemarks();


            Currency = Session.FindObject<Currencies>(new BinaryOperator("BoCode", GeneralSettings.LocalCurrency));

            IsClaimSuperCheck = false;
            IsClaimUserCheck = true;
            IsAcceptUserCheck = false;
            IsVerifyUserCheck = false;
            if (CreateUser != null)
            {
                if (CreateUser.Roles.Where(p => p.Name == GeneralSettings.claimsuperrole).Count() > 0)
                {
                    this.IsClaimSuperCheck = true;
                    this.IsClaimUserCheck = true;
                }
                //else if (this.CreateUser != null)
                //    if (CreateUser.Roles.Where(p => p.Name == GeneralSettings.claimrole).Count() > 0 && CreateUser.Oid == this.CreateUser.Oid)
                //        this.IsClaimUserCheck = true;

                if (CreateUser.Roles.Where(p => p.Name == GeneralSettings.Acceptancerole).Count() > 0)
                    this.IsAcceptUserCheck = true;

                if (CreateUser.Roles.Where(p => p.Name == GeneralSettings.verifyrole).Count() > 0)
                    this.IsVerifyUserCheck = true;

            }

            DevExpress.Xpo.Metadata.XPClassInfo myClass = Session.GetClassInfo(typeof(ClaimTypes));
            CriteriaOperator myCriteria = new BinaryOperator("IsActive", true);
            SortingCollection sortProps = new SortingCollection(null);
            sortProps.Add(new SortProperty("BoCode", DevExpress.Xpo.DB.SortingDirection.Ascending));

            var LDocType = Session.GetObjects(myClass, myCriteria, sortProps, 0, false, true);

            //XPCollection<DocTypes> LDocType = new XPCollection<DocTypes>();
            //LDocType.Load();

            int cnt = 0;
            foreach (var dtl in LDocType)
            {
                ClaimTypes mytype = Session.GetObjectByKey<ClaimTypes>(((ClaimTypes)dtl).Oid);
                //var mytype = ((ClaimTypes)dtl).Roles;
                if (mytype.Roles.Where(pp => pp.FilterRole.Users.Contains(CreateUser)).FirstOrDefault() == null) continue;
                cnt++;
                ClaimTrxDetails obj = new ClaimTrxDetails(Session);
                obj.Oid = cnt * -1;
                obj.ClaimType = Session.FindObject<ClaimTypes>(new BinaryOperator("Oid", ((ClaimTypes)dtl).Oid, BinaryOperatorType.Equal));
                if (obj.ClaimType.IsMileage)
                {
                    obj.Tax = Session.FindObject<Taxes>(new BinaryOperator("BoCode", GeneralSettings.defaultmileagetax, BinaryOperatorType.Equal));
                }
                else
                {
                    obj.Tax = Session.FindObject<Taxes>(new BinaryOperator("BoCode", GeneralSettings.defaulttax, BinaryOperatorType.Equal));
                }
                this.ClaimTrxDetail.Add(obj);
            }

        }
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



        private DocTypes _DocType;
        [XafDisplayName("Doc Type"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Appearance("DocType", Enabled = false)]
        [Index(304), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public virtual DocTypes DocType
        {
            get { return _DocType; }
            set
            {
                SetPropertyValue("DocType", ref _DocType, value);
            }
        }

        private Companies _Company;
        [XafDisplayName("Company"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(305), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public virtual Companies Company
        {
            get { return _Company; }
            set
            {
                SetPropertyValue("Company", ref _Company, value);
            }
        }

        private long _DocNumSeq;
        [Browsable(false)]
        public long DocNumSeq
        {
            get { return _DocNumSeq; }
            set
            {
                SetPropertyValue("DocNumSeq", ref _DocNumSeq, value);
            }
        }


        private string _DocNum;
        [Index(0), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Claim No")]
        [Appearance("DocNum", Enabled = false)]
        public string DocNum
        {
            get { return _DocNum; }
            set
            {
                SetPropertyValue("DocNum", ref _DocNum, value);
            }
        }


        private DateTime _DocDate;
        [ImmediatePostData]
        [Index(1), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [RuleRequiredField(DefaultContexts.Save)]
        //[ModelDefault("DisplayFormat", "{0:D}")] //LongDate
        //[ModelDefault("EditMask", "D")]
        //[ModelDefault("DisplayFormat", "{0:d}")] //ShortDate
        //[ModelDefault("EditMask", "d")]
        //[ModelDefault("DisplayFormat", "{0:dd/MM/yyyy}")]
        //[ModelDefault("EditMask", "dd/MM/yyyy")]
        //[ModelDefault("DisplayFormat", "{0:f}")] //LongDate+ShortTime
        //[ModelDefault("EditMask", "f")]
        //[ModelDefault("DisplayFormat", "{0: ddd, dd MMMM yyyy hh:mm:ss tt}")]
        //[ModelDefault("EditMask", "ddd, dd MMMM yyyy hh:mm:ss tt")]
        [XafDisplayName("Claim Date"), ToolTip("Enter Text")]
        [Appearance("DocDate", Enabled = false, Criteria = "IsProtectContect")]
        public DateTime DocDate
        {
            get { return _DocDate; }
            set
            {
                if (SetPropertyValue("DocDate", ref _DocDate, value))
                {
                    if (_DocDate != null)
                    {
                        _Remarks = getRemarks();
                    }
                }
            }
        }

        private Employees _Employee;
        [ImmediatePostData]
        [XafDisplayName("Employee"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(2), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [Appearance("Employee", Enabled = false, Criteria = "IsProtectContect")]
        [Appearance("Employee2", Enabled = false, Criteria = "not IsClaimSuperCheck")]
        [RuleRequiredField(DefaultContexts.Save)]
        public Employees Employee
        {
            get { return _Employee; }
            set
            {
                if (SetPropertyValue("Employee", ref _Employee, value))
                {
                    if (_Employee != null)
                    {
                        if (Session.IsNewObject(this))
                        {
                            if (_Employee.Department != null)
                            {
                                _Department = Session.GetObjectByKey<Departments>(_Employee.Department.Oid);
                            }
                            if (_Employee.Division != null)
                            {
                                _Division = Session.GetObjectByKey<Divisions>(_Employee.Division.Oid);
                            }
                            if (_Employee.Brand != null)
                            {
                                _Brand = Session.GetObjectByKey<Brands>(_Employee.Brand.Oid);
                            }
                            if (_Employee.Position != null)
                            {
                                _Position = Session.GetObjectByKey<Positions>(_Employee.Position.Oid);
                            }
                        }
                        Remarks = getRemarks();
                    }
                }
            }
        }

        private Positions _Position;
        [XafDisplayName("Position"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(3), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Appearance("Position", Enabled = false, Criteria = "IsProtectContect")]
        [Appearance("Position2", Enabled = false, Criteria = "not IsClaimSuperCheck")]
        public Positions Position
        {
            get { return _Position; }
            set
            {
                SetPropertyValue("Position", ref _Position, value);
            }
        }

        private Divisions _Division;
        [XafDisplayName("Division"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(4), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Appearance("Division", Enabled = false, Criteria = "IsProtectContect")]
        [Appearance("Division2", Enabled = false, Criteria = "not IsClaimSuperCheck")]
        public Divisions Division
        {
            get { return _Division; }
            set
            {
                SetPropertyValue("Division", ref _Division, value);
            }
        }

        private Brands _Brand;
        [XafDisplayName("Brand"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(5), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Appearance("Brand", Enabled = false, Criteria = "IsProtectContect")]
        [Appearance("HideBrand", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "IsHideBrand")]
        public Brands Brand
        {
            get { return _Brand; }
            set
            {
                SetPropertyValue("Brand", ref _Brand, value);
            }
        }

        private Departments _Department;
        [XafDisplayName("Department"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(10), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Appearance("Department", Enabled = false, Criteria = "IsProtectContect")]
        [Appearance("Department2", Enabled = false, Criteria = "not IsClaimSuperCheck")]
        public Departments Department
        {
            get { return _Department; }
            set
            {
                SetPropertyValue("Department", ref _Department, value);
            }
        }

        private Projects _Project;
        [XafDisplayName("Project"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(11), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [Appearance("Project", Enabled = false, Criteria = "IsProtectContect")]
        [Appearance("HideProject", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "IsHideProject")]
        public Projects Project
        {
            get { return _Project; }
            set
            {
                SetPropertyValue("Project", ref _Project, value);
            }
        }

        private string _RefNo;
        [Index(15), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[Appearance("RefNo", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
        [Appearance("RefNo", Enabled = false, Criteria = "IsProtectContect")]
        public string RefNo
        {
            get { return _RefNo; }
            set
            {
                SetPropertyValue("RefNo", ref _RefNo, value);
            }
        }

        private string _Remarks;
        [Index(16), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[Appearance("RefNo", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Appearance("Remarks", Enabled = false, Criteria = "IsProtectContect")]
        public string Remarks
        {
            get { return _Remarks; }
            set
            {
                SetPropertyValue("Remarks", ref _Remarks, value);
            }
        }

        [NonPersistent]
        [Index(20), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        public decimal Amount
        {
            get
            {
                decimal rtn = 0;
                if (ClaimTrxDetail != null)
                    rtn += ClaimTrxDetail.Sum(p => p.ItemAmount);

                if (ClaimTrxItem != null)
                    rtn += ClaimTrxItem.Sum(p => p.ItemAmount);

                return rtn;
            }
        }

        [NonPersistent]
        [Index(21), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public decimal KMAmount
        {
            get
            {
                decimal temp = 0;
                if (ClaimKM is null)
                    ClaimKM = new ClaimKMs(Session);
                ClaimKM.AddClaimTrxKM(ClaimTrxMileage, ref temp);
                return temp;
            }
        }

        [NonPersistent]
        [Index(22), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        public decimal TotalAmount
        {
            get
            {
                decimal rtn = KMAmount;
                if (ClaimTrxDetail != null)
                    rtn += ClaimTrxDetail.Sum(p => p.ItemAmount);

                if (ClaimTrxItem != null)
                    rtn += ClaimTrxItem.Sum(p => p.ItemAmount);

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
        [Index(30), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        [XafDisplayName("Advance Currency")]
        [Appearance("Currency", Enabled = false, Criteria = "IsProtectContect")]
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
                        if (IsNew)
                        {
                            foreach (ClaimTrxDetails dtl in ClaimTrxDetail)
                            {
                                if (Currency != null)
                                    dtl.Currency = Session.GetObjectByKey<Currencies>(Currency.Oid);
                                dtl.FCRate = FCRate;
                            }
                        }
                    }
                }
            }
        }
        private double _FCRate;
        [ImmediatePostData]
        [Index(31), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("FCRate", Enabled = false, Criteria = "not IsFC")]
        [Appearance("FCRate2", Enabled = false, Criteria = "IsProtectContect")]
        [XafDisplayName("Advance FC Rate")]
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
                            _AdvanceAmount = Math.Round(Convert.ToDecimal(_FCRate) * FCAmount, 2);
                            if (IsNew)
                            {
                                foreach (ClaimTrxDetails dtl in ClaimTrxDetail)
                                {
                                    if (Currency != null)
                                        dtl.Currency = Session.GetObjectByKey<Currencies>(Currency.Oid);
                                    dtl.FCRate = FCRate;
                                }
                            }
                        }
                    }
                }
            }
        }
        private decimal _FCAmount;
        [ImmediatePostData]
        [Index(32), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("FCAmount", Enabled = false, Criteria = "not IsFC")]
        [Appearance("FCAmount2", Enabled = false, Criteria = "IsProtectContect")]
        [XafDisplayName("Advance FC Amount")]
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
                            _AdvanceAmount = Math.Round(Convert.ToDecimal(FCRate) * _FCAmount, 2);
                        }
                    }
                }
            }
        }
        private decimal _AdvanceAmount;
        [ImmediatePostData]
        [Index(33), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [Appearance("AdvanceAmount", Enabled = false, Criteria = "IsProtectContect")]
        [Appearance("AdvanceAmount2", Enabled = false, Criteria = "IsFC")]
        public decimal AdvanceAmount
        {
            get { return _AdvanceAmount; }
            set
            {
                SetPropertyValue("AdvanceAmount", ref _AdvanceAmount, Math.Round(value, 2));
            }
        }

        [Index(34), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public decimal Balance
        {
            get
            {
                return TotalAmount - AdvanceAmount;
            }
        }
        private ApprovalStatuses _ApprovalStatus;
        [Index(50), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[Appearance("RefNo", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
        [Appearance("ApprovalStatus", Enabled = false)]
        public ApprovalStatuses ApprovalStatus
        {
            get { return _ApprovalStatus; }
            set
            {
                SetPropertyValue("ApprovalStatus", ref _ApprovalStatus, value);
            }
        }
        public IList<IStateMachine> GetStateMachines()
        {
            List<IStateMachine> result = new List<IStateMachine>();
            result.Add(new ClaimTrxsStateMachine(XPObjectSpace.FindObjectSpaceByObject(this)));
            return result;
        }
        private bool _IsCancelled;
        [ImmediatePostData]
        [XafDisplayName("Cancelled")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(100), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("IsCancelled", Enabled = false)]
        public bool IsCancelled
        {
            get { return _IsCancelled; }
            set
            {
                SetPropertyValue("IsCancelled", ref _IsCancelled, value);
            }
        }
        private bool _IsPassed;
        [ImmediatePostData]
        [XafDisplayName("Passed")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(101), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("IsPassed", Enabled = false)]
        public bool IsPassed
        {
            get { return _IsPassed; }
            set
            {
                SetPropertyValue("IsPassed", ref _IsPassed, value);
            }
        }
        private bool _IsAccepted;
        [ImmediatePostData]
        [XafDisplayName("Accepted")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(102), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("IsAccepted", Enabled = false)]
        public bool IsAccepted
        {
            get { return _IsAccepted; }
            set
            {
                SetPropertyValue("IsAccepted", ref _IsAccepted, value);
            }
        }
        private bool _IsRejected;
        [ImmediatePostData]
        [XafDisplayName("Rejected")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(103), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("IsRejected", Enabled = false)]
        public bool IsRejected
        {
            get { return _IsRejected; }
            set
            {
                SetPropertyValue("IsRejected", ref _IsRejected, value);
            }
        }
        private bool _IsClosed;
        [ImmediatePostData]
        [XafDisplayName("Closed")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(104), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("IsClosed", Enabled = false)]
        public bool IsClosed
        {
            get { return _IsClosed; }
            set
            {
                SetPropertyValue("IsClosed", ref _IsClosed, value);
            }
        }
        private bool _IsPosted;
        [ImmediatePostData]
        [XafDisplayName("Posted")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(105), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("IsPosted", Enabled = false)]
        public bool IsPosted
        {
            get { return _IsPosted; }
            set
            {
                SetPropertyValue("IsPosted", ref _IsPosted, value);
            }
        }
        private string _SAPPostCancelRemarks;
        [Index(107), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("SAPPostCancelRemarks", Enabled = false)]
        public string SAPPostCancelRemarks
        {
            get { return _SAPPostCancelRemarks; }
            set
            {
                SetPropertyValue("SAPPostCancelRemarks", ref _SAPPostCancelRemarks, value);
            }
        }
        private bool _IsSAPPosted;
        [Browsable(false)]
        [Appearance("IsSAPPosted", Enabled = false)]
        public bool IsSAPPosted
        {
            get { return _IsSAPPosted; }
            set
            {
                SetPropertyValue("IsSAPPosted", ref _IsSAPPosted, value);
            }
        }
        private int _SAPKey;
        [Browsable(false)]
        [Appearance("SAPKey", Enabled = false)]
        public int SAPKey
        {
            get { return _SAPKey; }
            set
            {
                SetPropertyValue("SAPKey", ref _SAPKey, value);
            }
        }
        private DateTime? _PaidDate;
        [Index(109), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Post Date"), ToolTip("Enter Text")]
        [Appearance("PaidDate", Enabled = false)]
        public DateTime? PaidDate
        {
            get { return _PaidDate; }
            set
            {
                SetPropertyValue("PaidDate", ref _PaidDate, value);
            }
        }

        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }

        [Index(200), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public string ClaimStatus
        {
            get
            {
                if (IsCancelled)
                    return "Cancelled";
                else if (IsPosted)
                    return "Posted";
                else if (IsClosed)
                    return "Closed";
                else if (IsRejected)
                    return "Rejected";
                else if (IsAccepted)
                    return "Accepted";
                else if (IsPassed)
                    return "Passed";

                return "New";
            }
        }

        //[Browsable(false)]
        //public int? DocYear
        //{
        //    get
        //    {
        //        if (DocDate <= DateTime.MinValue) return null;
        //        return DocDate == null ? null : (int?)DocDate.Year;
        //    }
        //}
        //[Browsable(false)]
        //public int? DocMonth
        //{
        //    get
        //    {
        //        if (DocDate <= DateTime.MinValue) return null;
        //        return DocDate == null ? null : (int?)DocDate.Month;
        //    }
        //}
        //[Browsable(false)]
        //public int? DocDay
        //{
        //    get
        //    {
        //        if (DocDate <= DateTime.MinValue) return null;
        //        return DocDate == null ? null : (int?)DocDate.Day;
        //    }
        //}
        //[PersistentAlias("Concat(Iif(Employee is null, '', Employee.BoName), ' ', IsNull(DocMonth,''), '/', IsNull(DocYear,''))")]
        //[Index(201), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //public string JEMemo
        //{
        //    get
        //    {
        //        return (string)EvaluateAlias("JEMemo");
        //    }
        //}
        [Browsable(false)]
        public bool IsProtectContect
        {
            get 
            {
                if (IsPassed)
                    return true;
                if (IsClaimUserCheck || IsClaimSuperCheck)
                    return false;
                return true; 
            }
        }
        [Browsable(false), NonPersistent]
        public bool IsClaimUserCheck { get; set; }
        [Browsable(false), NonPersistent]
        public bool IsClaimSuperCheck { get; set; }
        [Browsable(false), NonPersistent]
        public bool IsAcceptUserCheck { get; set; }
        [Browsable(false), NonPersistent]
        public bool IsVerifyUserCheck { get; set; }
        [Browsable(false), NonPersistent]
        public bool IsApprovalUserCheck { get; set; }

        [Association("ClaimTrxs-ClaimTrxDetails"), DevExpress.Xpo.Aggregated]
        [XafDisplayName("Claims")]
        [Appearance("ClaimTrxDetail", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not Company.IsDetail")]
        [Appearance("ClaimTrxDetail2", Enabled = false, Criteria = "IsProtectContect")]
        [Appearance("ClaimTrxDetail3", Enabled = false, Criteria = "IsNew")]
        public XPCollection<ClaimTrxDetails> ClaimTrxDetail
        {
            get { return GetCollection<ClaimTrxDetails>("ClaimTrxDetail"); }
        }

        [Association("ClaimTrxs-ClaimTrxItems"), DevExpress.Xpo.Aggregated]
        [XafDisplayName("Detail Item")]
        [Appearance("ClaimTrxItem", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not Company.IsItem")]
        [Appearance("ClaimTrxItem2", Enabled = false, Criteria = "IsProtectContect")]
        [Appearance("ClaimTrxItem3", Enabled = false, Criteria = "IsNew")]
        public XPCollection<ClaimTrxItems> ClaimTrxItem
        {
            get { return GetCollection<ClaimTrxItems>("ClaimTrxItem"); }
        }

        [Association("ClaimTrxs-ClaimTrxMileages"), DevExpress.Xpo.Aggregated]
        [XafDisplayName("Mileages")]
        [Appearance("ClaimTrxMileage", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not Company.IsMileage")]
        [Appearance("ClaimTrxMileage2", Enabled = false, Criteria = "IsProtectContect")]
        [Appearance("ClaimTrxMileage3", Enabled = false, Criteria = "IsNew")]
        public XPCollection<ClaimTrxMileages> ClaimTrxMileage
        {
            get { return GetCollection<ClaimTrxMileages>("ClaimTrxMileage"); }
        }

        [Appearance("ClaimTrxKM", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not Company.IsMileage")]
        //[Association("ClaimTrxDetails-ClaimTrxDetailMileages", typeof(ClaimTrxDetailKMs))]
        [XafDisplayName("Mileages Summary")]
        public ICollection<ClaimTrxKMs> ClaimTrxKM
        {
            get
            {
                decimal temp = 0;
                if (ClaimKM is null)
                    ClaimKM = new ClaimKMs(Session);
                return ClaimKM.AddClaimTrxKM(ClaimTrxMileage, ref temp);
            }
        }

        private ClaimTrxAppStages _CurrentAppStage;
        [Appearance("CurrentAppStage", Enabled = false)]
        public ClaimTrxAppStages CurrentAppStage
        {
            get { return _CurrentAppStage; }
            set
            {
                SetPropertyValue("CurrentAppStage", ref _CurrentAppStage, value);
            }
        }

        //private Approvals _CurrentApproval;
        ////[NonPersistent]
        //[Appearance("CurrentApproval", Enabled = false)]
        //public Approvals CurrentApproval
        //{
        //    get { return _CurrentApproval; }
        //    set
        //    {
        //        SetPropertyValue("CurrentApproval", ref _CurrentApproval, value);
        //    }
        //}

        [NonPersistent]
        [Browsable(false)]
        public ClaimKMs ClaimKM
        {
            get; set;
        }
        //[Association("ClaimTrxs-ClaimTrxPhotos", typeof(ClaimTrxPhotos))]
        //[XafDisplayName("Photo")]
        //public XPCollection<ClaimTrxPhotos> ClaimTrxPhoto
        //{
        //    get { return GetCollection<ClaimTrxPhotos>("ClaimTrxPhoto"); }
        //}

        [Association("ClaimTrxs-ClaimTrxAttachments"), DevExpress.Xpo.Aggregated]
        [XafDisplayName("Attachment")]
        [Appearance("ClaimTrxAttachment", Enabled = false, Criteria = "IsNew")]
        [Appearance("ClaimTrxAttachment2", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
        public XPCollection<ClaimTrxAttachments> ClaimTrxAttachment
        {
            get { return GetCollection<ClaimTrxAttachments>("ClaimTrxAttachment"); }
        }

        [Association("ClaimTrxs-ClaimTrxPostDetails"), DevExpress.Xpo.Aggregated]
        [XafDisplayName("Post Detail")]
        [Appearance("ClaimTrxPostDetail", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsPassed")]
        [Appearance("ClaimTrxPostDetail2", Enabled = false, Criteria = "IsClosed")]
        [Appearance("ClaimTrxPostDetail3", Enabled = false, Criteria = "not IsVerifyUserCheck")]
        public XPCollection<ClaimTrxPostDetails> ClaimTrxPostDetail
        {
            get { return GetCollection<ClaimTrxPostDetails>("ClaimTrxPostDetail"); }
        }
        [Association("ClaimTrxs-ClaimTrxDocStatuses"), DevExpress.Xpo.Aggregated]
        [XafDisplayName("Status")]
        public XPCollection<ClaimTrxDocStatuses> ClaimTrxDocStatus
        {
            get { return GetCollection<ClaimTrxDocStatuses>("ClaimTrxDocStatus"); }
        }
        [Association("ClaimTrxs-ClaimTrxAppStatuses"), DevExpress.Xpo.Aggregated]
        [XafDisplayName("Approval Status")]
        public XPCollection<ClaimTrxAppStatuses> ClaimTrxAppStatus
        {
            get { return GetCollection<ClaimTrxAppStatuses>("ClaimTrxAppStatus"); }
        }
        [Association("ClaimTrxs-ClaimTrxAppStages"), DevExpress.Xpo.Aggregated]
        [XafDisplayName("Approval Stage")]
        public XPCollection<ClaimTrxAppStages> ClaimTrxAppStage
        {
            get { return GetCollection<ClaimTrxAppStages>("ClaimTrxAppStage"); }
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
                    //assignDocNum();
                    ////DocNumSeq = com.n
                    ClaimTrxDocStatuses ds = new ClaimTrxDocStatuses(Session);
                    ds.DocStatus = DocumentStatus.Create;
                    ds.DocRemarks = "";
                    ds.CreateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
                    ds.CreateDate = DateTime.Now;
                    ds.UpdateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
                    ds.UpdateDate = DateTime.Now;
                    this.ClaimTrxDocStatus.Add(ds);

                }
            }
        }
        
        public void assignDocNum()
        {
            if (DocNumSeq > 0) return;

            CompanyDocs doc = Session.FindObject<CompanyDocs>(CriteriaOperator.Parse("Company.Oid=? and DocType.Oid=?", Company.Oid, DocType.Oid));
            DocNumSeq = doc.NextDocNo;
            DocNum = DocNumSeq.ToString();
            doc.NextDocNo++;

        }
        protected override void OnSaved()
        {
            base.OnSaved();
            this.Reload();
        }

        public int GetCurrentApprovalStageOid()
        {
            int rtn = 0;
            int appcount = 0;
            if (ClaimTrxAppStage != null && ClaimTrxAppStage.Count > 0)
            {
                foreach (ClaimTrxAppStages dtl in ClaimTrxAppStage.OrderBy(p => p.Oid))
                {
                    if (ClaimTrxAppStatus != null && ClaimTrxAppStatus.Count > 0)
                    {
                        appcount = 0;
                        foreach (ClaimTrxAppStatuses status in ClaimTrxAppStatus)
                        {
                            if (status.ClaimTrxAppStage != null)
                            {
                                if (status.ClaimTrxAppStage.Oid == dtl.Oid)
                                    appcount++;
                            }
                        }
                        if (dtl.Approval.ApprovalCnt > appcount)
                        {
                            rtn = dtl.Oid;
                            break;
                        }
                    }
                    else
                    {
                        if (dtl.Approval.ApprovalCnt > 0)
                        {
                            rtn = dtl.Oid;
                            break;
                        }
                    }
                }
            }

            return rtn;
        }
        protected override void OnLoaded()
        {
            base.OnLoaded();
            //int appcount = 0;
            //if (ClaimTrxAppStage != null && ClaimTrxAppStage.Count > 0)
            //{
            //    foreach (ClaimTrxAppStages dtl in ClaimTrxAppStage.OrderBy(p => p.Oid))
            //    {
            //        if (ClaimTrxAppStatus != null && ClaimTrxAppStatus.Count > 0)
            //        {
            //            appcount = 0;
            //            //appcount = ClaimTrxAppStatus.Where(p => p.ClaimTrxAppStage.Oid == dtl.Oid).Count();
            //            foreach (ClaimTrxAppStatuses status in ClaimTrxAppStatus)
            //            {
            //                if (status.ClaimTrxAppStage != null)
            //                {
            //                    status.ClaimTrxAppStage.Oid = dtl.Oid;
            //                    appcount++;
            //                }
            //            }
            //            if (dtl.Approval.ApprovalCnt > appcount)
            //            {
            //                CurrentAppStage = dtl;
            //                break;
            //            }
            //        }
            //        else
            //        {
            //            if (dtl.Approval.ApprovalCnt > 0)
            //            {
            //                CurrentAppStage = dtl;
            //                break;
            //            }
            //        }
            //    }
            //}
            IsHideBrand = Company.IsHideBrand;
            IsHideProject = Company.IsHideProject;

            IsClaimSuperCheck = false;
            IsClaimUserCheck = false;
            IsAcceptUserCheck = false;
            IsVerifyUserCheck = false;
            IsApprovalUserCheck = false;

            SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
            if (user != null)
            {
                if (user.Roles.Where(p => p.Name == GeneralSettings.claimsuperrole).Count() > 0)
                {
                    this.IsClaimSuperCheck = true;
                    this.IsClaimUserCheck = true;
                }
                else if (this.CreateUser != null)
                    if (user.Roles.Where(p => p.Name == GeneralSettings.claimrole).Count() > 0 && user.Oid == this.CreateUser.Oid)
                        this.IsClaimUserCheck = true;

                if (user.Roles.Where(p => p.Name == GeneralSettings.Acceptancerole).Count() > 0)
                    this.IsAcceptUserCheck = true;

                if (user.Roles.Where(p => p.Name == GeneralSettings.verifyrole).Count() > 0)
                    this.IsVerifyUserCheck = true;

                if (CurrentAppStage != null)
                {
                    Employees emp = Session.FindObject<Employees>(new BinaryOperator("SystemUser.Oid", user.Oid));
                    bool found = false;
                    if (user.Roles.Where(p => p.Name == GeneralSettings.ApprovalRole).Count() > 0)
                    {

                        if (Company.IsConcurrentAppStage) // IsConcurrentAppStage always false
                        {
                            if (CurrentAppStage.Approval.ApprovalBy == ApprovalBys.User)
                            {
                                foreach (ClaimTrxAppStages dtl in this.ClaimTrxAppStage)
                                {
                                    if (dtl.Approval.ApprovalUser.Where(p => p.Oid == user.Oid).Count() > 0)
                                        found = true;
                                }
                            }
                            else if (CurrentAppStage.Approval.ApprovalBy == ApprovalBys.Position)
                            {
                                if (emp != null)
                                {
                                    foreach (ClaimTrxAppStages dtl in this.ClaimTrxAppStage)
                                    {
                                        if (dtl.Approval.ApprovalPosition.Where(p => p.Oid == emp.Position.Oid).Count() > 0)
                                            found = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (CurrentAppStage.Approval.ApprovalBy == ApprovalBys.User)
                            {
                                if (CurrentAppStage.Approval.ApprovalUser.Where(p => p.Oid == user.Oid).Count() > 0)
                                    found = true;
                            }
                            else if (CurrentAppStage.Approval.ApprovalBy == ApprovalBys.Position)
                            {
                                if (emp != null && emp.Position != null)
                                {
                                    if (CurrentAppStage.Approval.ApprovalPosition.Where(p => p.Oid == emp.Position.Oid).Count() > 0)
                                    {
                                        if (CurrentAppStage.Approval.Division != null)
                                        {
                                            if (emp.Division != null && CurrentAppStage.Approval.Division.Oid == emp.Division.Oid)
                                                found = true;
                                        }
                                        else
                                            found = true;
                                    }
                                }
                            }
                        }
                    }
                    this.IsApprovalUserCheck = found;
                }

            }

        }
    }
}