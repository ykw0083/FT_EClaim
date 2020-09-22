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
    public partial class FilterListViewControllers : ViewController
    {
        private SingleChoiceAction filteringCriterionAction;
        public FilterListViewControllers()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
            TargetViewType = ViewType.ListView;
            filteringCriterionAction = new SingleChoiceAction(
                this, "FilteringCriterion", PredefinedCategory.Filters);
            filteringCriterionAction.Execute += new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(this.FilteringCriterionAction_Execute);

        }
        private void FilteringCriterionAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            ((ListView)View).CollectionSource.BeginUpdateCriteria();
            ((ListView)View).CollectionSource.Criteria.Clear();
            ((ListView)View).CollectionSource.Criteria[e.SelectedChoiceActionItem.Caption] =
                CriteriaEditorHelper.GetCriteriaOperator(
                e.SelectedChoiceActionItem.Data as string, View.ObjectTypeInfo.Type, ObjectSpace);
            ((ListView)View).CollectionSource.EndUpdateCriteria();
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            if (View is ListView)
            {
                if (View.IsRoot)
                {
                    SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
                    bool IsClaimUser = user.Roles.Where(p => p.Name == GeneralSettings.claimrole).Count() > 0 ? true : false;
                    bool IsClaimSuperUser = user.Roles.Where(p => p.Name == GeneralSettings.claimsuperrole).Count() > 0 ? true : false;
                    bool IsAcceptanceUser = user.Roles.Where(p => p.Name == GeneralSettings.Acceptancerole).Count() > 0 ? true : false;
                    bool IsVerifyUser = user.Roles.Where(p => p.Name == GeneralSettings.verifyrole).Count() > 0 ? true : false;
                    bool IsPostUser = user.Roles.Where(p => p.Name == GeneralSettings.postrole).Count() > 0 ? true : false;
                    bool IsApprovalUser = user.Roles.Where(p => p.Name == GeneralSettings.ApprovalRole).Count() > 0 ? true : false;

                    Employees employee = ObjectSpace.FindObject<Employees>(CriteriaOperator.Parse("SystemUser.Oid=?", user.Oid));

                    #region filteringcreterion
                    bool filterrolefound = false;
                    string description1st = "";
                    string description = "";

                    filteringCriterionAction.Items.Clear();
                    foreach (FilteringCriterion criterion in ObjectSpace.GetObjects<FilteringCriterion>().OrderBy(p => p.Remarks))
                    {
                        if (criterion.ObjectType == null) continue;

                        if (criterion.ObjectType.IsAssignableFrom(View.ObjectTypeInfo.Type))
                        {
                            if (criterion.Roles.Count <= 0)
                            {
                                if (criterion.Description == "-")
                                {
                                    description = "Default";
                                    description1st = description;
                                }
                                else
                                    description = criterion.Description;

                                filteringCriterionAction.Items.Add(
                                    new ChoiceActionItem(description, criterion.Criterion));

                                if (description1st == "") description1st = description;
                            }
                            else
                            {
                                filterrolefound = false;
                                foreach (IPermissionPolicyRole role in user.Roles)
                                {
                                    if (!filterrolefound)
                                    {
                                        if (criterion.Roles.Where(p => p.FilterRole.Name == role.Name).Count() > 0)
                                        {
                                            filterrolefound = true;
                                            if (criterion.Description == "-")
                                            {
                                                description = "Default";
                                                description1st = description;
                                            }
                                            else
                                                description = criterion.Description;

                                            filteringCriterionAction.Items.Add(
                                                new ChoiceActionItem(description, criterion.Criterion));

                                            if (description1st == "") description1st = description;

                                        }
                                    }
                                }
                            }
                        }
                    }

                    filteringCriterionAction.SelectedItem = filteringCriterionAction.Items.FindItemByID(description1st);
                    if (filteringCriterionAction.SelectedItem != null)
                        filteringCriterionAction.DoExecute(filteringCriterionAction.SelectedItem);
                    #endregion
                    /*
                    if (View.Id == "ClaimTrxs_ListView")
                    {
                        if (IsClaimSuperUser || IsApprovalUser)
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("Not IsCancelled");
                        }
                        else if (IsClaimUser)
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("Not IsCancelled");
                            ((ListView)View).CollectionSource.Criteria["Filter2"] = CriteriaOperator.Parse("CreateUser.Oid=?", user.Oid);
                        }
                        else
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("1=0");
                        }
                    }
                    else if (View.Id == "ClaimTrxs_ListView_Cancelled")
                    {
                        if (IsClaimSuperUser)
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("IsCancelled");
                        }
                        else if (IsClaimUser)
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("IsCancelled");
                            ((ListView)View).CollectionSource.Criteria["Filter2"] = CriteriaOperator.Parse("CreateUser.Oid=?", user.Oid);
                        }
                        else
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("1=0");
                        }
                    }
                    else if (View.Id == "ClaimTrxs_ListView_Rejected")
                    {
                        if (IsClaimSuperUser)
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("IsRejected and not IsCancelled");
                        }
                        else if (IsClaimUser)
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("IsRejected and not IsCancelled");
                            ((ListView)View).CollectionSource.Criteria["Filter2"] = CriteriaOperator.Parse("CreateUser.Oid=?", user.Oid);
                        }
                        else
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("1=0");
                        }
                    }
                    else if (View.Id == "ClaimTrxs_ListView_Passed")
                    {
                        if (IsClaimSuperUser || IsAcceptanceUser)
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("IsPassed and not IsAccepted");
                        }
                        else if (IsClaimUser)
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("IsPassed and not IsAccepted");
                            ((ListView)View).CollectionSource.Criteria["Filter2"] = CriteriaOperator.Parse("CreateUser.Oid=?", user.Oid);
                        }
                        else
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("1=0");
                        }
                    }
                    else if (View.Id == "ClaimTrxs_ListView_Accepted")
                    {
                        if (IsClaimSuperUser || IsVerifyUser)
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("IsAccepted and not IsClosed");
                        }
                        else if (IsClaimUser)
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("IsAccepted and not IsClosed");
                            ((ListView)View).CollectionSource.Criteria["Filter2"] = CriteriaOperator.Parse("CreateUser.Oid=?", user.Oid);
                        }
                        else
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("1=0");
                        }
                    }
                    else if (View.Id == "ClaimTrxs_ListView_Closed")
                    {
                        if (IsClaimSuperUser || IsPostUser)
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("IsClosed and not IsPosted");
                        }
                        else if (IsClaimUser)
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("IsClosed and not IsPosted");
                            ((ListView)View).CollectionSource.Criteria["Filter2"] = CriteriaOperator.Parse("CreateUser.Oid=?", user.Oid);
                        }
                        else
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("1=0");
                        }
                    }
                    else if (View.Id == "ClaimTrxs_ListView_Posted")
                    {
                        if (IsClaimSuperUser || IsPostUser)
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("IsPosted");
                        }
                        else if (IsClaimUser)
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("IsPosted");
                            ((ListView)View).CollectionSource.Criteria["Filter2"] = CriteriaOperator.Parse("CreateUser.Oid=?", user.Oid);
                        }
                        else
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("1=0");
                        }
                    }
                    */
                }
                else if (View.IsRoot == false) // nested listview
                {
                    if (typeof(FilteringCriterionRole) == View.ObjectTypeInfo.Type)
                    {
                        Frame.GetController<LinkUnlinkController>().LinkAction.Active.SetItemValue("", false);
                        Frame.GetController<LinkUnlinkController>().UnlinkAction.Active.SetItemValue("", false);
                    }
                    if (View.ObjectTypeInfo.Type == typeof(SystemUsers))
                    {
                        Frame.GetController<LinkUnlinkController>().LinkAction.Active.SetItemValue("", true);
                        Frame.GetController<LinkUnlinkController>().UnlinkAction.Active.SetItemValue("", true);
                        Frame.GetController<DeleteObjectsViewController>().DeleteAction.Active.SetItemValue("", false);
                        Frame.GetController<NewObjectViewController>().NewObjectAction.Active.SetItemValue("", false);
                    }
                    if (View.ObjectTypeInfo.Type == typeof(DevExpress.Persistent.BaseImpl.PermissionPolicy.PermissionPolicyUser))
                    {
                        Frame.GetController<LinkUnlinkController>().LinkAction.Active.SetItemValue("", true);
                        Frame.GetController<LinkUnlinkController>().UnlinkAction.Active.SetItemValue("", true);
                        Frame.GetController<DeleteObjectsViewController>().DeleteAction.Active.SetItemValue("", false);
                        Frame.GetController<NewObjectViewController>().NewObjectAction.Active.SetItemValue("", false);
                    }
                    if (View.ObjectTypeInfo.Type == typeof(DevExpress.Persistent.BaseImpl.PermissionPolicy.PermissionPolicyRole))
                    {
                        Frame.GetController<LinkUnlinkController>().LinkAction.Active.SetItemValue("", true);
                        Frame.GetController<LinkUnlinkController>().UnlinkAction.Active.SetItemValue("", true);
                        Frame.GetController<DeleteObjectsViewController>().DeleteAction.Active.SetItemValue("", false);
                        Frame.GetController<NewObjectViewController>().NewObjectAction.Active.SetItemValue("", false);
                    }
                    if (View.ObjectTypeInfo.Type == typeof(ClaimTrxDetails))
                    {
                        Frame.GetController<LinkUnlinkController>().LinkAction.Active.SetItemValue("", false);
                        Frame.GetController<LinkUnlinkController>().UnlinkAction.Active.SetItemValue("", false);
                        Frame.GetController<DeleteObjectsViewController>().DeleteAction.Active.SetItemValue("", false);
                        Frame.GetController<NewObjectViewController>().NewObjectAction.Active.SetItemValue("", false);
                    }
                    if (View.ObjectTypeInfo.Type == typeof(ClaimTrxAttachments))
                    {
                        Frame.GetController<LinkUnlinkController>().LinkAction.Active.SetItemValue("", false);
                        Frame.GetController<LinkUnlinkController>().UnlinkAction.Active.SetItemValue("", false);
                        Frame.GetController<DeleteObjectsViewController>().DeleteAction.Active.SetItemValue("", true);
                        Frame.GetController<NewObjectViewController>().NewObjectAction.Active.SetItemValue("", true);
                    }
                    if (View.ObjectTypeInfo.Type == typeof(ClaimTrxDetailNotes))
                    {
                        Frame.GetController<LinkUnlinkController>().LinkAction.Active.SetItemValue("", false);
                        Frame.GetController<LinkUnlinkController>().UnlinkAction.Active.SetItemValue("", false);
                        Frame.GetController<DeleteObjectsViewController>().DeleteAction.Active.SetItemValue("", true);
                        Frame.GetController<NewObjectViewController>().NewObjectAction.Active.SetItemValue("", true);
                    }
                    if (View.ObjectTypeInfo.Type == typeof(ClaimTrxMileages))
                    {
                        Frame.GetController<LinkUnlinkController>().LinkAction.Active.SetItemValue("", false);
                        Frame.GetController<LinkUnlinkController>().UnlinkAction.Active.SetItemValue("", false);
                        Frame.GetController<DeleteObjectsViewController>().DeleteAction.Active.SetItemValue("", true);
                        Frame.GetController<NewObjectViewController>().NewObjectAction.Active.SetItemValue("", true);
                    }
                    if (View.ObjectTypeInfo.Type == typeof(ClaimTrxPostDetails))
                    {
                        Frame.GetController<LinkUnlinkController>().LinkAction.Active.SetItemValue("", false);
                        Frame.GetController<LinkUnlinkController>().UnlinkAction.Active.SetItemValue("", false);
                        Frame.GetController<DeleteObjectsViewController>().DeleteAction.Active.SetItemValue("", true);
                        Frame.GetController<NewObjectViewController>().NewObjectAction.Active.SetItemValue("", true);
                    }
                    if (View.ObjectTypeInfo.Type == typeof(ClaimTrxDocStatuses))
                    {
                        Frame.GetController<LinkUnlinkController>().LinkAction.Active.SetItemValue("", false);
                        Frame.GetController<LinkUnlinkController>().UnlinkAction.Active.SetItemValue("", false);
                        Frame.GetController<DeleteObjectsViewController>().DeleteAction.Active.SetItemValue("", false);
                        Frame.GetController<NewObjectViewController>().NewObjectAction.Active.SetItemValue("", false);
                    }
                    if (View.ObjectTypeInfo.Type == typeof(MileageDetails))
                    {
                        Frame.GetController<LinkUnlinkController>().LinkAction.Active.SetItemValue("", false);
                        Frame.GetController<LinkUnlinkController>().UnlinkAction.Active.SetItemValue("", false);
                        Frame.GetController<DeleteObjectsViewController>().DeleteAction.Active.SetItemValue("", true);
                        Frame.GetController<NewObjectViewController>().NewObjectAction.Active.SetItemValue("", true);
                    }
                    if (View.ObjectTypeInfo.Type == typeof(SystemUsers))
                    {
                        Frame.GetController<LinkUnlinkController>().LinkAction.Active.SetItemValue("", true);
                        Frame.GetController<LinkUnlinkController>().UnlinkAction.Active.SetItemValue("", true);
                        Frame.GetController<DeleteObjectsViewController>().DeleteAction.Active.SetItemValue("", false);
                        Frame.GetController<NewObjectViewController>().NewObjectAction.Active.SetItemValue("", false);
                    }
                    if (View.ObjectTypeInfo.Type == typeof(Approvals))
                    {
                        Frame.GetController<LinkUnlinkController>().LinkAction.Active.SetItemValue("", true);
                        Frame.GetController<LinkUnlinkController>().UnlinkAction.Active.SetItemValue("", true);
                        Frame.GetController<DeleteObjectsViewController>().DeleteAction.Active.SetItemValue("", false);
                        Frame.GetController<NewObjectViewController>().NewObjectAction.Active.SetItemValue("", false);
                    }
                    if (View.ObjectTypeInfo.Type == typeof(Positions))
                    {
                        Frame.GetController<LinkUnlinkController>().LinkAction.Active.SetItemValue("", true);
                        Frame.GetController<LinkUnlinkController>().UnlinkAction.Active.SetItemValue("", true);
                        Frame.GetController<DeleteObjectsViewController>().DeleteAction.Active.SetItemValue("", false);
                        Frame.GetController<NewObjectViewController>().NewObjectAction.Active.SetItemValue("", false);
                    }
                    if (View.ObjectTypeInfo.Type == typeof(Projects))
                    {
                        Frame.GetController<LinkUnlinkController>().LinkAction.Active.SetItemValue("", true);
                        Frame.GetController<LinkUnlinkController>().UnlinkAction.Active.SetItemValue("", true);
                        Frame.GetController<DeleteObjectsViewController>().DeleteAction.Active.SetItemValue("", false);
                        Frame.GetController<NewObjectViewController>().NewObjectAction.Active.SetItemValue("", false);
                    }
                    if (View.ObjectTypeInfo.Type == typeof(Departments))
                    {
                        Frame.GetController<LinkUnlinkController>().LinkAction.Active.SetItemValue("", true);
                        Frame.GetController<LinkUnlinkController>().UnlinkAction.Active.SetItemValue("", true);
                        Frame.GetController<DeleteObjectsViewController>().DeleteAction.Active.SetItemValue("", false);
                        Frame.GetController<NewObjectViewController>().NewObjectAction.Active.SetItemValue("", false);
                    }
                    if (View.ObjectTypeInfo.Type == typeof(Divisions))
                    {
                        Frame.GetController<LinkUnlinkController>().LinkAction.Active.SetItemValue("", true);
                        Frame.GetController<LinkUnlinkController>().UnlinkAction.Active.SetItemValue("", true);
                        Frame.GetController<DeleteObjectsViewController>().DeleteAction.Active.SetItemValue("", false);
                        Frame.GetController<NewObjectViewController>().NewObjectAction.Active.SetItemValue("", false);
                    }
                    if (View.ObjectTypeInfo.Type == typeof(Brands))
                    {
                        Frame.GetController<LinkUnlinkController>().LinkAction.Active.SetItemValue("", true);
                        Frame.GetController<LinkUnlinkController>().UnlinkAction.Active.SetItemValue("", true);
                        Frame.GetController<DeleteObjectsViewController>().DeleteAction.Active.SetItemValue("", false);
                        Frame.GetController<NewObjectViewController>().NewObjectAction.Active.SetItemValue("", false);
                    }
                    if (View.ObjectTypeInfo.Type == typeof(Currencies))
                    {
                        Frame.GetController<LinkUnlinkController>().LinkAction.Active.SetItemValue("", true);
                        Frame.GetController<LinkUnlinkController>().UnlinkAction.Active.SetItemValue("", true);
                        Frame.GetController<DeleteObjectsViewController>().DeleteAction.Active.SetItemValue("", false);
                        Frame.GetController<NewObjectViewController>().NewObjectAction.Active.SetItemValue("", false);
                    }
                    if (View.ObjectTypeInfo.Type == typeof(Employees))
                    {
                        Frame.GetController<LinkUnlinkController>().LinkAction.Active.SetItemValue("", true);
                        Frame.GetController<LinkUnlinkController>().UnlinkAction.Active.SetItemValue("", true);
                        Frame.GetController<DeleteObjectsViewController>().DeleteAction.Active.SetItemValue("", false);
                        Frame.GetController<NewObjectViewController>().NewObjectAction.Active.SetItemValue("", false);
                    }
                    if (View.ObjectTypeInfo.Type == typeof(BudgetMasters))
                    {
                        Frame.GetController<LinkUnlinkController>().LinkAction.Active.SetItemValue("", true);
                        Frame.GetController<LinkUnlinkController>().UnlinkAction.Active.SetItemValue("", true);
                        Frame.GetController<DeleteObjectsViewController>().DeleteAction.Active.SetItemValue("", false);
                        Frame.GetController<NewObjectViewController>().NewObjectAction.Active.SetItemValue("", false);
                    }
                    if (View.ObjectTypeInfo.Type == typeof(CompanyDocs))
                    {
                        Frame.GetController<LinkUnlinkController>().LinkAction.Active.SetItemValue("", false);
                        Frame.GetController<LinkUnlinkController>().UnlinkAction.Active.SetItemValue("", false);
                        Frame.GetController<DeleteObjectsViewController>().DeleteAction.Active.SetItemValue("", false);
                        Frame.GetController<NewObjectViewController>().NewObjectAction.Active.SetItemValue("", false);
                    }
                }
            }
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
