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
using System.Drawing;

namespace FT_EClaim.Module.BusinessObjects
{
    [DefaultClassOptions]
    //[ImageName("BO_Contact")]
    [NavigationItem("Approval")]
    [DefaultProperty("BoFullName")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    [RuleCriteria("ApprovalsDeleteRule", DefaultContexts.Delete, "1=0", "Cannot Delete.")]
    [RuleCriteria("ApprovalsSaveRule", DefaultContexts.Save, "IsValid", "Cannot Save.")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Approvals : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Approvals(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            DocType = Session.FindObject<DocTypes>(new BinaryOperator("BoCode", GeneralSettings.defclaimdoc));

            IsOverride = false;
            IsActive = false;
            IsReject = false;
            ApprovalCnt = 1;
            ApprovalSQL = "";
            DocAmount = 0;
            AppType = ApprovalTypes.Document;
            Companies company = Session.FindObject<Companies>(new BinaryOperator("BoCode", GeneralSettings.hq, BinaryOperatorType.Equal));
            AppType = company.AppType;
            ApprovalBy = company.ApprovalBy;
            BoCode = "";
            BoName = "";
        }
        protected override void OnLoaded()
        {
            base.OnLoaded();
            //DevExpress.BarCodes.BarCode barCode = new DevExpress.BarCodes.BarCode();
            //barCode.Symbology = DevExpress.BarCodes.Symbology.QRCode;
            //barCode.CodeText = "http://www.devexpress.com";
            //barCode.BackColor = Color.White;
            //barCode.ForeColor = Color.Black;
            //barCode.RotationAngle = 0;
            //barCode.CodeBinaryData = Encoding.Default.GetBytes(barCode.CodeText);
            //barCode.Options.QRCode.CompactionMode = DevExpress.BarCodes.QRCodeCompactionMode.Byte;
            //barCode.Options.QRCode.ErrorLevel = DevExpress.BarCodes.QRCodeErrorLevel.Q;
            //barCode.Options.QRCode.ShowCodeText = false;
            //barCode.DpiX = 72;
            //barCode.DpiY = 72;
            //barCode.Module = 2f;

            //Bitmap bmp = barCode.BarCodeImage;
            //ImageConverter converter = new ImageConverter();
            //PhotoData = (byte[])converter.ConvertTo(bmp, typeof(byte[]));
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

        private DocTypes _DocType;
        [XafDisplayName("Document Type"), ToolTip("Select Document")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        [Appearance("DocType", Enabled = false, Criteria = "not IsNew")]
        [Index(3)]
        public DocTypes DocType
        {
            get { return _DocType; }
            set
            {
                SetPropertyValue("DocType", ref _DocType, value);
            }
        }

        private ApprovalTypes _AppType;
        [ImmediatePostData]
        [XafDisplayName("Approval Type"), ToolTip("Select Type")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Appearance("AppType", Enabled = false, Criteria = "not IsNew")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(4)]
        public ApprovalTypes AppType
        {
            get { return _AppType; }
            set
            {
                if (SetPropertyValue("AppType", ref _AppType, value))
                {
                    if (!IsLoading)
                    {
                        SetPropertyValue("DocAmount", ref _DocAmount, 0);
                        SetPropertyValue("ApprovalSQL", ref _ApprovalSQL, "");
                    }
                }
            }
        }
        private int _ApprovalCnt;
        [XafDisplayName("Number of Approval"), ToolTip("Enter Number")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(5)]
        public int ApprovalCnt
        {
            get { return _ApprovalCnt; }
            set
            {
                SetPropertyValue("ApprovalCnt", ref _ApprovalCnt, value);
            }
        }

        private string _ApprovalLevel;
        [XafDisplayName("Approval Level"), ToolTip("Enter Number")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        [RuleUniqueValue]
        [Index(6)]
        public string ApprovalLevel
        {
            get { return _ApprovalLevel; }
            set
            {
                SetPropertyValue("ApprovalLevel", ref _ApprovalLevel, value);
            }
        }
        private decimal _DocAmount;
        [XafDisplayName("Document Amount"), ToolTip("Enter Number")]
        [Appearance("DocAmount", Enabled = false, Criteria = "not IsAllowedDocAmount")]
        [Index(7)]
        public decimal DocAmount
        {
            get { return _DocAmount; }
            set
            {
                SetPropertyValue("DocAmount", ref _DocAmount, value);
            }
        }
        [Browsable(false)]
        public bool IsAllowedDocAmount
        {
            get
            {
                if (AppType == ApprovalTypes.Document)
                    return true;
                return false;
            }
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
        private bool _IsOverride;
        [XafDisplayName("Override Lower Level?")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(20)]
        public bool IsOverride
        {
            get { return _IsOverride; }
            set
            {
                SetPropertyValue("IsOverride", ref _IsOverride, value);
            }
        }

        private bool _IsReject;
        [XafDisplayName("Reject?")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(30)]
        public bool IsReject
        {
            get { return _IsReject; }
            set
            {
                SetPropertyValue("IsReject", ref _IsReject, value);
            }
        }

        private string _ApprovalSQL;
        [XafDisplayName("Approval SQL"), ToolTip("Enter Text")]
        [Appearance("ApprovalSQL", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsSQL")]
        [Index(40)]
        [Size(4000)]
        public string ApprovalSQL
        {
            get { return _ApprovalSQL; }
            set
            {
                SetPropertyValue("ApprovalSQL", ref _ApprovalSQL, value);
            }
        }
        private ApprovalBys _ApprovalBy;
        [ImmediatePostData]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(50)]
        public ApprovalBys ApprovalBy
        {
            get { return _ApprovalBy; }
            set
            {
                if (SetPropertyValue("ApprovalBy", ref _ApprovalBy, value))
                {
                    if (!IsLoading)
                    {
                        //SetPropertyValue("Department", ref _Department, null);
                        SetPropertyValue("Division", ref _Division, null);
                    }
                }
            }
        }
        //private Departments _Department;
        //[XafDisplayName("Approval Pos. by Dept.?")]
        //[Appearance("", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "ApprovalBy = 0")]
        //[Index(51)]
        //public Departments Department
        //{
        //    get { return _Department; }
        //    set
        //    {
        //        SetPropertyValue("Department", ref _Department, value);
        //    }
        //}
        private Divisions _Division;
        [XafDisplayName("Approval Pos. by Division.?")]
        [Appearance("", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "ApprovalBy = 0")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(52)]
        public Divisions Division
        {
            get { return _Division; }
            set
            {
                SetPropertyValue("Division", ref _Division, value);
            }
        }

        //[Size(SizeAttribute.Unlimited), ImageEditor]
        //public Byte[] PhotoData { get; set; }


        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }

        [Browsable(false)]
        public bool IsBudget
        {
            get
            {
                if (AppType == ApprovalTypes.Budget)
                    return true;
                return false;
            }
        }

        [Browsable(false)]
        public bool IsSQL
        {
            get
            {
                if (AppType == ApprovalTypes.SQL)
                    return true;
                return false;
            }
        }
        [Browsable(false)]
        public bool IsUser
        {
            get
            {
                if (ApprovalBy == ApprovalBys.User)
                    return true;
                return false;
            }
        }
        [Browsable(false)]
        public bool IsPosition
        {
            get
            {
                if (ApprovalBy == ApprovalBys.Position)
                    return true;
                return false;
            }
        }

        [Browsable(false)]
        public bool IsValid
        {
            get
            {
                if (AppType == ApprovalTypes.Budget && DocAmount <= 0)
                    return true;

                if (AppType == ApprovalTypes.Document && DocAmount > 0)
                    return true;

                if (AppType == ApprovalTypes.SQL && DocAmount <= 0 && !string.IsNullOrEmpty(ApprovalSQL))
                    return true;

                return false;
            }
        }

        [Association("ApprovalTriggers")]
        [XafDisplayName("Email User")]
        [Appearance("TriggerUser", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsUser")]
        public XPCollection<SystemUsers> TriggerUser
        {
            get { return GetCollection<SystemUsers>("TriggerUser"); }
        }
        [Association("ApprovalUsers")]
        [XafDisplayName("Approve User")]
        [Appearance("ApprovalUser", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsUser")]
        public XPCollection<SystemUsers> ApprovalUser
        {
            get { return GetCollection<SystemUsers>("ApprovalUser"); }
        }
        [Association("ApprovalTriggerPositions")]
        [XafDisplayName("Email Position")]
        [Appearance("TriggerPosition", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsPosition")]
        public XPCollection<Positions> TriggerPosition
        {
            get { return GetCollection<Positions>("TriggerPosition"); }
        }
        [Association("ApprovalUserPositions")]
        [XafDisplayName("Approve Position")]
        [Appearance("ApprovalPosition", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsPosition")]
        public XPCollection<Positions> ApprovalPosition
        {
            get { return GetCollection<Positions>("ApprovalPosition"); }
        }

        [Association("ApprovalBudgetMasters")]
        [XafDisplayName("Budget Master")]
        [Appearance("BudgetMaster", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsBudget")]
        public XPCollection<BudgetMasters> BudgetMaster
        {
            get { return GetCollection<BudgetMasters>("BudgetMaster"); }
        }
    }
}