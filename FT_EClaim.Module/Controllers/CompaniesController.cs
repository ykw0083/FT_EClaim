using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FT_EClaim.Module.BusinessObjects;

namespace FT_EClaim.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class CompaniesController : ViewController
    {
        GenControllers genCon;
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public CompaniesController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
            TargetObjectType = typeof(Companies);
            TargetViewType = ViewType.DetailView;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            this.SAPConnection.Active.SetItemValue("Enabled", false);
            this.EmailConnection.Active.SetItemValue("Enabled", false);
            
            if (GeneralSettings.B1Post)
                this.SAPConnection.Active.SetItemValue("Enabled", true);
            if (GeneralSettings.EmailSend)
                this.EmailConnection.Active.SetItemValue("Enabled", true);
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
            genCon = Frame.GetController<GenControllers>();
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void SAPConnection_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (GeneralSettings.B1Post && genCon.ConnectSAP())
            {
                genCon.showMsg("", "Sap Connection Succssful with current user.", InformationType.Success);
            }

        }

        private void EmailConnection_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            IObjectSpace emailos = Application.CreateObjectSpace();

            EmailSents emailobj = emailos.CreateObject<EmailSents>();
            emailobj.CreateDate = (DateTime?)DateTime.Now;
            //assign body will get error???
            emailobj.EmailBody = "This is a Testing E-mail";
            emailobj.EmailSubject = "Test e-claim email";

            EmailSentDetails emaildtl = emailos.CreateObject<EmailSentDetails>();
            emaildtl.EmailUser = emaildtl.Session.GetObjectByKey<SystemUsers>((Guid)SecuritySystem.CurrentUserId);

            if (string.IsNullOrEmpty(emaildtl.EmailUser.UserEmail))
            {
                genCon.showMsg("", "Current user has no email.", InformationType.Error);
                return;
            }
            emaildtl.EmailAddress = emaildtl.EmailUser.UserEmail;
            emailobj.EmailSentDetail.Add(emaildtl);
            emailos.CommitChanges();

            if (emailobj != null)
            {
                if (genCon.SendEmail_By_Object(emailobj) != 0)
                {
                    genCon.showMsg("", "Done", InformationType.Info);
                }

            }

        }
    }
}
