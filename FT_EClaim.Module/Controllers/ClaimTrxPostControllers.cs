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
    public partial class ClaimTrxPostControllers : ViewController
    {
        GenControllers genCon;
        RecordsNavigationController recordnaviator;
        DateTime? _postdate;
        public ClaimTrxPostControllers()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
            TargetObjectType = typeof(ClaimTrxs);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            resetButton();

            if (View is DetailView)
            {
                ((DetailView)View).ViewEditModeChanged += ClaimTrxPostControllers_ViewEditModeChanged;

                recordnaviator = Frame.GetController<RecordsNavigationController>();
                if (recordnaviator != null)
                {
                    recordnaviator.PreviousObjectAction.Executed += PreviousObjectAction_Executed;
                    recordnaviator.NextObjectAction.Executed += NextObjectAction_Executed;
                }
            }
            
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
        private void ClaimTrxPostControllers_ViewEditModeChanged(object sender, EventArgs e)
        {
            resetButton();
        }
        private void resetButton()
        {
            SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
            bool IsPostUser = user.Roles.Where(p => p.Name == GeneralSettings.postrole).Count() > 0 ? true : false;
            this.PostClaim.Active.SetItemValue("Enabled", false);
            this.PostClaimV2.Active.SetItemValue("Enabled", false);
            if (IsPostUser)
            {
                if (View.GetType() == typeof(DetailView))
                {
                    ClaimTrxs selectedObject = (ClaimTrxs)View.CurrentObject;
                    if (selectedObject.IsNew)
                    {
                    }
                    else
                    {
                        if (selectedObject.IsClosed && !selectedObject.IsPosted)
                        {
                            this.PostClaimV2.Active.SetItemValue("Enabled", true);
                        }
                    }
                    this.PostClaimV2.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.View);
                }
                else if (View is ListView)
                {
                    //if (View.Id == "ClaimTrxs_ListView_Closed")
                        this.PostClaimV2.Active.SetItemValue("Enabled", true);
                }
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
                ((DetailView)View).ViewEditModeChanged -= ClaimTrxPostControllers_ViewEditModeChanged;
                if (recordnaviator != null)
                {
                    recordnaviator.PreviousObjectAction.Executed -= PreviousObjectAction_Executed;
                    recordnaviator.NextObjectAction.Executed -= NextObjectAction_Executed;
                }
            }
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void PostClaim_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (GeneralSettings.B1Post && genCon.ConnectSAP())
            {
                IObjectSpace ios = Application.CreateObjectSpace();
                if (View is DetailView)
                {
                    ClaimTrxs selectedObject = (ClaimTrxs)View.CurrentObject;
                    if (GeneralSettings.B1Post)
                        GeneralSettings.oCompany.StartTransaction();
                    if (PostToSAP(selectedObject, ios) == -1)
                    {
                        if (GeneralSettings.B1Post)
                            if (GeneralSettings.oCompany.InTransaction)
                                GeneralSettings.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                        ios.Rollback(false);
                    }
                    if (GeneralSettings.B1Post)
                        if (GeneralSettings.oCompany.InTransaction)
                            GeneralSettings.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                    ios.CommitChanges();

                }
                else if (View is ListView)
                {
                    if (e.SelectedObjects.Count > 0)
                    {
                        //IObjectSpace os;
                        //os = Application.CreateObjectSpace();
                        //ClaimTrxs obj = os.CreateObject<ClaimTrxs>();

                        if (GeneralSettings.B1Post)
                            GeneralSettings.oCompany.StartTransaction();
                        foreach (ClaimTrxs selectedObject in e.SelectedObjects)
                        {
                            if (PostToSAP(selectedObject, ios) == -1)
                            {
                                if (GeneralSettings.B1Post)
                                    if (GeneralSettings.oCompany.InTransaction)
                                        GeneralSettings.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                                ios.Rollback(false);
                                break;
                            }
                        }
                        if (GeneralSettings.B1Post)
                            if (GeneralSettings.oCompany.InTransaction)
                                GeneralSettings.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                        ios.CommitChanges();
                    }
                }

                RefreshController refreshController = Frame.GetController<RefreshController>();
                if (refreshController != null)
                {
                    refreshController.RefreshAction.DoExecute();
                }
                genCon.showMsg("Successful", "Post to SAP B1 Done.", InformationType.Success);

            }
            else
            {

            }
        }

        private int PostToSAP(ClaimTrxs selectedObject, IObjectSpace ios)
        {
            int temp = 0;

            ClaimTrxs iobj = ios.GetObjectByKey<ClaimTrxs>(selectedObject.Oid);
            Companies Company = ios.FindObject<Companies>(new BinaryOperator("Oid", selectedObject.Company.Oid, BinaryOperatorType.Equal));

            //ShowViewParameters svp = new ShowViewParameters();
            //DetailView dv = Application.CreateDetailView(ios, iobj);
            //dv.ViewEditMode = ViewEditMode.View;
            //svp.CreatedView = dv;

            if (iobj.IsClosed && !iobj.IsPosted)
            {
                iobj.PaidDate = (DateTime)_postdate;
                if (GeneralSettings.B1Post)
                    if (Company.PostToDocument == PostToDocuments.JE)
                    {
                        if (Company.EClaimSAPDoc == EClaimSAPDocs.Document)
                            temp = genCon.PostJEtoSAP(iobj);
                        else if (Company.EClaimSAPDoc == EClaimSAPDocs.Draft)
                            temp = genCon.PostJVtoSAP(iobj);
                    }
                    else if (Company.PostToDocument == PostToDocuments.APINV)
                        temp = genCon.PostAPIVtoSAP(iobj);
                else
                    temp = genCon.DelayPostToSAP(iobj);

                if (temp > 0)
                {
                    if (GeneralSettings.B1Post)
                        iobj.IsSAPPosted = true;

                    iobj.SAPKey = temp;
                    iobj.IsClosed = false;
                    iobj.IsPosted = true;
                    ClaimTrxDocStatuses ds = ios.CreateObject<ClaimTrxDocStatuses>();
                    ds.DocStatus = DocumentStatus.Posted;
                    ds.DocRemarks = "";
                    iobj.ClaimTrxDocStatus.Add(ds);
                    //iobj.OnPropertyChanged("ClaimTrxDocStatus");

                }
                else if (temp == 0)
                {
                }
                else if (temp == -1)
                {
                }
            }

            return temp;
        }

        private void PostClaimV2_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<DateParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((DateParameters)dv.CurrentObject).IsErr = false;
            ((DateParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action, else press Cancel.";

            e.View = dv;
        }

        private void PostClaimV2_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            DateParameters p = (DateParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;

            _postdate = p.ParamDate;
            if (_postdate is null)
            {
                genCon.showMsg("Cannot post", "Posting Date is not valid.", InformationType.Error);
                return;
            }
            if (GeneralSettings.B1Post && genCon.ConnectSAP())
            {
                IObjectSpace ios = Application.CreateObjectSpace();
                if (View is DetailView)
                {
                    ClaimTrxs selectedObject = (ClaimTrxs)View.CurrentObject;
                    if (GeneralSettings.B1Post)
                        GeneralSettings.oCompany.StartTransaction();
                    if (PostToSAP(selectedObject, ios) == -1)
                    {
                        if (GeneralSettings.B1Post)
                            if (GeneralSettings.oCompany.InTransaction)
                                GeneralSettings.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                        ios.Rollback(false);
                    }
                    if (GeneralSettings.B1Post)
                        if (GeneralSettings.oCompany.InTransaction)
                            GeneralSettings.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                    ios.CommitChanges();

                }
                else if (View is ListView)
                {
                    if (e.SelectedObjects.Count > 0)
                    {
                        //IObjectSpace os;
                        //os = Application.CreateObjectSpace();
                        //ClaimTrxs obj = os.CreateObject<ClaimTrxs>();

                        if (GeneralSettings.B1Post)
                            GeneralSettings.oCompany.StartTransaction();
                        foreach (ClaimTrxs selectedObject in e.SelectedObjects)
                        {
                            if (PostToSAP(selectedObject, ios) == -1)
                            {
                                if (GeneralSettings.B1Post)
                                    if (GeneralSettings.oCompany.InTransaction)
                                        GeneralSettings.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                                ios.Rollback(false);
                                break;
                            }
                        }
                        if (GeneralSettings.B1Post)
                            if (GeneralSettings.oCompany.InTransaction)
                                GeneralSettings.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                        ios.CommitChanges();
                    }
                }

                RefreshController refreshController = Frame.GetController<RefreshController>();
                if (refreshController != null)
                {
                    refreshController.RefreshAction.DoExecute();
                }
                genCon.showMsg("Successful", "Post to SAP B1 Done.", InformationType.Success);

            }
            else
            {

            }
        }
    }
}
