using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace FT_EClaim.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class SwitchViewControllers : ViewController
    {
        GenControllers genCon;
        public SwitchViewControllers()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            this.SwitchView.Active.SetItemValue("Enabled", false);
            if (View.GetType() == typeof(DetailView))
            {
                this.SwitchView.Active.SetItemValue("Enabled", true);
                this.SwitchView.Active.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.Edit);
                ((DetailView)View).ViewEditModeChanged += SwitchViewControllers_ViewEditModeChanged;
            }
        }

        private void SwitchViewControllers_ViewEditModeChanged(object sender, EventArgs e)
        {
            if (View.GetType() == typeof(DetailView))
            {
                this.SwitchView.Active.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.Edit);
            }
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
            genCon = Frame.GetController<GenControllers>();
        }
        protected override void OnDeactivated()
        {
            if (View.GetType() == typeof(DetailView))
            {
                ((DetailView)View).ViewEditModeChanged -= SwitchViewControllers_ViewEditModeChanged;
            }
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void SwitchView_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (ObjectSpace.ModifiedObjects.Count == 0)
            {
                ((DetailView)View).ViewEditMode = ViewEditMode.View;
                View.BreakLinksToControls();
                View.CreateControls();
            }
            else
            {
                genCon.showMsg("Error", "Please save the document 1st.", InformationType.Info);
            }
        }
    }
}
