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
    [RuleCriteria("CreditLimitsDeleteRule", DefaultContexts.Delete, "1=0", "Cannot Delete.")]
    [RuleCriteria("CreditLimitsSaveRule", DefaultContexts.Save, "IsValid", "Cannot Save.")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class CreditLimits : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public CreditLimits(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            DocType = Session.FindObject<DocTypes>(new BinaryOperator("BoCode", GeneralSettings.defclaimdoc));

            IsReject = true;
            DocAmount = 0;
            Companies company = Session.FindObject<Companies>(new BinaryOperator("BoCode", GeneralSettings.hq, BinaryOperatorType.Equal));
            CreditLimitBy = company.CreditLimitBy;
            CreditLimitType = company.CreditLimitType;
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

        private CreditLimitTypes _CreditLimitType;
        [XafDisplayName("Credit Limit Type")]
        [Index(7)]
        public CreditLimitTypes CreditLimitType
        {
            get { return _CreditLimitType; }
            set
            {
                SetPropertyValue("CreditLimitType", ref _CreditLimitType, value);
            }
        }

        private decimal _DocAmount;
        [XafDisplayName("Document Amount"), ToolTip("Enter Number")]
        [Index(8)]
        public decimal DocAmount
        {
            get { return _DocAmount; }
            set
            {
                SetPropertyValue("DocAmount", ref _DocAmount, value);
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

        private bool _IsReject;
        [XafDisplayName("Reject?")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Appearance("IsReject", Enabled = false)]
        [Index(30)]
        public bool IsReject
        {
            get { return _IsReject; }
            set
            {
                SetPropertyValue("IsReject", ref _IsReject, value);
            }
        }

        private CreditLimitBys _CreditLimitBy;
        [ImmediatePostData]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(50)]
        public CreditLimitBys CreditLimitBy
        {
            get { return _CreditLimitBy; }
            set
            {
                if (SetPropertyValue("CreditLimitBy", ref _CreditLimitBy, value))
                {
                    if (!IsLoading)
                    {
                        //SetPropertyValue("Department", ref _Department, null);
                        //SetPropertyValue("Division", ref _Division, null);
                    }
                }
            }
        }
        private Accounts _Account;
        [XafDisplayName("Account ?")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(20)]
        public Accounts Account
        {
            get { return _Account; }
            set
            {
                SetPropertyValue("Account", ref _Account, value);
            }
        }

        private Departments _Department;
        [XafDisplayName("Department ?")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(21)]
        public Departments Department
        {
            get { return _Department; }
            set
            {
                SetPropertyValue("Department", ref _Department, value);
            }
        }
        private Divisions _Division;
        [XafDisplayName("Division ?")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(22)]
        public Divisions Division
        {
            get { return _Division; }
            set
            {
                SetPropertyValue("Division", ref _Division, value);
            }
        }
        private Projects _Project;
        [XafDisplayName("Project ?")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(23)]
        public Projects Project
        {
            get { return _Project; }
            set
            {
                SetPropertyValue("Project", ref _Project, value);
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
        public bool IsEmployee
        {
            get
            {
                if (CreditLimitBy == CreditLimitBys.Employee)
                    return true;
                return false;
            }
        }
        [Browsable(false)]
        public bool IsPosition
        {
            get
            {
                if (CreditLimitBy == CreditLimitBys.Position)
                    return true;
                return false;
            }
        }

        [Browsable(false)]
        public bool IsValid
        {
            get
            {
                if (DocAmount > 0) return true;
                return false;
            }
        }

        [Association("CreditLimitEmployees")]
        [XafDisplayName("Credit Limit Employee")]
        [Appearance("CreditLimitEmployee", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsEmployee")]
        public XPCollection<Employees> Employee
        {
            get { return GetCollection<Employees>("Employee"); }
        }
        [Association("CreditLimitUserPositions")]
        [XafDisplayName("Credit Limit Position")]
        [Appearance("ApprovalPosition", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsPosition")]
        public XPCollection<Positions> Position
        {
            get { return GetCollection<Positions>("Position"); }
        }

    }
}