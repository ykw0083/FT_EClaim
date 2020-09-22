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
    public partial class BudgetControllers : ViewController
    {
        GenControllers genCon;
        public BudgetControllers()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
            TargetObjectType = typeof(Budgets);
            TargetViewNesting = Nesting.Nested;
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

        private void AssignBudget_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            bool err = true;
            ListView lv = ((ListView)View);
            if (lv.CollectionSource is PropertyCollectionSource)
            {
                PropertyCollectionSource collectionSource = (PropertyCollectionSource)lv.CollectionSource;
                if (collectionSource.MasterObject != null)
                {
                    if (!ObjectSpace.IsNewObject(collectionSource.MasterObject))
                    {
                        err = false;
                    }
                }
            }
            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<BudgetParameters>(), true);
            ((BudgetParameters)dv.CurrentObject).IsErr = err;
            ((BudgetParameters)dv.CurrentObject).ActionMessage = "Plesae save the record before Assign Budget.";
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;

            e.View = dv;


        }

        private void AssignBudget_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            BudgetParameters p = (BudgetParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;
            if (p.Amount <= 0) return;
            if (p.ParamYear >= 2010 && p.ParamYear <= 2099)
            { }
            else
                return;

            Budgets currentobject = (Budgets)View.CurrentObject;
            ListView lv = ((ListView)View);
            if (lv.CollectionSource is PropertyCollectionSource)
            {
                PropertyCollectionSource collectionSource = (PropertyCollectionSource)lv.CollectionSource;
                if (collectionSource.MasterObject != null)
                {

                    if (collectionSource.MasterObjectType == typeof(BudgetMasters))
                    {
                        BudgetMasters masterobject = (BudgetMasters)collectionSource.MasterObject;
                        if (p.IsYearly)
                        {
                            Budgets budget = ObjectSpace.CreateObject<Budgets>();
                            budget.DateFrom = new DateTime(p.ParamYear, 1, 1);
                            budget.DateTo = new DateTime(p.ParamYear, 12, 31);
                            budget.Amount = p.Amount;
                            masterobject.Budget.Add(budget);
                        }
                        else
                        {
                            for (int x = 1; x <= 12; x++)
                            {
                                Budgets budget = ObjectSpace.CreateObject<Budgets>();
                                budget.DateFrom = new DateTime(p.ParamYear, x, 1);
                                budget.DateTo = budget.DateFrom.AddMonths(1).AddDays(-1);
                                budget.Amount = p.Amount;
                                masterobject.Budget.Add(budget);
                            }
                        }
                    }






                }
            }
        }

    }
}
