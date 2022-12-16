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
    [NavigationItem("Setup")]
    [XafDisplayName("Company")]
    [DefaultProperty("BoFullName")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    [Appearance("NewRecord", AppearanceItemType = "Action", TargetItems = "New", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    //[Appearance("EditRecord", AppearanceItemType = "Action", TargetItems = "Edit", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("DeleteRecord", AppearanceItemType = "Action", TargetItems = "Delete", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("LinkRecord", AppearanceItemType = "Action", TargetItems = "Link", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("UnlinkRecord", AppearanceItemType = "Action", TargetItems = "Unlink", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [RuleCriteria("CompaniesDeleteRule", DefaultContexts.Delete, "1=0", "Cannot Delete.")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Companies : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Companies(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).

            IsHideTax = false;
            IsPassAccept = false;
            IsRejectWOApp = true;
            IsDetail = true;
            IsItem = true;
            IsMileage = true;
            PostToDocument = PostToDocuments.JE;
            ApprovalBy = ApprovalBys.User;
            IsConcurrentAppStage = false; // IsConcurrentAppStage always false
            BoCode = "";
            BoName = "";

            DevExpress.Xpo.Metadata.XPClassInfo myClass = Session.GetClassInfo(typeof(DocTypes));
            CriteriaOperator myCriteria = new BinaryOperator("IsActive", true);
            SortingCollection sortProps = new SortingCollection(null);
            sortProps.Add(new SortProperty("BoCode", DevExpress.Xpo.DB.SortingDirection.Ascending));

            var LDocType = Session.GetObjects(myClass, myCriteria, sortProps, 0, false, true);

            //XPCollection<DocTypes> LDocType = new XPCollection<DocTypes>();
            //LDocType.Load();

            int cnt = 0;
            foreach (var dtl in LDocType)
            {
                cnt++;
                CompanyDocs obj = new CompanyDocs(Session);
                obj.DocType = Session.FindObject<DocTypes>(new BinaryOperator("Oid", ((DocTypes)dtl).Oid, BinaryOperatorType.Equal));
                obj.NextDocNo = (cnt * 1000000) + 1;
                this.CompanyDoc.Add(obj);
            }



        }

        protected override void OnSaving()
        {
            base.OnSaving();
            if (!(Session is NestedUnitOfWork)
                && (Session.DataLayer != null)
                    && (Session.ObjectLayer is SimpleObjectLayer)
                        //OR
                        //&& !(Session.ObjectLayer is DevExpress.ExpressApp.Security.ClientServer.SecuredSessionObjectLayer)
                        && Session.IsNewObject(this))
            {
                //DateTime DocTime = DateTime.Now;
                //OutletDocs outletdoc = Session.FindObject<OutletDocs>(CriteriaOperator.Parse("Outlet.Oid=? and DocType=?", Outlet.Oid, DocumentType.SalesOrder));
                //SequentialNumber = outletdoc.NextSeq;
                //Prefix = outletdoc.Prefix;

                //outletdoc.NextSeq++;

                //outletdoc.Save();

            }
        }

        protected override void OnSaved()
        {
            base.OnSaved();
            this.Reload();
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

        private EClaimSAPDocs _EClaimSAPDoc;
        [XafDisplayName("E-Claim Sap Document")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(10)]
        public EClaimSAPDocs EClaimSAPDoc
        {
            get { return _EClaimSAPDoc; }
            set
            {
                SetPropertyValue("EClaimSAPDoc", ref _EClaimSAPDoc, value);
            }
        }

        private bool _IsDetail;
        [XafDisplayName("Template?")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(20)]
        public bool IsDetail
        {
            get { return _IsDetail; }
            set
            {
                SetPropertyValue("IsDetail", ref _IsDetail, value);
            }
        }
        private bool _IsItem;
        [XafDisplayName("Detail Item?")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(21)]
        public bool IsItem
        {
            get { return _IsItem; }
            set
            {
                SetPropertyValue("IsItem", ref _IsItem, value);
            }
        }
        private bool _IsMileage;
        [XafDisplayName("Mileage?")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(22)]
        public bool IsMileage
        {
            get { return _IsMileage; }
            set
            {
                SetPropertyValue("IsMileage", ref _IsMileage, value);
            }
        }
        private bool _IsPassAccept;
        [XafDisplayName("Pass = Accept")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(30)]
        public bool IsPassAccept
        {
            get { return _IsPassAccept; }
            set
            {
                SetPropertyValue("IsPassAccept", ref _IsPassAccept, value);
            }
        }
        private bool _IsRejectWOApp;
        [ImmediatePostData]
        [XafDisplayName("Reject w/o Approval?")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(31)]
        public bool IsRejectWOApp
        {
            get { return _IsRejectWOApp; }
            set
            {
                SetPropertyValue("IsRejectWOApp", ref _IsRejectWOApp, value);
            }
        }
        private string _RejectWOAppRemarks;
        [XafDisplayName("Reject w/o Approval Remarks")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Appearance("RejectWOAppRemarks", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsRejectWOApp")]
        [Index(32)]
        public string RejectWOAppRemarks
        {
            get { return _RejectWOAppRemarks; }
            set
            {
                SetPropertyValue("RejectWOAppRemarks", ref _RejectWOAppRemarks, value);
            }
        }
        private bool _IsSimpleApproval;
        [ImmediatePostData]
        [XafDisplayName("Simple Approval?")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(33)]
        public bool IsSimpleApproval
        {
            get { return _IsSimpleApproval; }
            set
            {
                SetPropertyValue("IsSimpleApproval", ref _IsSimpleApproval, value);
            }
        }

        private Accounts _ClaimControlAccount;
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(40)]
        public Accounts ClaimControlAccount
        {
            get { return _ClaimControlAccount; }
            set
            {
                SetPropertyValue("ClaimControlAccount", ref _ClaimControlAccount, value);
            }
        }
        private Accounts _AdvanceControlAccount;
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(41)]
        public Accounts AdvanceControlAccount
        {
            get { return _AdvanceControlAccount; }
            set
            {
                SetPropertyValue("selectedObject", ref _AdvanceControlAccount, value);
            }
        }
        private PostToDocuments _PostToDocument;
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(50)]
        public PostToDocuments PostToDocument
        {
            get { return _PostToDocument; }
            set
            {
                SetPropertyValue("PostToDocument", ref _PostToDocument, value);
            }
        }
        private int _ClaimSeriesNo;
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(51)]
        public int ClaimSeriesNo
        {
            get { return _ClaimSeriesNo; }
            set
            {
                SetPropertyValue("ClaimSeriesNo", ref _ClaimSeriesNo, value);
            }
        }

        private ApprovalBys _ApprovalBy;
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [XafDisplayName("Default Approval By")]
        [Index(60)]
        public ApprovalBys ApprovalBy
        {
            get { return _ApprovalBy; }
            set
            {
                SetPropertyValue("ApprovalBy", ref _ApprovalBy, value);
            }
        }

        private ApprovalTypes _AppType;
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [XafDisplayName("Default Approval Type")]
        [Index(70)]
        public ApprovalTypes AppType
        {
            get { return _AppType; }
            set
            {
                SetPropertyValue("AppType", ref _AppType, value);
            }
        }

        private CreditLimitBys _CreditLimitBy;
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [XafDisplayName("Default Credit Limit By")]
        [Index(71)]
        public CreditLimitBys CreditLimitBy
        {
            get { return _CreditLimitBy; }
            set
            {
                SetPropertyValue("CreditLimitBy", ref _CreditLimitBy, value);
            }
        }
        private CreditLimitTypes _CreditLimitType;
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [XafDisplayName("Default Credit Limit Type")]
        [Index(72)]
        public CreditLimitTypes CreditLimitType
        {
            get { return _CreditLimitType; }
            set
            {
                SetPropertyValue("CreditLimitType", ref _CreditLimitType, value);
            }
        }

        private bool _IsHideTax;
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [XafDisplayName("Hide Tax Columns")]
        [Index(73)]
        public bool IsHideTax
        {
            get { return _IsHideTax; }
            set
            {
                SetPropertyValue("IsHideTax", ref _IsHideTax, value);
            }
        }

        private bool _IsHideBrand;
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [XafDisplayName("Hide Brand Columns")]
        [Index(74)]
        public bool IsHideBrand
        {
            get { return _IsHideBrand; }
            set
            {
                SetPropertyValue("IsHideBrand", ref _IsHideBrand, value);
            }
        }
        private bool _IsHideProject;
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [XafDisplayName("Hide Project Columns")]
        [Index(75)]
        public bool IsHideProject
        {
            get { return _IsHideProject; }
            set
            {
                SetPropertyValue("IsHideProject", ref _IsHideProject, value);
            }
        }
        private string _ClaimLink;
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [XafDisplayName("Claim Doc Link")]
        [Index(76)]
        public string ClaimLink
        {
            get { return _ClaimLink; }
            set
            {
                SetPropertyValue("ClaimLink", ref _ClaimLink, value);
            }
        }

        private bool _IsConcurrentAppStage;
        [Index(80)]
        [Browsable(false)]
        public bool IsConcurrentAppStage
        {
            get { return _IsConcurrentAppStage; }
            set
            {
                SetPropertyValue("IsConcurrentAppStage", ref _IsConcurrentAppStage, value);
            }
        }
        [Association("Companies-CompanyDocs"), DevExpress.Xpo.Aggregated]
        public XPCollection<CompanyDocs> CompanyDoc
        {
            get { return GetCollection<CompanyDocs>("CompanyDoc"); }
        }

        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }


    }


}