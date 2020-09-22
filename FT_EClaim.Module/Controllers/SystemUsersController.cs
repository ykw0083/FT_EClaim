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
using FT_EClaim.Module.BusinessObjects;

namespace FT_EClaim.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class SystemUsersController : ViewController
    {
        GenControllers genCon;
        public SystemUsersController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
            TargetObjectType = typeof(SystemUsers);
            TargetViewType = ViewType.DetailView;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            if (View is DetailView)
            {
                ((DetailView)View).ViewEditModeChanged += SystemUsersController_ViewEditModeChanged;
            }
        }

        private void SystemUsersController_ViewEditModeChanged(object sender, EventArgs e)
        {
            this.GotoEmp.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.View);
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

        private void GotoEmp_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            SystemUsers selectebobject = (SystemUsers)View.CurrentObject;
            IObjectSpace os = Application.CreateObjectSpace();

            Employees targetobject = os.FindObject<Employees>(new BinaryOperator("SystemUser.Oid", selectebobject.Oid, BinaryOperatorType.Equal));
            if (targetobject is null)
            {
                targetobject = os.CreateObject<Employees>();
                targetobject.SystemUser = os.GetObjectByKey<SystemUsers>(selectebobject.Oid);
            }
            DetailView dv = Application.CreateDetailView(os, targetobject, true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((Employees)dv.CurrentObject).IsSystemUserCalling = true;

            e.View = dv;

        }
        private void GotoEmp_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {

        }

    }
}
