using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using FT_EClaim.Module.BusinessObjects;
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;

namespace FT_EClaim.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class ClaimTrxControllers : ViewController
    {
        GenControllers genCon;
        RecordsNavigationController recordnaviator;
        private SystemUsers user = null;
        private bool IsClaimUser = false;
        private bool IsClaimSuperUser = false;
        private bool IsAcceptanceUser = false;
        private bool IsVerifyUser = false;
        private bool IsPostUser = false;
        private bool IsApprovalUser = false;
        private bool IsApprovalListUser = false;
        private bool IsRejectApproveRole = false;
        private bool IsSimpleApproval = false;
        public ClaimTrxControllers()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
            TargetObjectType = typeof(ClaimTrxs);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            user = (SystemUsers)SecuritySystem.CurrentUser;
            IsClaimUser = user.Roles.Where(p => p.Name == GeneralSettings.claimrole).Count() > 0 ? true : false;
            IsClaimSuperUser = user.Roles.Where(p => p.Name == GeneralSettings.claimsuperrole).Count() > 0 ? true : false;
            IsAcceptanceUser = user.Roles.Where(p => p.Name == GeneralSettings.Acceptancerole).Count() > 0 ? true : false;
            IsVerifyUser = user.Roles.Where(p => p.Name == GeneralSettings.verifyrole).Count() > 0 ? true : false;
            IsPostUser = user.Roles.Where(p => p.Name == GeneralSettings.postrole).Count() > 0 ? true : false;
            IsApprovalUser = user.Roles.Where(p => p.Name == GeneralSettings.ApprovalRole).Count() > 0 ? true : false;
            IsApprovalListUser = user.Roles.Where(p => p.Name == GeneralSettings.ApprovalListRole).Count() > 0 ? true : false;
            IsRejectApproveRole = user.Roles.Where(p => p.Name == GeneralSettings.RejectApproveRole).Count() > 0 ? true : false;
            IsSimpleApproval = user.Company.IsSimpleApproval;

            resetButton();
            if (View is DetailView)
            {
                #region check companydoc exists before assignDocNum
                ClaimTrxs selectedObject = (ClaimTrxs)View.CurrentObject;
                if (!selectedObject.checkCompanyDoc())
                {
                    disableButton();
                    this.View.BreakLinksToControls();
                    throw new Exception ("Company Document Series not found.");
                }
                #endregion

                ((DetailView)View).ViewEditModeChanged += ClaimTrxControllers_ViewEditModeChanged;

                recordnaviator = Frame.GetController<RecordsNavigationController>();
                if (recordnaviator != null)
                {
                    recordnaviator.PreviousObjectAction.Executed += PreviousObjectAction_Executed;
                    recordnaviator.NextObjectAction.Executed += NextObjectAction_Executed;
                }
            }
            NewObjectViewController controller = Frame.GetController<NewObjectViewController>();
            if (controller != null)
            {
                //controller.NewObjectAction.Execute += NewObjectAction_Execute;
                controller.ObjectCreated += Controller_ObjectCreated;
            }
        }

        private void Controller_ObjectCreated(object sender, ObjectCreatedEventArgs e)
        {
            genCon.showMsg("Successful", "Please save the New Claim before proceed to details.", InformationType.Success);
        }

        private void PreviousObjectAction_Executed(object sender, ActionBaseEventArgs e)
        {
            if (View.GetType() == typeof(DetailView))
            {
                resetButton();
            }
        }
        private void NextObjectAction_Executed(object sender, ActionBaseEventArgs e)
        {
            if (View.GetType() == typeof(DetailView))
            {
                resetButton();
            }
        }
        public void publicCallResetButton()
        {
            resetButton();
        }
        private void disableButton()
        {
            this.CancelDoc.Active.SetItemValue("Enabled", false);
            this.PassDoc.Active.SetItemValue("Enabled", false);
            this.RejectDoc.Active.SetItemValue("Enabled", false);
            this.AcceptDoc.Active.SetItemValue("Enabled", false);
            this.CloseDoc.Active.SetItemValue("Enabled", false);
            this.CloseSelected.Active.SetItemValue("Enabled", false);
            this.ReopenDoc.Active.SetItemValue("Enabled", false);
            this.DuplicateDoc.Active.SetItemValue("Enabled", false);
            this.SetPaidDate.Active.SetItemValue("Enabled", false);
            this.Approval.Active.SetItemValue("Enabled", false);
            this.ApproveSelected.Active.SetItemValue("Enabled", false);
            this.ApprovalApprove.Active.SetItemValue("Enabled", false);
            this.ApprovalReject.Active.SetItemValue("Enabled", false);

        }
        private void resetButton()
        {
            ClaimTrxs selectedObject = (ClaimTrxs)View.CurrentObject;

            disableButton();

            if (View is ListView)
            {
                if (IsApprovalListUser)
                    this.ApproveSelected.Active.SetItemValue("Enabled", true);
                if (IsVerifyUser)
                    this.CloseSelected.Active.SetItemValue("Enabled", true);

            }
            else if (View.GetType() == typeof(DetailView))
            {

                if (selectedObject.IsNew)
                {
                }
                else
                {
                    if (selectedObject.IsAccepted)
                    {
                        if (selectedObject.ApprovalStatus == ApprovalStatuses.Not_Applicable || selectedObject.ApprovalStatus == ApprovalStatuses.Approved)
                        {
                            //this.SetPaidDate.Active.SetItemValue("Enabled", true);
                        }
                    }
                    if (IsClaimUser || IsClaimSuperUser)
                    {
                        this.DuplicateDoc.Active.SetItemValue("Enabled", true);
                    }

                    if (selectedObject.IsCancelled || selectedObject.IsPosted)
                    { }
                    else
                    {
                        if (!selectedObject.IsPassed)
                        {
                            if (IsClaimSuperUser)
                            {
                                this.CancelDoc.Active.SetItemValue("Enabled", true);
                                this.PassDoc.Active.SetItemValue("Enabled", true);
                            }
                            else if (IsClaimUser)
                            {
                                if (user.Oid == selectedObject.CreateUser.Oid)
                                {
                                    this.CancelDoc.Active.SetItemValue("Enabled", true);
                                    this.PassDoc.Active.SetItemValue("Enabled", true);
                                }
                            }
                        }
                        else
                        {
                            if (!selectedObject.IsClosed)
                            {
                                if (IsAcceptanceUser && !selectedObject.IsAccepted)
                                    this.AcceptDoc.Active.SetItemValue("Enabled", true);

                                //if (IsAcceptanceUser && selectedObject.IsAccepted)
                                //{
                                //    if (selectedObject.ApprovalStatus == ApprovalStatuses.Rejected)
                                //    {
                                //        if (user.Oid == selectedObject.CreateUser.Oid || IsClaimSuperUser)
                                //            this.RejectDoc.Active.SetItemValue("Enabled", true);
                                //    }
                                //}
                                if (IsRejectApproveRole && selectedObject.IsAccepted)
                                {
                                    if (selectedObject.ApprovalStatus == ApprovalStatuses.Required_Approval || selectedObject.ApprovalStatus == ApprovalStatuses.Not_Applicable || selectedObject.ApprovalStatus == ApprovalStatuses.Approved)
                                    {
                                        this.RejectDoc.Active.SetItemValue("Enabled", true);
                                    }
                                }
                                if (IsVerifyUser && selectedObject.IsAccepted)
                                {
                                    if (selectedObject.ApprovalStatus == ApprovalStatuses.Not_Applicable || selectedObject.ApprovalStatus == ApprovalStatuses.Approved)
                                    {
                                        this.CloseDoc.Active.SetItemValue("Enabled", true);
                                    }
                                }
                                if (IsApprovalUser && selectedObject.IsAccepted && selectedObject.IsApprovalUserCheck)
                                {
                                    if (selectedObject.ApprovalStatus != ApprovalStatuses.Not_Applicable)
                                    {
                                        this.Approval.Active.SetItemValue("Enabled", !IsSimpleApproval);
                                        this.ApprovalApprove.Active.SetItemValue("Enabled", IsSimpleApproval);
                                        this.ApprovalReject.Active.SetItemValue("Enabled", IsSimpleApproval);
                                    }
                                }
                            }
                            else
                            {
                                if (IsVerifyUser && selectedObject.IsAccepted)
                                    this.ReopenDoc.Active.SetItemValue("Enabled", true);

                            }
                        }
                    }
                }
                enableButton();
            }
        }
        private void enableButton()
        {
            //this.CancelDoc.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.Edit);
            //this.PassDoc.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.Edit);
            //this.RejectDoc.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.Edit);
            //this.AcceptDoc.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.Edit);
            //this.CloseDoc.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.Edit);
            //this.ReopenDoc.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.Edit);
            this.DuplicateDoc.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.View);
            this.SetPaidDate.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.View);
            //this.Approval.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.Edit);
            //this.ApprovalApprove.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.Edit);
            //this.ApprovalReject.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.Edit);
        }
        private void ClaimTrxControllers_ViewEditModeChanged(object sender, EventArgs e)
        {
            enableButton();
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
                ((DetailView)View).ViewEditModeChanged -= ClaimTrxControllers_ViewEditModeChanged;
                if (recordnaviator != null)
                {
                    recordnaviator.PreviousObjectAction.Executed -= PreviousObjectAction_Executed;
                    recordnaviator.NextObjectAction.Executed -= NextObjectAction_Executed;
                }
            }
            NewObjectViewController controller = Frame.GetController<NewObjectViewController>();
            if (controller != null)
            {
                //controller.NewObjectAction.Execute += NewObjectAction_Execute;
                controller.ObjectCreated -= Controller_ObjectCreated;
            }
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void StringParametersCustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            bool err = false;
            //if (((DetailView)View).ViewEditMode == ViewEditMode.View)
            //{
            //    genCon.showMsg("Failed", "Viewing Document cannot proceed.", InformationType.Error);
            //    err = true;
            //}
            if (err)
            { }
            else
            {
                if (View is DetailView)
                {
                    ClaimTrxs selectedObject = (ClaimTrxs)View.CurrentObject;
                    if (selectedObject.IsNew && !err)
                    {
                        genCon.showMsg("Failed", "New Document cannot proceed.", InformationType.Error);
                        err = true;
                    }
                    else if (selectedObject.ApprovalStatus == ApprovalStatuses.Required_Approval)
                    {
                        if (e.Action.Id != this.RejectDoc.Id)
                        {
                            genCon.showMsg("Failed", "Approval in process.", InformationType.Error);
                            err = true;
                        }
                    }
                    else
                    {
                        #region 1st layer cancel and post document cannot proceed
                        if (selectedObject.IsCancelled && !err)
                        {
                            genCon.showMsg("Failed", "Cancelled Document cannot proceed.", InformationType.Error);
                            err = true;
                        }
                        if (selectedObject.IsPosted && !err)
                        {
                            genCon.showMsg("Failed", "Posted Document cannot proceed.", InformationType.Error);
                            err = true;
                        }
                        #endregion
                        #region 1st layer approval rejected cannot proceed
                        if (selectedObject.ApprovalStatus == ApprovalStatuses.Rejected && e.Action.Id != this.RejectDoc.Id)
                        {
                            if (!err)
                            {
                                genCon.showMsg("Failed", "Reject in Approval.", InformationType.Error);
                                err = true;
                            }
                        }
                        #endregion
                        #region 1st layer check passdoc and canceldoc

                        if (e.Action.Id == this.PassDoc.Id || e.Action.Id == this.CancelDoc.Id)
                        {
                            if (!selectedObject.IsClaimUserCheck && !err)
                            {
                                genCon.showMsg("Failed", "This is not a Claim User process.", InformationType.Error);
                                err = true;
                            }
                            if (selectedObject.IsPassed && !err)
                            {
                                genCon.showMsg("Failed", "Passed Document cannot proceed.", InformationType.Error);
                                err = true;
                            }
                            if (selectedObject.IsAccepted && !err)
                            {
                                genCon.showMsg("Failed", "Accepted Document cannot proceed.", InformationType.Error);
                                err = true;
                            }
                            if (selectedObject.IsClosed && !err)
                            {
                                genCon.showMsg("Failed", "Closed Document cannot proceed.", InformationType.Error);
                                err = true;
                            }

                        }
                        #endregion
                        else
                        {
                            #region 2nd layer pending and reject document cannot proceed
                            if (!selectedObject.IsPassed && !err)
                            {
                                genCon.showMsg("Failed", "Pending Document cannot proceed.", InformationType.Error);
                                err = true;
                            }
                            if (selectedObject.IsRejected && !err)
                            {
                                genCon.showMsg("Failed", "Rejected Document cannot proceed.", InformationType.Error);
                                err = true;
                            }
                            #endregion
                            #region 2nd layer check AcceptDoc and rejectdoc
                            if (e.Action.Id == this.AcceptDoc.Id || e.Action.Id == this.RejectDoc.Id)
                            {
                                if (selectedObject.IsClosed && !err)
                                {
                                    genCon.showMsg("Failed", "Closed Document cannot proceed.", InformationType.Error);
                                    err = true;
                                }
                                if (e.Action.Id == this.AcceptDoc.Id)
                                {
                                    if (!selectedObject.IsAcceptUserCheck && !err)
                                    {
                                        genCon.showMsg("Failed", "This is not a Acceptance User process.", InformationType.Error);
                                        err = true;
                                    }
                                    if (selectedObject.IsAccepted && !err)
                                    {
                                        genCon.showMsg("Failed", "Accepted Document cannot proceed.", InformationType.Error);
                                        err = true;
                                    }
                                }
                                else if (e.Action.Id == this.RejectDoc.Id)
                                {
                                    if (selectedObject.IsAccepted)
                                    {
                                        #region layer 2 when Accept require verify and Acceptance user role
                                        if (selectedObject.ApprovalStatus == ApprovalStatuses.Rejected && !err)
                                        {
                                            if (selectedObject.IsAcceptUserCheck || selectedObject.IsVerifyUserCheck)
                                            { }
                                            else
                                            {
                                                genCon.showMsg("Failed", "This is not a Acceptance/Verify User process.", InformationType.Error);
                                                err = true;
                                            }
                                        }
                                        else
                                        {
                                            if (!selectedObject.IsVerifyUserCheck && !err)
                                            {
                                                genCon.showMsg("Failed", "This is not a Verify User process.", InformationType.Error);
                                                err = true;
                                            }
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region layer 2 when not Accept require Acceptance user role
                                        if (!selectedObject.IsAcceptUserCheck && !err)
                                        {
                                            genCon.showMsg("Failed", "This is not a Acceptance User process.", InformationType.Error);
                                            err = true;
                                        }
                                        #endregion
                                    }
                                }
                            }
                            #endregion
                            else
                            {
                                #region 3rd layer not Accept cannot proceed
                                if (!selectedObject.IsAccepted && !err)
                                {
                                    genCon.showMsg("Failed", "Pending for Accepted Document cannot proceed.", InformationType.Error);
                                    err = true;
                                }
                                #endregion
                                #region 3rd layer check closedoc and reopendoc
                                if (e.Action.Id == this.CloseDoc.Id)
                                {
                                    if (!selectedObject.IsVerifyUserCheck && !err)
                                    {
                                        genCon.showMsg("Failed", "This is not a Verify User process.", InformationType.Error);
                                        err = true;
                                    }
                                    if (selectedObject.IsClosed && !err)
                                    {
                                        genCon.showMsg("Failed", "Closed Document cannot proceed.", InformationType.Error);
                                        err = true;
                                    }
                                    if (selectedObject.ClaimTrxPostDetail.Sum(p => p.Amount) != 0 && !err)
                                    {
                                        genCon.showMsg("Failed", "Amount is not valid.", InformationType.Error);
                                        err = true;
                                    }
                                }
                                if (e.Action.Id == this.ReopenDoc.Id)
                                {
                                    if (!selectedObject.IsVerifyUserCheck && !err)
                                    {
                                        genCon.showMsg("Failed", "This is not a Verify User process.", InformationType.Error);
                                        err = true;
                                    }
                                    if (!selectedObject.IsClosed && !err)
                                    {
                                        genCon.showMsg("Failed", "Not Closed Document cannot proceed.", InformationType.Error);
                                        err = true;
                                    }
                                }
                                #endregion 3rd layer

                            }
                        }
                    }
                }
            }

            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<StringParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = err;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;

        }

        private void PassDoc_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            ClaimTrxs selectedObject = (ClaimTrxs)e.CurrentObject;
            StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;

            if (((DetailView)View).ViewEditMode == ViewEditMode.View)
                ((DetailView)View).ViewEditMode = ViewEditMode.Edit;

            ModificationsController controller = Frame.GetController<ModificationsController>();

            generatePostingDetail(selectedObject);

            if (controller != null)
            {
                if (!controller.SaveAction.DoExecute())
                {
                    genCon.showMsg("Failed", "Process failed. Please contact administrator", InformationType.Error);
                    return;
                }
            }

            #region sp_DocValidation

            //SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
            XPObjectSpace persistentObjectSpace = (XPObjectSpace)Application.CreateObjectSpace();

            int ErrCode = 0;
            string ErrText = "";

            SelectedData sprocData = persistentObjectSpace.Session.ExecuteSproc("sp_DocValidation", new OperandValue(user.UserName), new OperandValue(selectedObject.Oid), new OperandValue("ClaimTrxs"));

            if (sprocData.ResultSet.Count() > 0)
            {
                if (sprocData.ResultSet[0].Rows.Count() > 0)
                {
                    foreach (SelectStatementResultRow row in sprocData.ResultSet[0].Rows)
                    {
                        if (row.Values[0] != null)
                        {
                            ErrCode = int.Parse(row.Values[0].ToString());
                            ErrText = row.Values[1].ToString();
                        }
                    }
                }
            }
            persistentObjectSpace.Session.DropIdentityMap();
            persistentObjectSpace.Dispose();
            sprocData = null;

            if (ErrCode != 0)
            {
                genCon.showMsg("Failed", "[ErrCode=" + ErrCode.ToString() + "] ErrText=" + ErrText, InformationType.Error);
                return;
            }
            #endregion

            selectedObject.IsPassed = true;
            selectedObject.IsRejected = false;
            selectedObject.CurrentAppStage = null;
            ClaimTrxDocStatuses ds = ObjectSpace.CreateObject<ClaimTrxDocStatuses>();
            ds.DocStatus = DocumentStatus.DocPassed;
            ds.DocRemarks = p.ParamString;
            selectedObject.ClaimTrxDocStatus.Add(ds);
            //selectedObject.OnPropertyChanged("ClaimTrxDocStatus");

            if (selectedObject.Company.IsPassAccept)
            {
                AcceptDoc_Execute(sender, e); // assignDocNum in AcceptDoc_Execute
            }
            else if (!selectedObject.Company.IsPassAccept)
            {                
                selectedObject.assignDocNum();

                if (controller != null)
                {
                    if (!controller.SaveAction.DoExecute())
                    {
                        genCon.showMsg("Failed", "Process failed. Please contact administrator", InformationType.Error);
                        return;
                    }
                }

                //IObjectSpace os = Application.CreateObjectSpace();
                //ClaimTrxs claimtrx = os.FindObject<ClaimTrxs>(new BinaryOperator("Oid", selectedObject.Oid));
                //genCon.openNewView(os, claimtrx, ViewEditMode.View);
                ((DetailView)View).ViewEditMode = ViewEditMode.View;
                //View.BreakLinksToControls();
                //View.CreateControls();
                RefreshController refreshcontroller = Frame.GetController<RefreshController>();
                if (refreshcontroller != null)
                    refreshcontroller.RefreshAction.DoExecute();
                genCon.showMsg("Successful", "Passing Done.", InformationType.Success);
                //resetButton();
            }
        }

        private void CancelDoc_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (((DetailView)View).ViewEditMode == ViewEditMode.View)
                ((DetailView)View).ViewEditMode = ViewEditMode.Edit;

            ClaimTrxs selectedObject = (ClaimTrxs)e.CurrentObject;
            StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;

            selectedObject.IsCancelled = true;
            selectedObject.IsPassed = false;
            ClaimTrxDocStatuses ds = ObjectSpace.CreateObject<ClaimTrxDocStatuses>();
            ds.DocStatus = DocumentStatus.Cancelled;
            ds.DocRemarks = p.ParamString;
            selectedObject.ClaimTrxDocStatus.Add(ds);
            //selectedObject.OnPropertyChanged("ClaimTrxDocStatus");

            ModificationsController controller = Frame.GetController<ModificationsController>();
            if (controller != null)
            {
                if (!controller.SaveAction.DoExecute())
                {
                    genCon.showMsg("Failed", "Process failed. Please contact administrator", InformationType.Error);
                    return;
                }
            }

            //IObjectSpace os = Application.CreateObjectSpace();
            //ClaimTrxs claimtrx = os.FindObject<ClaimTrxs>(new BinaryOperator("Oid", selectedObject.Oid));
            //genCon.openNewView(os, claimtrx, ViewEditMode.View);
            ((DetailView)View).ViewEditMode = ViewEditMode.View;
            //View.BreakLinksToControls();
            //View.CreateControls();
            RefreshController refreshcontroller = Frame.GetController<RefreshController>();
            if (refreshcontroller != null)
                refreshcontroller.RefreshAction.DoExecute();
            genCon.showMsg("Successful", "Cancel Done.", InformationType.Success);
            //resetButton();

        }

        private void RejectDoc_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (((DetailView)View).ViewEditMode == ViewEditMode.View)
                ((DetailView)View).ViewEditMode = ViewEditMode.Edit;

            ClaimTrxs selectedObject = (ClaimTrxs)e.CurrentObject;
            StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;

            selectedObject.IsRejected = true;
            selectedObject.IsAccepted = false;
            selectedObject.IsPassed = false;
            selectedObject.ApprovalStatus = ApprovalStatuses.Not_Applicable;
            selectedObject.CurrentAppStage = null;
            ClaimTrxDocStatuses ds = ObjectSpace.CreateObject<ClaimTrxDocStatuses>();
            ds.DocStatus = DocumentStatus.Rejected;
            ds.DocRemarks = p.ParamString;
            selectedObject.ClaimTrxDocStatus.Add(ds);
            //selectedObject.OnPropertyChanged("ClaimTrxDocStatus");

            ModificationsController controller = Frame.GetController<ModificationsController>();
            if (controller != null)
            {
                if (!controller.SaveAction.DoExecute())
                {
                    genCon.showMsg("Failed", "Process failed. Please contact administrator", InformationType.Error);
                    return;
                }
            }
            //IObjectSpace os = Application.CreateObjectSpace();
            //ClaimTrxs claimtrx = os.FindObject<ClaimTrxs>(new BinaryOperator("Oid", selectedObject.Oid));
            //int CurrentApprovalStageOid = claimtrx.GetCurrentApprovalStageOid();
            //if (CurrentApprovalStageOid > 0)
            //    claimtrx.CurrentAppStage = claimtrx.Session.GetObjectByKey<ClaimTrxAppStages>(CurrentApprovalStageOid);
            //else
            //    claimtrx.CurrentAppStage = null;
            //os.CommitChanges();
            ((DetailView)View).ViewEditMode = ViewEditMode.View;
            //View.BreakLinksToControls();
            //View.CreateControls();
            RefreshController refreshcontroller = Frame.GetController<RefreshController>();
            if (refreshcontroller != null)
                refreshcontroller.RefreshAction.DoExecute();

            try
            {
                IObjectSpace emailos = Application.CreateObjectSpace();
                EmailSents emailobj = null;
                int emailcnt = 0;
                string emailbody = "";
                string emailsubject = "Claim Document Rejected";
                string emailaddress = "";
                Guid emailuser;

                emailbody = "<html><body>" + selectedObject.DocNum + " is rejected, please click below link " + "<br/><a href=\"" + GeneralSettings.appurl + string.Format(selectedObject.Company.ClaimLink, selectedObject.Oid.ToString()) + "\">Link</a></body></html>";

                emailaddress = selectedObject.Employee.UserEmail;
                emailuser = (Guid)selectedObject.Employee.SystemUser.Oid;

                emailobj = emailos.CreateObject<EmailSents>();
                emailobj.CreateDate = (DateTime?)DateTime.Now;
                //assign body will get error???
                emailobj.EmailBody = emailbody;
                emailobj.EmailSubject = emailsubject;
                emailobj.ClaimTrx = emailobj.Session.GetObjectByKey<ClaimTrxs>(selectedObject.Oid);

                EmailSentDetails emaildtl = emailos.CreateObject<EmailSentDetails>();
                emaildtl.EmailUser = emaildtl.Session.GetObjectByKey<SystemUsers>(emailuser);
                emaildtl.EmailAddress = emailaddress;
                emailobj.EmailSentDetail.Add(emaildtl);
                emailcnt++;

                if (selectedObject.CreateUser.Oid != selectedObject.Employee.SystemUser.Oid)
                {
                    Employees emp = emaildtl.Session.FindObject<Employees>(CriteriaOperator.Parse("SystemUser.Oid=?", selectedObject.CreateUser.Oid));
                    emaildtl = emailos.CreateObject<EmailSentDetails>();
                    emaildtl.EmailUser = emaildtl.Session.GetObjectByKey<SystemUsers>(emp.SystemUser.Oid);
                    emaildtl.EmailAddress = emp.UserEmail;
                    emailobj.EmailSentDetail.Add(emaildtl);
                    emailcnt++;
                }
                //emailobj.Save();
                if (emailcnt > 0)
                    emailos.CommitChanges();

                if (emailobj != null)
                {
                    genCon.SendEmail_By_Object(emailobj);
                }
            }
            catch (Exception ex)
            {
                genCon.showMsg("Reject doc", ex.Message, InformationType.Error);
                return;
            }

            genCon.showMsg("Successful", "Rejected Done.", InformationType.Success);
            //resetButton();

        }
        public void generatePostingDetail(ClaimTrxs selectedObject)
        {
            #region generate posting detail
            decimal amount = 0;
            #region remove generated records
            if (selectedObject.ClaimTrxPostDetail.Count > 0)
            {
                CriteriaOperator op = CriteriaOperator.Parse("ClaimTrx.Oid=?", selectedObject.Oid);
                XPCollection<ClaimTrxPostDetails> xpcol = (XPCollection<ClaimTrxPostDetails>)ObjectSpace.GetObjects<ClaimTrxPostDetails>(op);
                ObjectSpace.Delete(xpcol);

            }
            if (selectedObject.ClaimTrxAppStage.Count > 0)
            {
                CriteriaOperator op = CriteriaOperator.Parse("ClaimTrx.Oid=?", selectedObject.Oid);
                XPCollection<ClaimTrxAppStages> xpcol = (XPCollection<ClaimTrxAppStages>)ObjectSpace.GetObjects<ClaimTrxAppStages>(op);
                ObjectSpace.Delete(xpcol);
            }
            if (selectedObject.ClaimTrxAppStatus.Count > 0)
            {
                CriteriaOperator op = CriteriaOperator.Parse("ClaimTrx.Oid=?", selectedObject.Oid);
                XPCollection<ClaimTrxAppStatuses> xpcol = (XPCollection<ClaimTrxAppStatuses>)ObjectSpace.GetObjects<ClaimTrxAppStatuses>(op);
                ObjectSpace.Delete(xpcol);
            }

            #endregion
            decimal combineamount = 0;
            ClaimTrxPostDetails postds = null;
            foreach (ClaimTrxDetails obj in selectedObject.ClaimTrxDetail)
            {
                if (obj.ClaimType != null)
                {
                    #region add trxpostdetails note level
                    if (obj.ClaimType.IsNote)
                    {
                        if (obj.ClaimType.IsCombineAmount)
                        {
                            if (obj.Amount != 0)
                            {
                                combineamount = obj.Amount;
                            }
                            foreach (ClaimTrxDetailNotes dtl in obj.ClaimTrxDetailNote)
                            {
                                if (dtl.Amount != 0)
                                {
                                    combineamount += dtl.Amount;
                                }
                            }
                            if (combineamount != 0)
                            {
                                postds = ObjectSpace.CreateObject<ClaimTrxPostDetails>();
                                postds.Tax = ObjectSpace.FindObject<Taxes>(new BinaryOperator("BoCode", obj.Tax.BoCode));
                                postds.TaxAmount = obj.TaxAmount;
                                postds.RefNo = obj.RefNo;
                                if (obj.Remarks != null)
                                {
                                    if (obj.Remarks.Length > 50)
                                        postds.Remarks = obj.Remarks.Substring(0, 50);
                                    else
                                        postds.Remarks = obj.Remarks;
                                }
                                postds.Amount = combineamount;
                                postds.IsLineJERemarks = obj.ClaimType.IsLineJERemarks;
                                if (obj.Project != null)
                                    postds.Project = ObjectSpace.GetObjectByKey<Projects>(obj.Project.Oid);
                                else
                                    if (selectedObject.Project != null)
                                {
                                    postds.Project = ObjectSpace.GetObjectByKey<Projects>(selectedObject.Project.Oid);
                                }
                                if (obj.Department != null)
                                    postds.Department = ObjectSpace.GetObjectByKey<Departments>(obj.Department.Oid);
                                else if (selectedObject.Department != null)
                                {
                                    postds.Department = ObjectSpace.GetObjectByKey<Departments>(selectedObject.Department.Oid);
                                }
                                if (obj.Division != null)
                                    postds.Division = ObjectSpace.GetObjectByKey<Divisions>(obj.Division.Oid);
                                else if (selectedObject.Division != null)
                                {
                                    postds.Division = ObjectSpace.GetObjectByKey<Divisions>(selectedObject.Division.Oid);
                                }
                                if (obj.Brand != null)
                                    postds.Brand = ObjectSpace.GetObjectByKey<Brands>(obj.Brand.Oid);
                                else if (selectedObject.Brand != null)
                                {
                                    postds.Brand = ObjectSpace.GetObjectByKey<Brands>(selectedObject.Brand.Oid);
                                }

                                if (obj.ClaimType.Account != null)
                                {
                                    postds.Account = ObjectSpace.GetObjectByKey<Accounts>(obj.ClaimType.Account.Oid);
                                }
                                selectedObject.ClaimTrxPostDetail.Add(postds);
                            }
                        }
                        else
                        {
                            if (obj.Amount != 0)
                            {
                                postds = ObjectSpace.CreateObject<ClaimTrxPostDetails>();
                                postds.Tax = ObjectSpace.FindObject<Taxes>(new BinaryOperator("BoCode", obj.Tax.BoCode));
                                postds.TaxAmount = obj.TaxAmount;
                                postds.RefNo = obj.RefNo;
                                if (obj.Remarks != null)
                                {
                                    if (obj.Remarks.Length > 50)
                                        postds.Remarks = obj.Remarks.Substring(0, 50);
                                    else
                                        postds.Remarks = obj.Remarks;
                                }
                                postds.Amount = obj.Amount;
                                postds.IsLineJERemarks = obj.ClaimType.IsLineJERemarks;
                                if (obj.Project != null)
                                    postds.Project = ObjectSpace.GetObjectByKey<Projects>(obj.Project.Oid);
                                else
                                    if (selectedObject.Project != null)
                                {
                                    postds.Project = ObjectSpace.GetObjectByKey<Projects>(selectedObject.Project.Oid);
                                }
                                if (obj.Department != null)
                                    postds.Department = ObjectSpace.GetObjectByKey<Departments>(obj.Department.Oid);
                                else if (selectedObject.Department != null)
                                {
                                    postds.Department = ObjectSpace.GetObjectByKey<Departments>(selectedObject.Department.Oid);
                                }
                                if (obj.Division != null)
                                    postds.Division = ObjectSpace.GetObjectByKey<Divisions>(obj.Division.Oid);
                                else if (selectedObject.Division != null)
                                {
                                    postds.Division = ObjectSpace.GetObjectByKey<Divisions>(selectedObject.Division.Oid);
                                }
                                if (obj.Brand != null)
                                    postds.Brand = ObjectSpace.GetObjectByKey<Brands>(obj.Brand.Oid);
                                else if (selectedObject.Brand != null)
                                {
                                    postds.Brand = ObjectSpace.GetObjectByKey<Brands>(selectedObject.Brand.Oid);
                                }

                                if (obj.ClaimType.Account != null)
                                {
                                    postds.Account = ObjectSpace.GetObjectByKey<Accounts>(obj.ClaimType.Account.Oid);
                                }
                                selectedObject.ClaimTrxPostDetail.Add(postds);

                            }

                            #region add trxpostdetails note detail level
                            foreach (ClaimTrxDetailNotes dtl in obj.ClaimTrxDetailNote)
                            {
                                if (dtl.Amount != 0)
                                {
                                    postds = ObjectSpace.CreateObject<ClaimTrxPostDetails>();
                                    postds.Tax = ObjectSpace.FindObject<Taxes>(new BinaryOperator("BoCode", obj.Tax.BoCode));
                                    postds.TaxAmount = dtl.TaxAmount;
                                    postds.RefNo = dtl.RefNo;
                                    if (dtl.Remarks != null)
                                    {
                                        if (dtl.Remarks.Length > 50)
                                            postds.Remarks = dtl.Remarks.Substring(0, 50);
                                        else
                                            postds.Remarks = dtl.Remarks;
                                    }
                                    postds.Amount = dtl.Amount;
                                    postds.IsLineJERemarks = obj.ClaimType.IsLineJERemarks;
                                    if (dtl.Project != null)
                                        postds.Project = ObjectSpace.GetObjectByKey<Projects>(dtl.Project.Oid);
                                    else if (obj.Project != null)
                                        postds.Project = ObjectSpace.GetObjectByKey<Projects>(obj.Project.Oid);
                                    else if (selectedObject.Project != null)
                                    {
                                        postds.Project = ObjectSpace.GetObjectByKey<Projects>(selectedObject.Project.Oid);
                                    }
                                    if (dtl.Department != null)
                                        postds.Department = ObjectSpace.GetObjectByKey<Departments>(dtl.Department.Oid);
                                    else if (obj.Department != null)
                                        postds.Department = ObjectSpace.GetObjectByKey<Departments>(obj.Department.Oid);
                                    else if (selectedObject.Department != null)
                                    {
                                        postds.Department = ObjectSpace.GetObjectByKey<Departments>(selectedObject.Department.Oid);
                                    }
                                    if (dtl.Division != null)
                                        postds.Division = ObjectSpace.GetObjectByKey<Divisions>(dtl.Division.Oid);
                                    else if (obj.Division != null)
                                        postds.Division = ObjectSpace.GetObjectByKey<Divisions>(obj.Division.Oid);
                                    else if (selectedObject.Division != null)
                                    {
                                        postds.Division = ObjectSpace.GetObjectByKey<Divisions>(selectedObject.Division.Oid);
                                    }
                                    if (dtl.Brand != null)
                                        postds.Brand = ObjectSpace.GetObjectByKey<Brands>(dtl.Brand.Oid);
                                    else if (obj.Brand != null)
                                        postds.Brand = ObjectSpace.GetObjectByKey<Brands>(obj.Brand.Oid);
                                    else if (selectedObject.Brand != null)
                                    {
                                        postds.Brand = ObjectSpace.GetObjectByKey<Brands>(selectedObject.Brand.Oid);
                                    }

                                    if (obj.ClaimType.Account != null)
                                    {
                                        postds.Account = ObjectSpace.GetObjectByKey<Accounts>(obj.ClaimType.Account.Oid);
                                    }
                                    selectedObject.ClaimTrxPostDetail.Add(postds);
                                }
                            }
                            #endregion
                        }
                    }

                    #region add trxpostdetails mileage detail level
                    if (obj.ClaimType.IsMileage)
                    {
                        if (obj.ClaimType.IsCombineAmount)
                        {
                            foreach (ClaimTrxKMs dtl in obj.ClaimTrxDetailKM)
                            {
                                if (dtl.Amount != 0)
                                {
                                    combineamount += dtl.Amount;
                                }
                            }
                            if (combineamount != 0)
                            {
                                postds = ObjectSpace.CreateObject<ClaimTrxPostDetails>();
                                postds.Tax = ObjectSpace.FindObject<Taxes>(new BinaryOperator("BoCode", obj.Tax.BoCode));
                                postds.RefNo = obj.RefNo;
                                if (obj.Remarks != null)
                                {
                                    if (obj.Remarks.Length > 50)
                                        postds.Remarks = obj.Remarks.Substring(0, 50);
                                    else
                                        postds.Remarks = obj.Remarks;
                                }
                                postds.Amount = combineamount;
                                postds.IsLineJERemarks = obj.ClaimType.IsLineJERemarks;
                                if (obj.Project != null)
                                    postds.Project = ObjectSpace.GetObjectByKey<Projects>(obj.Project.Oid);
                                else if (selectedObject.Project != null)
                                {
                                    postds.Project = ObjectSpace.GetObjectByKey<Projects>(selectedObject.Project.Oid);
                                }
                                if (obj.Department != null)
                                    postds.Department = ObjectSpace.GetObjectByKey<Departments>(obj.Department.Oid);
                                else if (selectedObject.Department != null)
                                {
                                    postds.Department = ObjectSpace.GetObjectByKey<Departments>(selectedObject.Department.Oid);
                                }
                                if (obj.Division != null)
                                    postds.Division = ObjectSpace.GetObjectByKey<Divisions>(obj.Division.Oid);
                                else if (selectedObject.Division != null)
                                {
                                    postds.Division = ObjectSpace.GetObjectByKey<Divisions>(selectedObject.Division.Oid);
                                }
                                if (obj.Brand != null)
                                    postds.Brand = ObjectSpace.GetObjectByKey<Brands>(obj.Brand.Oid);
                                else if (selectedObject.Brand != null)
                                {
                                    postds.Brand = ObjectSpace.GetObjectByKey<Brands>(selectedObject.Brand.Oid);
                                }

                                if (obj.ClaimType.Account != null)
                                {
                                    postds.Account = ObjectSpace.GetObjectByKey<Accounts>(obj.ClaimType.Account.Oid);
                                }
                                selectedObject.ClaimTrxPostDetail.Add(postds);
                            }
                        }
                        else
                        {
                            foreach (ClaimTrxKMs dtl in obj.ClaimTrxDetailKM)
                            {
                                if (dtl.Amount != 0)
                                {
                                    amount = dtl.Amount;

                                    foreach (ClaimTrxMileages mls in obj.ClaimTrxDetailMileage)
                                    {
                                        postds = ObjectSpace.CreateObject<ClaimTrxPostDetails>();
                                        postds.Tax = ObjectSpace.FindObject<Taxes>(new BinaryOperator("BoCode", obj.Tax.BoCode));
                                        postds.RefNo = mls.RefNo;
                                        if (mls.Remarks != null)
                                        {
                                            if (mls.Remarks.Length > 50)
                                                postds.Remarks = mls.Remarks.Substring(0, 50);
                                            else
                                                postds.Remarks = mls.Remarks;
                                        }
                                        postds.Amount = Math.Round((decimal)mls.KM / (decimal)dtl.KM * amount, 2);
                                        postds.IsLineJERemarks = obj.ClaimType.IsLineJERemarks;
                                        if (mls.Project != null)
                                            postds.Project = ObjectSpace.GetObjectByKey<Projects>(mls.Project.Oid);
                                        else if (obj.Project != null)
                                            postds.Project = ObjectSpace.GetObjectByKey<Projects>(obj.Project.Oid);
                                        else if (selectedObject.Project != null)
                                        {
                                            postds.Project = ObjectSpace.GetObjectByKey<Projects>(selectedObject.Project.Oid);
                                        }
                                        if (mls.Department != null)
                                            postds.Department = ObjectSpace.GetObjectByKey<Departments>(mls.Department.Oid);
                                        else if (obj.Department != null)
                                            postds.Department = ObjectSpace.GetObjectByKey<Departments>(obj.Department.Oid);
                                        else if (selectedObject.Department != null)
                                        {
                                            postds.Department = ObjectSpace.GetObjectByKey<Departments>(selectedObject.Department.Oid);
                                        }
                                        if (mls.Division != null)
                                            postds.Division = ObjectSpace.GetObjectByKey<Divisions>(mls.Division.Oid);
                                        else if (obj.Division != null)
                                            postds.Division = ObjectSpace.GetObjectByKey<Divisions>(obj.Division.Oid);
                                        else if (selectedObject.Division != null)
                                        {
                                            postds.Division = ObjectSpace.GetObjectByKey<Divisions>(selectedObject.Division.Oid);
                                        }
                                        if (mls.Brand != null)
                                            postds.Brand = ObjectSpace.GetObjectByKey<Brands>(mls.Brand.Oid);
                                        else if (obj.Brand != null)
                                            postds.Brand = ObjectSpace.GetObjectByKey<Brands>(obj.Brand.Oid);
                                        else if (selectedObject.Brand != null)
                                        {
                                            postds.Brand = ObjectSpace.GetObjectByKey<Brands>(selectedObject.Brand.Oid);
                                        }

                                        if (obj.ClaimType.Account != null)
                                        {
                                            postds.Account = ObjectSpace.GetObjectByKey<Accounts>(obj.ClaimType.Account.Oid);
                                        }
                                        selectedObject.ClaimTrxPostDetail.Add(postds);
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                    #endregion

                }
            }

            foreach (ClaimTrxItems obj in selectedObject.ClaimTrxItem)
            {
                if (obj.Amount != 0)
                {
                    postds = ObjectSpace.CreateObject<ClaimTrxPostDetails>();
                    postds.Tax = ObjectSpace.FindObject<Taxes>(new BinaryOperator("BoCode", obj.Tax.BoCode));
                    postds.TaxAmount = obj.TaxAmount;
                    postds.RefNo = obj.RefNo;
                    if (obj.Remarks != null)
                    {
                        if (obj.Remarks.Length > 50)
                            postds.Remarks = obj.Remarks.Substring(0, 50);
                        else
                            postds.Remarks = obj.Remarks;
                    }
                    postds.Amount = obj.Amount;
                    if (obj.Project != null)
                        postds.Project = ObjectSpace.GetObjectByKey<Projects>(obj.Project.Oid);
                    else if (selectedObject.Project != null)
                    {
                        postds.Project = ObjectSpace.GetObjectByKey<Projects>(selectedObject.Project.Oid);
                    }
                    if (obj.Department != null)
                        postds.Department = ObjectSpace.GetObjectByKey<Departments>(obj.Department.Oid);
                    else if (selectedObject.Department != null)
                    {
                        postds.Department = ObjectSpace.GetObjectByKey<Departments>(selectedObject.Department.Oid);
                    }
                    if (obj.Division != null)
                        postds.Division = ObjectSpace.GetObjectByKey<Divisions>(obj.Division.Oid);
                    else if (selectedObject.Division != null)
                    {
                        postds.Division = ObjectSpace.GetObjectByKey<Divisions>(selectedObject.Division.Oid);
                    }
                    if (obj.Brand != null)
                        postds.Brand = ObjectSpace.GetObjectByKey<Brands>(obj.Brand.Oid);
                    else if (selectedObject.Brand != null)
                    {
                        postds.Brand = ObjectSpace.GetObjectByKey<Brands>(selectedObject.Brand.Oid);
                    }

                    if (obj.ClaimItem.Account != null)
                    {
                        postds.Account = ObjectSpace.GetObjectByKey<Accounts>(obj.ClaimItem.Account.Oid);
                    }
                    selectedObject.ClaimTrxPostDetail.Add(postds);

                }

            }

            foreach (ClaimTrxKMs obj in selectedObject.ClaimTrxKM)
            {
                if (obj.Amount != 0)
                {
                    amount = obj.Amount;

                    foreach (ClaimTrxMileages mls in selectedObject.ClaimTrxMileage)
                    {
                        if (mls.Mileage != obj.Mileage) continue;
                        postds = ObjectSpace.CreateObject<ClaimTrxPostDetails>();
                        postds.Tax = ObjectSpace.FindObject<Taxes>(new BinaryOperator("BoCode", GeneralSettings.defaultmileagetax));
                        postds.RefNo = mls.RefNo;
                        if (mls.Remarks != null)
                        {
                            if (mls.Remarks.Length > 50)
                                postds.Remarks = mls.Remarks.Substring(0, 50);
                            else
                                postds.Remarks = mls.Remarks;
                        }
                        postds.Amount = Math.Round((decimal)mls.KM / (decimal)obj.KM * amount, 2);

                        if (mls.Project != null)
                            postds.Project = ObjectSpace.GetObjectByKey<Projects>(mls.Project.Oid);
                        else if (selectedObject.Project != null)
                        {
                            postds.Project = ObjectSpace.GetObjectByKey<Projects>(selectedObject.Project.Oid);
                        }

                        if (mls.Department != null)
                            postds.Department = ObjectSpace.GetObjectByKey<Departments>(mls.Department.Oid);
                        else if (selectedObject.Department != null)
                        {
                            postds.Department = ObjectSpace.GetObjectByKey<Departments>(selectedObject.Department.Oid);
                        }
                        if (mls.Division != null)
                            postds.Division = ObjectSpace.GetObjectByKey<Divisions>(mls.Division.Oid);
                        else if (selectedObject.Division != null)
                        {
                            postds.Division = ObjectSpace.GetObjectByKey<Divisions>(selectedObject.Division.Oid);
                        }
                        if (mls.Brand != null)
                            postds.Brand = ObjectSpace.GetObjectByKey<Brands>(mls.Brand.Oid);
                        else if (selectedObject.Brand != null)
                        {
                            postds.Brand = ObjectSpace.GetObjectByKey<Brands>(selectedObject.Brand.Oid);
                        }

                        if (mls.Mileage.Account != null)
                        {
                            postds.Account = ObjectSpace.GetObjectByKey<Accounts>(mls.Mileage.Account.Oid);
                        }
                        selectedObject.ClaimTrxPostDetail.Add(postds);
                    }
                }
            }

            #region advance
            decimal totalclaimamount = 0;
            decimal offsetamount = 0;
            if (selectedObject.ClaimTrxPostDetail != null && selectedObject.ClaimTrxPostDetail.Count > 0)
            {
                foreach (ClaimTrxPostDetails obj in selectedObject.ClaimTrxPostDetail)
                {
                    totalclaimamount += obj.Amount;
                }
            }
            offsetamount = totalclaimamount;

            if (selectedObject.AdvanceAmount > 0)
            {
                postds = ObjectSpace.CreateObject<ClaimTrxPostDetails>();
                postds.Tax = ObjectSpace.FindObject<Taxes>(new BinaryOperator("BoCode", GeneralSettings.defaulttax));
                postds.RefNo = "";
                postds.Remarks = "Advance";
                postds.Amount = selectedObject.AdvanceAmount * -1;
                postds.SystemGen = true;

                //if (selectedObject.Project != null)
                //{
                //    postds.Project = ObjectSpace.GetObjectByKey<Projects>(selectedObject.Project.Oid);
                //}

                //if (selectedObject.Department != null)
                //{
                //    postds.Department = ObjectSpace.GetObjectByKey<Departments>(selectedObject.Department.Oid);
                //}

                if (selectedObject.Company.AdvanceControlAccount != null)
                {
                    postds.Account = ObjectSpace.GetObjectByKey<Accounts>(selectedObject.Company.AdvanceControlAccount.Oid);
                }
                selectedObject.ClaimTrxPostDetail.Add(postds);

                offsetamount = offsetamount - selectedObject.AdvanceAmount;
            }
            #endregion

            #region post to JE
            if (offsetamount != 0)
            {
                if (selectedObject.Company.PostToDocument == PostToDocuments.JE)
                {
                    postds = ObjectSpace.CreateObject<ClaimTrxPostDetails>();
                    postds.Tax = ObjectSpace.FindObject<Taxes>(new BinaryOperator("BoCode", GeneralSettings.defaulttax));
                    postds.RefNo = "";
                    postds.Remarks = selectedObject.getJERemarks();
                    postds.Amount = offsetamount * -1;
                    postds.SystemGen = true;

                    //if (selectedObject.Project != null)
                    //{
                    //    postds.Project = ObjectSpace.GetObjectByKey<Projects>(selectedObject.Project.Oid);
                    //}

                    //if (selectedObject.Department != null)
                    //{
                    //    postds.Department = ObjectSpace.GetObjectByKey<Departments>(selectedObject.Department.Oid);
                    //}

                    if (selectedObject.Company.ClaimControlAccount != null)
                    {
                        postds.Account = ObjectSpace.GetObjectByKey<Accounts>(selectedObject.Company.ClaimControlAccount.Oid);
                        //postds.Remarks = postds.Account.BoName;
                    }
                    selectedObject.ClaimTrxPostDetail.Add(postds);
                }
            }
            #endregion

            #endregion
        }
        private void AcceptDoc_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            ClaimTrxs selectedObject = (ClaimTrxs)e.CurrentObject;
            StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;

            if (((DetailView)View).ViewEditMode == ViewEditMode.View)
                ((DetailView)View).ViewEditMode = ViewEditMode.Edit;

            selectedObject.IsAccepted = true;
            ClaimTrxDocStatuses ds = ObjectSpace.CreateObject<ClaimTrxDocStatuses>();
            ds.DocStatus = DocumentStatus.Accepted;
            ds.DocRemarks = p.ParamString;
            selectedObject.ClaimTrxDocStatus.Add(ds);
            //selectedObject.OnPropertyChanged("ClaimTrxDocStatus");

            //generatePostingDetail(selectedObject);

            ModificationsController controller = Frame.GetController<ModificationsController>();
            if (controller != null)
            {
                if (!controller.SaveAction.DoExecute())
                {
                    genCon.showMsg("Failed", "Process failed. Please contact administrator", InformationType.Error);
                    return;
                }
            }

            //ModificationsController controller = Frame.GetController<ModificationsController>();
            //if (controller != null)
            //{
            //    controller.SaveAction.DoExecute();
            //}

            #region approval
            //string spsql = "exec sp_GetApproval " + user.UserName + "," + selectedObject.Oid.ToString() + "," + "'ClaimTrxs'";

            //SqlParameter param = new SqlParameter("@usercode", user.UserName);
            //SqlParameter param1 = new SqlParameter("@docid", selectedObject.Oid);
            //SqlParameter param2 = new SqlParameter("@objtype", "ClaimTrxs");

            //NonPersistentObjectSpace objectSpace = (NonPersistentObjectSpace)ObjectSpace;
            //XPObjectSpace persistentObjectSpace = (XPObjectSpace)objectSpace.AdditionalObjectSpaces[0];
            //IList<GetApproval> lists = persistentObjectSpace.Session.GetObjectsFromSproc<GetApproval>("sp_GetApproval", new OperandValue(user.UserName), new OperandValue(selectedObject.Oid), new OperandValue("ClaimTrxs")).ToList();

            //XPObjectSpace persistentObjectSpace = (XPObjectSpace)ObjectSpace;
            XPObjectSpace persistentObjectSpace = (XPObjectSpace)Application.CreateObjectSpace();
            //SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;

            //List<string> ToEmails = new List<string>();
            string emailbody = "";
            string emailsubject = "";
            string emailaddress = "";
            Guid emailuser;
            DateTime emailtime = DateTime.Now;
            SelectedData sprocData = persistentObjectSpace.Session.ExecuteSproc("sp_GetApproval", new OperandValue(user.UserName), new OperandValue(selectedObject.Oid), new OperandValue("ClaimTrxs"));

            IObjectSpace emailos = Application.CreateObjectSpace();
            EmailSents emailobj = null;
            int emailcnt = 0;

            if (sprocData.ResultSet.Count() > 0)
            {
                if (sprocData.ResultSet[0].Rows.Count() > 0)
                {
                    foreach (SelectStatementResultRow row in sprocData.ResultSet[0].Rows)
                    {
                        if (row.Values[1] != null)
                        {
                            emailcnt++;
                            emailbody = "<html><body>" + row.Values[3] + "<br/><a href=\"" + GeneralSettings.appurl + row.Values[2].ToString() + "\">Link</a></body></html>";
                            emailsubject = "Claim Document Approval Required";
                            emailaddress = row.Values[1].ToString();
                            emailuser = (Guid)row.Values[0];

                            if (emailcnt == 1)
                            {

                                //ToEmails.Add(emailaddress);

                                emailobj = emailos.CreateObject<EmailSents>();
                                emailobj.CreateDate = (DateTime?)emailtime;
                                //assign body will get error???
                                emailobj.EmailBody = emailbody;
                                emailobj.EmailSubject = emailsubject;
                                emailobj.ClaimTrx = emailobj.Session.GetObjectByKey<ClaimTrxs>(selectedObject.Oid);
                            }

                            EmailSentDetails emaildtl = emailos.CreateObject<EmailSentDetails>();
                            emaildtl.EmailUser = emaildtl.Session.GetObjectByKey<SystemUsers>(emailuser);
                            emaildtl.EmailAddress = emailaddress;
                            emailobj.EmailSentDetail.Add(emaildtl);
                        }
                    }
                    //emailobj.Save();
                    if (emailcnt > 0)
                        emailos.CommitChanges();
                }
            }
            persistentObjectSpace.Session.DropIdentityMap();
            persistentObjectSpace.Dispose();
            sprocData = null;

            if (emailobj != null)
            {
                genCon.SendEmail_By_Object(emailobj);
            }
            //if (ToEmails.Count > 0)
            //{
            //    if (genCon.SendEmail(emailsubject, emailbody, ToEmails) == 1)
            //    {
            //        if (emailos.IsModified)
            //            emailos.CommitChanges();
            //    }
            //}
            #endregion

            RefreshController refreshcontroller = Frame.GetController<RefreshController>();

            IObjectSpace os = Application.CreateObjectSpace();
            ClaimTrxs claimtrx = os.FindObject<ClaimTrxs>(new BinaryOperator("Oid", selectedObject.Oid));
            int CurrentApprovalStageOid = claimtrx.GetCurrentApprovalStageOid();
            if (CurrentApprovalStageOid > 0)
            {
                claimtrx.CurrentAppStage = claimtrx.Session.GetObjectByKey<ClaimTrxAppStages>(CurrentApprovalStageOid);
            }
            else
            {
                claimtrx.CurrentAppStage = null;
                if (claimtrx.Company.IsRejectWOApp)
                {
                    claimtrx.IsAccepted = false;
                    claimtrx.IsPassed = false;
                    claimtrx.IsRejected = true;
                    ClaimTrxDocStatuses claimstatus = os.CreateObject<ClaimTrxDocStatuses>();
                    claimstatus.DocStatus = DocumentStatus.Rejected;
                    claimstatus.DocRemarks = claimtrx.Company.RejectWOAppRemarks;
                    claimtrx.ClaimTrxDocStatus.Add(claimstatus);
                    os.CommitChanges();

                    genCon.openNewView(os, claimtrx, ViewEditMode.Edit);
                    genCon.showMsg("Failed", claimtrx.Company.RejectWOAppRemarks, InformationType.Error);

                    return;
                }
            }
            claimtrx.assignDocNum();
            os.CommitChanges();

            ((DetailView)View).ViewEditMode = ViewEditMode.View;
            //View.BreakLinksToControls();
            //View.CreateControls();
            if (refreshcontroller != null)
                refreshcontroller.RefreshAction.DoExecute();
            genCon.showMsg("Successful", "Acceptance Done.", InformationType.Success);
            //resetButton();

        }
        private void CloseSelected_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;

            if (View is ListView)
            {
                foreach (ClaimTrxs selectedObject in View.SelectedObjects)
                {
                    if (!selectedObject.IsClosed && selectedObject.IsAccepted && !selectedObject.IsCancelled && !selectedObject.IsRejected && selectedObject.IsVerifyUserCheck && (selectedObject.ApprovalStatus == ApprovalStatuses.Approved || selectedObject.ApprovalStatus == ApprovalStatuses.Not_Applicable))
                    {

                        genCloseDoc(selectedObject, p.ParamString);

                        ObjectSpace.CommitChanges();

                    }
                }
                RefreshController refreshcontroller = Frame.GetController<RefreshController>();
                if (refreshcontroller != null)
                    refreshcontroller.RefreshAction.DoExecute();
                genCon.showMsg("Successful", "Close Selected Claims Done.", InformationType.Success);
            }
        }

        private void genCloseDoc(ClaimTrxs selectedObject, string ParamString)
        {
            selectedObject.IsClosed = true;
            ClaimTrxDocStatuses ds = ObjectSpace.CreateObject<ClaimTrxDocStatuses>();
            ds.DocStatus = DocumentStatus.Closed;
            ds.DocRemarks = ParamString;
            selectedObject.ClaimTrxDocStatus.Add(ds);
            //selectedObject.OnPropertyChanged("ClaimTrxDocStatus");

            foreach (ClaimTrxPostDetails obj in selectedObject.ClaimTrxPostDetail)
            {
                if (obj.Project != null)
                {
                    obj.ProjectCode = obj.Project.PostCode;
                }
                else
                {
                    obj.ProjectCode = GeneralSettings.defemptyproject.Trim();
                }
                if (obj.Department != null)
                {
                    obj.DepartmentCode = obj.Department.PostCode;
                }
                else
                {
                    obj.DepartmentCode = GeneralSettings.defemptydepartment.Trim();
                }
                if (obj.Division != null)
                {
                    obj.DivisionCode = obj.Division.PostCode;
                }
                else
                {
                    obj.DivisionCode = GeneralSettings.defemptydivision.Trim();
                }
                if (obj.Brand != null)
                {
                    obj.BrandCode = obj.Brand.PostCode;
                }
                else
                {
                    obj.BrandCode = GeneralSettings.defemptybrand.Trim();
                }
                if (obj.Account != null)
                {
                    obj.AccountCode = obj.Account.SystemCode;
                }
            }

        }
        private void CloseDoc_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (((DetailView)View).ViewEditMode == ViewEditMode.View)
                ((DetailView)View).ViewEditMode = ViewEditMode.Edit;

            StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;

            ClaimTrxs selectedObject = (ClaimTrxs)e.CurrentObject;

            genCloseDoc(selectedObject, p.ParamString);

            ModificationsController controller = Frame.GetController<ModificationsController>();
            if (controller != null)
            {
                if (!controller.SaveAction.DoExecute())
                {
                    genCon.showMsg("Failed", "Process failed. Please contact administrator", InformationType.Error);
                    return;
                }
            }

            //IObjectSpace os = Application.CreateObjectSpace();
            //ClaimTrxs claimtrx = os.FindObject<ClaimTrxs>(new BinaryOperator("Oid", selectedObject.Oid));
            //genCon.openNewView(os, claimtrx, ViewEditMode.View);
            ((DetailView)View).ViewEditMode = ViewEditMode.View;
            //View.BreakLinksToControls();
            //View.CreateControls();
            RefreshController refreshcontroller = Frame.GetController<RefreshController>();
            if (refreshcontroller != null)
                refreshcontroller.RefreshAction.DoExecute();
            genCon.showMsg("Successful", "Closing Done.", InformationType.Success);
            //resetButton();

        }

        private void ReopenDoc_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (((DetailView)View).ViewEditMode == ViewEditMode.View)
                ((DetailView)View).ViewEditMode = ViewEditMode.Edit;

            ClaimTrxs selectedObject = (ClaimTrxs)e.CurrentObject;
            StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;

            selectedObject.IsClosed = false;
            ClaimTrxDocStatuses ds = ObjectSpace.CreateObject<ClaimTrxDocStatuses>();
            ds.DocStatus = DocumentStatus.Reopen;
            ds.DocRemarks = p.ParamString;
            selectedObject.ClaimTrxDocStatus.Add(ds);
            //selectedObject.OnPropertyChanged("ClaimTrxDocStatus");

            ModificationsController controller = Frame.GetController<ModificationsController>();
            if (controller != null)
            {
                if (!controller.SaveAction.DoExecute())
                {
                    genCon.showMsg("Failed", "Process failed. Please contact administrator", InformationType.Error);
                    return;
                }
            }

            //IObjectSpace os = Application.CreateObjectSpace();
            //ClaimTrxs claimtrx = os.FindObject<ClaimTrxs>(new BinaryOperator("Oid", selectedObject.Oid));
            //genCon.openNewView(os, claimtrx, ViewEditMode.View);
            ((DetailView)View).ViewEditMode = ViewEditMode.View;
            //View.BreakLinksToControls();
            //View.CreateControls();
            RefreshController refreshcontroller = Frame.GetController<RefreshController>();
            if (refreshcontroller != null)
                refreshcontroller.RefreshAction.DoExecute();
            genCon.showMsg("Successful", "Re-open Done.", InformationType.Success);
            //resetButton();

        }
        private void DuplicateDoc_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ClaimTrxs obj = (ClaimTrxs)e.CurrentObject;

            //SystemUsers user = ObjectSpace.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);

            IObjectSpace os = Application.CreateObjectSpace();
            ClaimTrxs claimtrx = os.CreateObject<ClaimTrxs>();
            genCon.duplicateclaim(ref claimtrx, obj, os);

            claimtrx.IsClaimUserCheck = true;
            if (user.Roles.Where(p => p.Name == GeneralSettings.claimsuperrole).Count() > 0)
                claimtrx.IsClaimSuperCheck = true;


            int cnt = 0;
            foreach (ClaimTrxItems dtl in obj.ClaimTrxItem)
            {
                cnt++;
                ClaimTrxItems claimtrxdtl = os.CreateObject<ClaimTrxItems>();
                genCon.duplicatedetailItem(ref claimtrxdtl, dtl, os);
                claimtrxdtl.Oid = cnt * -1;
                //claimtrxdtl.ClaimItem = os.GetObjectByKey<ClaimItems>(dtl.ClaimItem.Oid);
                //claimtrxdtl.DocDate = dtl.DocDate;
                //claimtrxdtl.RefNo = dtl.RefNo;
                //claimtrxdtl.Remarks = dtl.Remarks;
                //claimtrxdtl.Amount = dtl.Amount;
                //claimtrxdtl.TaxAmount = dtl.TaxAmount;
                claimtrx.ClaimTrxItem.Add(claimtrxdtl);
            }
            foreach (ClaimTrxMileages dtl in obj.ClaimTrxMileage)
            {
                cnt++;
                ClaimTrxMileages claimtrxdtl = os.CreateObject<ClaimTrxMileages>();
                genCon.duplicatedetailImileage(ref claimtrxdtl, dtl, os);
                claimtrxdtl.Oid = cnt * -1;
                //claimtrxdtlitem.Mileage = os.GetObjectByKey<Mileages>(dtl.Mileage.Oid);
                //claimtrxdtlitem.DocDate = dtl.DocDate;
                //claimtrxdtlitem.RefNo = dtl.RefNo;
                //claimtrxdtlitem.Remarks = dtl.Remarks;
                //claimtrxdtlitem.KM = dtl.KM;
                obj.ClaimTrxMileage.Add(claimtrxdtl);
            }

            ClaimTrxDetails ClaimTrxDetail = null;
            while (claimtrx.ClaimTrxDetail.Count > 0)
            {
                ClaimTrxDetail = claimtrx.ClaimTrxDetail[0];
                claimtrx.ClaimTrxDetail.Remove(ClaimTrxDetail);
            }
            foreach (ClaimTrxDetails dtl in obj.ClaimTrxDetail)
            {
                cnt++;
                ClaimTrxDetails claimtrxdtl = os.CreateObject<ClaimTrxDetails>();
                genCon.duplicatedetail(ref claimtrxdtl, dtl, os);
                claimtrxdtl.Oid = cnt * -1;
                //claimtrxdtl.ClaimType = os.GetObjectByKey<ClaimTypes>(dtl.ClaimType.Oid);
                //claimtrxdtl.DocDate = dtl.DocDate;
                //claimtrxdtl.RefNo = dtl.RefNo;
                //claimtrxdtl.Remarks = dtl.Remarks;
                //claimtrxdtl.Amount = dtl.Amount;
                //claimtrxdtl.TaxAmount = dtl.TaxAmount;
                //claimtrxdtl.Tax = os.GetObjectByKey<Taxes>(dtl.Tax.Oid);
                ////claimtrxdtl.RecordTotal = dtl.RecordTotal;

                foreach (ClaimTrxMileages dtlitem in dtl.ClaimTrxDetailMileage)
                {
                    cnt++;
                    ClaimTrxMileages claimtrxdtlitem = os.CreateObject<ClaimTrxMileages>();
                    genCon.duplicatedetailImileage(ref claimtrxdtlitem, dtlitem, os);
                    claimtrxdtlitem.Oid = cnt * -1;
                    //claimtrxdtlitem.Mileage = os.GetObjectByKey<Mileages>(dtlitem.Mileage.Oid);
                    //claimtrxdtlitem.DocDate = dtlitem.DocDate;
                    //claimtrxdtlitem.RefNo = dtlitem.RefNo;
                    //claimtrxdtlitem.Remarks = dtlitem.Remarks;
                    //claimtrxdtlitem.KM = dtlitem.KM;
                    claimtrxdtl.ClaimTrxDetailMileage.Add(claimtrxdtlitem);

                }
                foreach (ClaimTrxDetailNotes dtlitem in dtl.ClaimTrxDetailNote)
                {
                    cnt++;
                    ClaimTrxDetailNotes claimtrxdtlitem = os.CreateObject<ClaimTrxDetailNotes>();
                    genCon.duplicatedetailnote(ref claimtrxdtlitem, dtlitem, os);
                    claimtrxdtlitem.Oid = cnt * -1;

                    //claimtrxdtlitem.DocDate = dtlitem.DocDate;
                    //claimtrxdtlitem.RefNo = dtlitem.RefNo;
                    //claimtrxdtlitem.Remarks = dtlitem.Remarks;
                    //claimtrxdtlitem.Amount = dtlitem.Amount;
                    //claimtrxdtlitem.TaxAmount = dtlitem.TaxAmount;
                    //claimtrxdtlitem.Tax = os.GetObjectByKey<Taxes>(dtlitem.Tax.Oid);
                    claimtrxdtl.ClaimTrxDetailNote.Add(claimtrxdtlitem);

                }


                claimtrx.ClaimTrxDetail.Add(claimtrxdtl);
            }

            //os.CommitChanges();

            genCon.openNewView(os, claimtrx, ViewEditMode.Edit);
            genCon.showMsg("Successful", "Please save the New Claim before proceed to details.", InformationType.Success);

        }

        private void Approval_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            bool err = false;

            if (View is DetailView)
            {
                ClaimTrxs selectedObject = (ClaimTrxs)View.CurrentObject;
                //if (((DetailView)View).ViewEditMode == ViewEditMode.View)
                //{
                //    genCon.showMsg("Failed", "Viewing Document cannot proceed.", InformationType.Error);
                //    err = true;
                //}
                if (err)
                { }
                else
                {
                    if (!selectedObject.IsAccepted)
                    {
                        genCon.showMsg("Failed", "Document is not Accepted.", InformationType.Error);
                        err = true;
                    }
                    if (!selectedObject.IsApprovalUserCheck)
                    {
                        genCon.showMsg("Failed", "This is not a Approval User process.", InformationType.Error);
                        err = true;
                    }
                    if (selectedObject.IsClosed)
                    {
                        genCon.showMsg("Failed", "Document is Closed.", InformationType.Error);
                        err = true;
                    }
                    if (selectedObject.ApprovalStatus == ApprovalStatuses.Not_Applicable)
                    {
                        genCon.showMsg("Failed", "Document is not Approval Required.", InformationType.Error);
                        err = true;
                    }
                }
            }
            //SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<ApprovalParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;

            if (IsSimpleApproval)
            {
                if (e.Action.Id == this.ApprovalApprove.Id)
                {
                    ((ApprovalParameters)dv.CurrentObject).IsAppStatusHide = true;
                    ((ApprovalParameters)dv.CurrentObject).IsParamStringHide = true;
                    ((ApprovalParameters)dv.CurrentObject).AppStatus = ApprovalActions.Yes;
                    ((ApprovalParameters)dv.CurrentObject).IsErr = err;
                    ((ApprovalParameters)dv.CurrentObject).ActionMessage = "Press OK to Approve the Claim and SAVE, else press Cancel.";
                    dv.Caption = "Approve Claim";
                }
                else if (e.Action.Id == this.ApprovalReject.Id)
                {
                    ((ApprovalParameters)dv.CurrentObject).IsAppStatusHide = true;
                    ((ApprovalParameters)dv.CurrentObject).AppStatus = ApprovalActions.No;
                    ((ApprovalParameters)dv.CurrentObject).IsErr = err;
                    ((ApprovalParameters)dv.CurrentObject).ActionMessage = "Press OK to Reject the Claim and SAVE, else press Cancel.";
                    dv.Caption = "Reject Claim";
                }
                else if (e.Action.Id == this.ApproveSelected.Id)
                {
                    ((ApprovalParameters)dv.CurrentObject).IsAppStatusHide = true;
                    ((ApprovalParameters)dv.CurrentObject).IsParamStringHide = true;
                    ((ApprovalParameters)dv.CurrentObject).AppStatus = ApprovalActions.Yes;
                    ((ApprovalParameters)dv.CurrentObject).IsErr = err;
                    ((ApprovalParameters)dv.CurrentObject).ActionMessage = "Press OK to Approve Selected Claims, else press Cancel.";
                    dv.Caption = "Approve Claim";
                }
            }
            else
            {
                if (e.Action.Id == this.Approval.Id)
                {
                    ApprovalStatuses appstatus = ApprovalStatuses.Required_Approval;
                    if (View is DetailView)
                    {
                        ClaimTrxs selectedObject1 = (ClaimTrxs)View.CurrentObject;
                        if (selectedObject1.ClaimTrxAppStatus.Where(p => p.CreateUser.Oid == user.Oid).Count() > 0)
                            appstatus = selectedObject1.ClaimTrxAppStatus.Where(p => p.CreateUser.Oid == user.Oid).OrderBy(c => c.Oid).Last().AppStatus;
                    }
                    switch (appstatus)
                    {
                        case ApprovalStatuses.Required_Approval:
                            ((ApprovalParameters)dv.CurrentObject).AppStatus = ApprovalActions.NA;
                            break;
                        case ApprovalStatuses.Approved:
                            ((ApprovalParameters)dv.CurrentObject).AppStatus = ApprovalActions.Yes;
                            break;
                        case ApprovalStatuses.Rejected:
                            ((ApprovalParameters)dv.CurrentObject).AppStatus = ApprovalActions.No;
                            break;
                    }
                    ((ApprovalParameters)dv.CurrentObject).IsErr = err;
                    ((ApprovalParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";
                }
                else if (e.Action.Id == this.ApproveSelected.Id)
                {
                    ((ApprovalParameters)dv.CurrentObject).AppStatus = ApprovalActions.NA;
                    ((ApprovalParameters)dv.CurrentObject).IsErr = err;
                    ((ApprovalParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action of Selected Claims, else press Cancel.";
                }
            }
            e.View = dv;

        }
        private void ApproveSelected_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            ApprovalParameters p = (ApprovalParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;

            ApprovalStatuses appstatus = ApprovalStatuses.Required_Approval;
            if (p.AppStatus == ApprovalActions.NA)
            {
                appstatus = ApprovalStatuses.Required_Approval;
            }
            if (p.AppStatus == ApprovalActions.Yes)
            {
                appstatus = ApprovalStatuses.Approved;
            }
            if (p.AppStatus == ApprovalActions.No)
            {
                appstatus = ApprovalStatuses.Rejected;
            }
            ApprovalStatuses appstatus2 = ApprovalStatuses.Required_Approval;
            if (View is ListView)
            {
                foreach (ClaimTrxs selectedObject in View.SelectedObjects)
                {
                    if (selectedObject.ApprovalStatus == ApprovalStatuses.Required_Approval && !selectedObject.IsCancelled && !selectedObject.IsRejected && selectedObject.IsAccepted && !selectedObject.IsClosed && selectedObject.IsApprovalUserCheck)
                    {
                        if (selectedObject.ClaimTrxAppStatus.Where(x => x.CreateUser.Oid == user.Oid).Count() > 0)
                            appstatus2 = selectedObject.ClaimTrxAppStatus.Where(x => x.CreateUser.Oid == user.Oid).OrderBy(c => c.Oid).Last().AppStatus;

                        if (appstatus != appstatus2)
                        {
                            ClaimTrxAppStatuses ds = ObjectSpace.CreateObject<ClaimTrxAppStatuses>();
                            ds.AppStatus = appstatus;
                            ds.AppRemarks = p.ParamString;
                            if (!selectedObject.Company.IsConcurrentAppStage)
                                ds.ClaimTrxAppStage = ds.Session.GetObjectByKey<ClaimTrxAppStages>(selectedObject.CurrentAppStage.Oid);

                            selectedObject.ClaimTrxAppStatus.Add(ds);
                            ObjectSpace.CommitChanges();

                            genApproval(selectedObject, appstatus);
                        }
                    }
                }
                RefreshController refreshcontroller = Frame.GetController<RefreshController>();
                if (refreshcontroller != null)
                    refreshcontroller.RefreshAction.DoExecute();
                genCon.showMsg("Successful", "Approve Selected Claims Done.", InformationType.Success);
            }
        }
        public void genApproval(ClaimTrxs selectedObject, ApprovalStatuses appstatus)
        {
            #region approval
            XPObjectSpace persistentObjectSpace = (XPObjectSpace)Application.CreateObjectSpace();

            //List<string> ToEmails = new List<string>();
            string emailbody = "";
            string emailsubject = "";
            string emailaddress = "";
            Guid emailuser;
            DateTime emailtime = DateTime.Now;

            int curappstage = selectedObject.Company.IsConcurrentAppStage ? 0 : selectedObject.CurrentAppStage.Oid;

            SelectedData sprocData = persistentObjectSpace.Session.ExecuteSproc("sp_Approval", new OperandValue(user.UserName), new OperandValue(selectedObject.Oid), new OperandValue("ClaimTrxs"), new OperandValue((int)appstatus), new OperandValue(curappstage));

            IObjectSpace emailos = Application.CreateObjectSpace();
            EmailSents emailobj = null;
            int emailcnt = 0;

            if (sprocData.ResultSet.Count() > 0)
            {
                if (sprocData.ResultSet[0].Rows.Count() > 0)
                {
                    foreach (SelectStatementResultRow row in sprocData.ResultSet[0].Rows)
                    {
                        if (row.Values[1] != null)
                        {
                            emailcnt++;

                            emailbody = "<html><body>" + row.Values[3] + "<br/><a href=\"" + GeneralSettings.appurl + row.Values[2].ToString() + "\">Link</a></body></html>";

                            if (appstatus == ApprovalStatuses.Approved)
                                emailsubject = "Claim Document Approval Completed";
                            else if (appstatus == ApprovalStatuses.Rejected)
                                emailsubject = "Claim Document Approval Rejected";

                            emailaddress = row.Values[1].ToString();
                            emailuser = (Guid)row.Values[0];

                            if (emailcnt == 1)
                            {
                                emailobj = emailos.CreateObject<EmailSents>();
                                emailobj.CreateDate = (DateTime?)emailtime;
                                //assign body will get error???
                                emailobj.EmailBody = emailbody;
                                emailobj.EmailSubject = emailsubject;
                                emailobj.ClaimTrx = emailobj.Session.GetObjectByKey<ClaimTrxs>(selectedObject.Oid);
                            }
                            EmailSentDetails emaildtl = emailos.CreateObject<EmailSentDetails>();
                            emaildtl.EmailUser = emaildtl.Session.GetObjectByKey<SystemUsers>(emailuser);
                            emaildtl.EmailAddress = emailaddress;
                            emailobj.EmailSentDetail.Add(emaildtl);
                        }
                    }
                    //emailobj.Save();
                    if (emailcnt > 0)
                        emailos.CommitChanges();
                }
            }
            persistentObjectSpace.Session.DropIdentityMap();
            persistentObjectSpace.Dispose();
            sprocData = null;

            if (emailobj != null)
            {
                genCon.SendEmail_By_Object(emailobj);
            }

            //if (ToEmails.Count > 0)
            //{
            //    if (genCon.SendEmail(emailsubject, emailbody, ToEmails) == 1)
            //    {
            //        if (emailos.IsModified)
            //            emailos.CommitChanges();
            //    }
            //}
            #endregion

            IObjectSpace os = Application.CreateObjectSpace();
            ClaimTrxs claimtrx = os.FindObject<ClaimTrxs>(new BinaryOperator("Oid", selectedObject.Oid));
            int CurrentApprovalStageOid = claimtrx.GetCurrentApprovalStageOid();
            if (CurrentApprovalStageOid > 0)
                claimtrx.CurrentAppStage = claimtrx.Session.GetObjectByKey<ClaimTrxAppStages>(CurrentApprovalStageOid);
            else
                claimtrx.CurrentAppStage = null;
            os.CommitChanges();

        }
        private void Approval_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (((DetailView)View).ViewEditMode == ViewEditMode.View)
                ((DetailView)View).ViewEditMode = ViewEditMode.Edit;

            ApprovalParameters p = (ApprovalParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;
            ApprovalStatuses appstatus = ApprovalStatuses.Required_Approval;
            if (p.AppStatus == ApprovalActions.NA)
            {
                appstatus = ApprovalStatuses.Required_Approval;
            }
            if (p.AppStatus == ApprovalActions.Yes)
            {
                appstatus = ApprovalStatuses.Approved;
            }
            if (p.AppStatus == ApprovalActions.No)
            {
                appstatus = ApprovalStatuses.Rejected;
            }

            ClaimTrxs selectedObject = (ClaimTrxs)e.CurrentObject;
            //SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
            ApprovalStatuses appstatus2 = ApprovalStatuses.Required_Approval;

            if (selectedObject.ClaimTrxAppStatus.Where(x => x.CreateUser.Oid == user.Oid && x.ClaimTrxAppStage.Oid == x.ClaimTrx.CurrentAppStage.Oid).Count() > 0)
                appstatus2 = selectedObject.ClaimTrxAppStatus.Where(x => x.CreateUser.Oid == user.Oid && x.ClaimTrxAppStage.Oid == x.ClaimTrx.CurrentAppStage.Oid).OrderBy(c => c.Oid).Last().AppStatus;

            if (appstatus == appstatus2)
            {
                genCon.showMsg("Failed", "Same Approval Status is not allowed.", InformationType.Error);
                return;
            }
            //if (appstatus == ApprovalStatuses.Required_Approval && p.AppStatus == ApprovalActions.NA)
            //{
            //    genCon.showMsg("Failed", "Same Approval Status is not allowed.", InformationType.Error);
            //    return;
            //}
            //else if (appstatus == ApprovalStatuses.Approved && p.AppStatus == ApprovalActions.Yes)
            //{
            //    genCon.showMsg("Failed", "Same Approval Status is not allowed.", InformationType.Error);
            //    return;
            //}
            //else if (appstatus == ApprovalStatuses.Rejected && p.AppStatus == ApprovalActions.No)
            //{
            //    genCon.showMsg("Failed", "Same Approval Status is not allowed.", InformationType.Error);
            //    return;
            //}

            ClaimTrxAppStatuses ds = ObjectSpace.CreateObject<ClaimTrxAppStatuses>();
            ds.AppStatus = appstatus;
            ds.AppRemarks = p.ParamString;
            if (!selectedObject.Company.IsConcurrentAppStage)
                ds.ClaimTrxAppStage = ds.Session.GetObjectByKey<ClaimTrxAppStages>(selectedObject.CurrentAppStage.Oid);

            selectedObject.ClaimTrxAppStatus.Add(ds);
            //selectedObject.OnPropertyChanged("ClaimTrxDocStatus");

            ModificationsController controller = Frame.GetController<ModificationsController>();
            if (controller != null)
            {
                if (!controller.SaveAction.DoExecute())
                {
                    genCon.showMsg("Failed", "Process failed. Please contact administrator", InformationType.Error);
                    return;
                }
            }

            genApproval(selectedObject, appstatus);

            ((DetailView)View).ViewEditMode = ViewEditMode.View;
            //View.BreakLinksToControls();
            //View.CreateControls();
            RefreshController refreshcontroller = Frame.GetController<RefreshController>();
            if (refreshcontroller != null)
                refreshcontroller.RefreshAction.DoExecute();
            genCon.showMsg("Successful", "Approval Done.", InformationType.Success);
            //resetButton();

        }

        private void SetPaidDate_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<DateParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((DateParameters)dv.CurrentObject).IsErr = false;
            ((DateParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action, else press Cancel.";

            e.View = dv;
        }

        private void SetPaidDate_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            ClaimTrxs selectedObject = (ClaimTrxs)e.CurrentObject;
            //SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;

            DateParameters p = (DateParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;

            selectedObject.PaidDate = p.ParamDate;
            ObjectSpace.CommitChanges();
            RefreshController refreshcontroller = Frame.GetController<RefreshController>();
            if (refreshcontroller != null)
                refreshcontroller.RefreshAction.DoExecute();
            genCon.showMsg("Successful", "Paid Date Updated.", InformationType.Success);

        }

    }
}
