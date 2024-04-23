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
using DevExpress.Utils.Extensions;
using FT_EClaim.Module.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FT_EClaim.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class EmailSentsController : ViewController
    {
        GenControllers genCon;
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public EmailSentsController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
            TargetObjectType = typeof(EmailSents);
            TargetViewType = ViewType.ListView;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
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

        private void SendEmail_Execute(object sender, ParametrizedActionExecuteEventArgs e)
        {
            string toemail = e.ParameterCurrentValue.ToString();

            foreach (EmailSents email in ((ListView)View).SelectedObjects)
            {
                IObjectSpace ios = Application.CreateObjectSpace();
                EmailSents emailobj = ios.GetObjectByKey<EmailSents>(email.Oid);
                for (int i = emailobj.EmailSentDetail.Count - 1; i >= 0 ; i--)
                {
                    emailobj.EmailSentDetail.Remove(emailobj.EmailSentDetail[i]);
                }
                EmailSentDetails dtl = ios.CreateObject<EmailSentDetails>();
                dtl.EmailAddress = toemail;
                emailobj.EmailSentDetail.Add(dtl);

                genCon.SendEmail_By_Object(emailobj);
            }
        }
    }
}
