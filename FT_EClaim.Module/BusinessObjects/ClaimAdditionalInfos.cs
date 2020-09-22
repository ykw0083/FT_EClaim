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
    [XafDisplayName("Claim Additional Information")]
    [DefaultProperty("BoFullName")]
    //[DefaultClassOptions]
    //[ImageName("BO_Contact")]
    //[NavigationItem("Claim Setup")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[RuleCriteria("ClaimAdditionalInfosDeleteRule", DefaultContexts.Delete, "1=0", "Cannot Delete.")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class ClaimAdditionalInfos : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public ClaimAdditionalInfos(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
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
        [Index(0), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public string BoFullName
        {
            get
            {
                string rtn = "";
                string temp = "";

                if (!string.IsNullOrEmpty(TypeOfEntertainment) || !string.IsNullOrEmpty(PersonEntertainment) || Relationship != null)
                {
                    temp = "";
                    if (!string.IsNullOrEmpty(TypeOfEntertainment)) temp += "[Type]:" + TypeOfEntertainment + "";
                    if (!string.IsNullOrEmpty(PersonEntertainment)) temp += "[CO/Person]:" + PersonEntertainment + "";
                    if (Relationship != null) temp += "[Relationship]:" + Relationship.BoCode + "";
                    rtn += "<BE=" + temp + ">";
                }
                if (Employee != null || Department != null || !string.IsNullOrEmpty(Destination) || Purpose != null)
                {
                    temp = "";
                    if (Employee != null) temp += "[Emp]:" + Employee.BoName + "";
                    if (Department != null) temp += "[Dept]:" + Department.BoCode + "";
                    if (!string.IsNullOrEmpty(Destination)) temp += "[Dest]:" + Destination + "";
                    if (Purpose != null) temp += "[Purpose]:" + Purpose.BoCode + "";
                    rtn += "<TRAVEL=" + temp + ">";
                }
                if (!string.IsNullOrEmpty(Recipient) || CompanyRelationship != null || !string.IsNullOrEmpty(TypeofExpense))
                {
                    temp = "";
                    if (!string.IsNullOrEmpty(Recipient)) temp += "[Recipient]:" + Recipient + "";
                    if (CompanyRelationship != null) temp += "[Relationship]:" + CompanyRelationship.BoCode + "";
                    if (!string.IsNullOrEmpty(TypeofExpense)) temp += "[Type]:" + TypeofExpense + "";
                    rtn += "<OTH=" + temp + ">";
                }
                
                return rtn;
            }
        }
        private string _TypeOfEntertainment;
        [XafDisplayName("BE. Types Of Entertainment")]
        [Index(1)]
        public string TypeOfEntertainment
        {
            get { return _TypeOfEntertainment; }
            set
            {                
                SetPropertyValue("TypeOfEntertainment", ref _TypeOfEntertainment, value);
            }
        }

        private string _PersonEntertainment;
        [XafDisplayName("BE. CO/Person Entertainment")]
        [Index(2)]
        public string PersonEntertainment
        {
            get { return _PersonEntertainment; }
            set
            {
                SetPropertyValue("PersonEntertainment", ref _PersonEntertainment, value);
            }
        }

        private AddInfoCompanyRelationship _Relationship;
        [XafDisplayName("BE. Relationship")]
        [Index(3)]
        public AddInfoCompanyRelationship Relationship
        {
            get { return _Relationship; }
            set
            {
                SetPropertyValue("Relationship", ref _Relationship, value);
            }
        }

        private Employees _Employee;
        [XafDisplayName("Travelling's Employee")]
        [Index(4)]
        public Employees Employee
        {
            get { return _Employee; }
            set
            {
                SetPropertyValue("Relationship", ref _Employee, value);
            }
        }

        private Departments _Department;
        [XafDisplayName("Travelling's Department")]
        [Index(5)]
        public Departments Department
        {
            get { return _Department; }
            set
            {
                SetPropertyValue("Department", ref _Department, value);
            }
        }

        private string _Destination;
        [XafDisplayName("Travelling's Destination")]
        [Index(6)]
        public string Destination
        {
            get { return _Destination; }
            set
            {
                SetPropertyValue("Destination", ref _Destination, value);
            }
        }

        private AddInfoPurpose _Purpose;
        [XafDisplayName("Travelling's Purpose")]
        [Index(7)]
        public AddInfoPurpose Purpose
        {
            get { return _Purpose; }
            set
            {
                SetPropertyValue("Purpose", ref _Purpose, value);
            }
        }

        private string _Recipient;
        [XafDisplayName("Recipient Name")]
        [Index(8)]
        public string Recipient
        {
            get { return _Recipient; }
            set
            {
                SetPropertyValue("Recipient", ref _Recipient, value);
            }
        }

        private AddInfoCompanyRelationship _CompanyRelationship;
        [XafDisplayName("Relationship with Com.")]
        [Index(9)]
        public AddInfoCompanyRelationship CompanyRelationship
        {
            get { return _CompanyRelationship; }
            set
            {
                SetPropertyValue("CompanyRelationship", ref _CompanyRelationship, value);
            }
        }

        private string _TypeofExpense;
        [XafDisplayName("Type of Expenses")]
        [Index(10)]
        public string TypeofExpense
        {
            get { return _TypeofExpense; }
            set
            {
                SetPropertyValue("TypeofExpense", ref _TypeofExpense, value);
            }
        }

        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }

    }
}