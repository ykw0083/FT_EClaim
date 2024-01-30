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
using SAPbobsCOM;
using System.Net.Mail;
using System.Net;
using FT_EClaim.Module.BusinessObjects;

namespace FT_EClaim.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class GenControllers : ViewController
    {
        public GenControllers()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
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
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
        public void openNewView(IObjectSpace os, object target, ViewEditMode viewmode)
        {
            ShowViewParameters svp = new ShowViewParameters();
            DetailView dv = Application.CreateDetailView(os, target);
            dv.ViewEditMode = viewmode;
            dv.IsRoot = true;
            svp.CreatedView = dv;

            Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));

        }
        public void showMsg(string caption, string msg, InformationType msgtype)
        {
            MessageOptions options = new MessageOptions();
            options.Duration = 3000;
            //options.Message = string.Format("{0} task(s) have been successfully updated!", e.SelectedObjects.Count);
            options.Message = string.Format("{0}", msg);
            options.Type = msgtype;
            options.Web.Position = InformationPosition.Right;
            options.Win.Caption = caption;
            options.Win.Type = WinMessageType.Flyout;
            Application.ShowViewStrategy.ShowMessage(options);

        }
        public bool ConnectSAP()
        {
            if (GeneralSettings.B1Post)
            {
                FT_EClaim.Module.BusinessObjects.SystemUsers user = (FT_EClaim.Module.BusinessObjects.SystemUsers)SecuritySystem.CurrentUser;
                if (string.IsNullOrEmpty(user.SAPUserID))
                {
                    showMsg("Failed", "SAP User ID is empty", InformationType.Error);
                    return false;
                }

                if (GeneralSettings.oCompany == null)
                {
                    GeneralSettings.oCompany = new SAPbobsCOM.Company();
                }

                if (GeneralSettings.oCompany != null && !GeneralSettings.oCompany.Connected)
                {
                    bool exist = Enum.IsDefined(typeof(SAPbobsCOM.BoDataServerTypes), GeneralSettings.B1DbServerType);
                    if (exist)
                        GeneralSettings.oCompany.DbServerType = (SAPbobsCOM.BoDataServerTypes)Enum.Parse(typeof(SAPbobsCOM.BoDataServerTypes), GeneralSettings.B1DbServerType);
                    else
                        GeneralSettings.oCompany.DbServerType = BoDataServerTypes.dst_MSSQL2019;

                    exist = Enum.IsDefined(typeof(SAPbobsCOM.BoSuppLangs), GeneralSettings.B1Language);
                    if (exist)
                        GeneralSettings.oCompany.language = (SAPbobsCOM.BoSuppLangs)Enum.Parse(typeof(SAPbobsCOM.BoSuppLangs), GeneralSettings.B1Language);
                    else
                        GeneralSettings.oCompany.language = BoSuppLangs.ln_English;

                    GeneralSettings.oCompany.Server = GeneralSettings.B1Server;
                    GeneralSettings.oCompany.CompanyDB = GeneralSettings.B1CompanyDB;
                    if (!string.IsNullOrEmpty(GeneralSettings.B1License))
                        GeneralSettings.oCompany.LicenseServer = GeneralSettings.B1License;
                    if (!string.IsNullOrEmpty(GeneralSettings.SLDServer))
                        GeneralSettings.oCompany.SLDServer = GeneralSettings.SLDServer;
                    GeneralSettings.oCompany.DbUserName = GeneralSettings.B1DbUserName;
                    GeneralSettings.oCompany.DbPassword = GeneralSettings.B1DbPassword;
                    GeneralSettings.oCompany.UserName = user.SAPUserID;
                    if (!string.IsNullOrEmpty(user.SAPPassword))
                        GeneralSettings.oCompany.Password = user.SAPPassword;
                    if (GeneralSettings.oCompany.Connect() != 0)
                    {
                        showMsg("Failed", GeneralSettings.oCompany.GetLastErrorDescription(), InformationType.Error);
                    }
                }
                else if (GeneralSettings.oCompany != null && GeneralSettings.oCompany.Connected)
                {
                    if (GeneralSettings.oCompany.UserName != user.SAPUserID)
                    {
                        GeneralSettings.oCompany.Disconnect();

                        bool exist = Enum.IsDefined(typeof(SAPbobsCOM.BoDataServerTypes), GeneralSettings.B1DbServerType);
                        if (exist)
                            GeneralSettings.oCompany.DbServerType = (SAPbobsCOM.BoDataServerTypes)Enum.Parse(typeof(SAPbobsCOM.BoDataServerTypes), GeneralSettings.B1DbServerType);
                        else
                            GeneralSettings.oCompany.DbServerType = BoDataServerTypes.dst_MSSQL2019;

                        exist = Enum.IsDefined(typeof(SAPbobsCOM.BoSuppLangs), GeneralSettings.B1Language);
                        if (exist)
                            GeneralSettings.oCompany.language = (SAPbobsCOM.BoSuppLangs)Enum.Parse(typeof(SAPbobsCOM.BoSuppLangs), GeneralSettings.B1Language);
                        else
                            GeneralSettings.oCompany.language = BoSuppLangs.ln_English;

                        GeneralSettings.oCompany.Server = GeneralSettings.B1Server;
                        GeneralSettings.oCompany.CompanyDB = GeneralSettings.B1CompanyDB;
                        GeneralSettings.oCompany.LicenseServer = GeneralSettings.B1License;
                        GeneralSettings.oCompany.DbUserName = GeneralSettings.B1DbUserName;
                        GeneralSettings.oCompany.DbPassword = GeneralSettings.B1DbPassword;
                        GeneralSettings.oCompany.UserName = user.SAPUserID;
                        if (!string.IsNullOrEmpty(user.SAPPassword))
                            GeneralSettings.oCompany.Password = user.SAPPassword;
                        if (GeneralSettings.oCompany.Connect() != 0)
                        {
                            showMsg("Failed", GeneralSettings.oCompany.GetLastErrorDescription(), InformationType.Error);
                        }
                    }
                }
                return GeneralSettings.oCompany.Connected;
            }
            else
            {
                return false;
            }
        }
        public int DelayPostToSAP(FT_EClaim.Module.BusinessObjects.ClaimTrxs oTargetDoc)
        {
            // return 0 = post nothing
            // return -1 = posting error
            // return > 0 = posting successful
            if (oTargetDoc.IsSAPPosted) return 0;

            if (oTargetDoc.IsClosed && !oTargetDoc.IsPosted)
            {
                return 1;
            }
            return 0;
        }
        public int PostAPIVtoSAP(FT_EClaim.Module.BusinessObjects.ClaimTrxs oTargetDoc)
        {
            // return 0 = post nothing
            // return -1 = posting error
            // return > 0 = posting successful
            try
            {
                if (oTargetDoc.IsSAPPosted) return 0;
                string temp = "";

                if (oTargetDoc.IsClosed && !oTargetDoc.IsPosted)
                {
                    bool found = false;
                    foreach (FT_EClaim.Module.BusinessObjects.ClaimTrxPostDetails dtl in oTargetDoc.ClaimTrxPostDetail)
                    {
                        if (dtl.Amount > 0)
                        {
                            found = true;
                        }
                    }
                    if (!found) return 0;

                    Guid g;
                    // Create and display the value of two GUIDs.
                    g = Guid.NewGuid();

                    if (oTargetDoc.ClaimTrxAttachment != null && oTargetDoc.ClaimTrxAttachment.Count > 0)
                    {
                        foreach (FT_EClaim.Module.BusinessObjects.ClaimTrxAttachments obj in oTargetDoc.ClaimTrxAttachment)
                        {
                            string fullpath = GeneralSettings.B1AttachmentPath + g.ToString() + obj.File.FileName;
                            using (System.IO.FileStream fs = System.IO.File.OpenWrite(fullpath))
                            {
                                obj.File.SaveToStream(fs);
                            }
                        }
                    }

                    int sapempid = 0;
                    //DateTime reqdate = DateTime.Today;
                    //foreach (PMSClaimEF.Module.BusinessObjects.ClaimTrxDocStatuses dtl in oTargetDoc.ClaimTrxDocStatus)
                    //{
                    //    if (dtl.DocStatus == BusinessObjects.DocumentStatus.DocPassed)
                    //    {
                    //        reqdate = (DateTime)dtl.CreateDate;
                    //    }
                    //}


                    SAPbobsCOM.Documents oDoc = null;
                    if (oTargetDoc.Company.EClaimSAPDoc == BusinessObjects.EClaimSAPDocs.Draft)
                    {
                        oDoc = (SAPbobsCOM.Documents)GeneralSettings.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                        oDoc.DocObjectCode = SAPbobsCOM.BoObjectTypes.oPurchaseInvoices;
                    }
                    else if (oTargetDoc.Company.EClaimSAPDoc == BusinessObjects.EClaimSAPDocs.Document)
                    {
                        oDoc = (SAPbobsCOM.Documents)GeneralSettings.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices);
                    }
                    oDoc.DocType = BoDocumentTypes.dDocument_Service;
                    oDoc.CardCode = oTargetDoc.Employee.BoCode;
                    //oDoc.UserFields.Fields.Item(GeneralSettings.B1PRCol).Value = "Y";
                    //oDoc.UserFields.Fields.Item(GeneralSettings.B1PRNoCol).Value = oTargetDoc.DocNum;
                    //oDoc.UserFields.Fields.Item(GeneralSettings.B1WONoCol).Value = oTargetDoc.WorkOrder.DocNum;
                    if (sapempid > 0)
                        oDoc.DocumentsOwner = sapempid;
                    oDoc.DocDate = (DateTime)oTargetDoc.PaidDate;
                    oDoc.TaxDate = oTargetDoc.DocDate;

                    if (oTargetDoc.Company.ClaimSeriesNo > 0)
                        oDoc.Series = oTargetDoc.Company.ClaimSeriesNo;

                    int cnt = 0;
                    string acctcode = "";
                    string formatcode = "";
                    foreach (FT_EClaim.Module.BusinessObjects.ClaimTrxPostDetails dtl in oTargetDoc.ClaimTrxPostDetail)
                    {
                        if (dtl.Amount > 0)
                        {
                            cnt++;
                            if (cnt == 1)
                            {
                            }
                            else
                            {
                                oDoc.Lines.Add();
                                oDoc.Lines.SetCurrentLine(oDoc.Lines.Count - 1);
                            }
                            oDoc.Lines.ItemDescription = dtl.Remarks;
                            oDoc.Lines.Quantity = 1;
                            oDoc.Lines.TaxCode = dtl.Tax.BoCode;
                            //oDoc.Lines.Price = (double)dtl.Amount;
                            //oDoc.Lines.GrossPrice = (double)dtl.ItemAmount;
                            oDoc.Lines.UnitPrice = (double)dtl.Amount;
                            oDoc.Lines.TaxTotal = (double)dtl.TaxAmount;

                            if (!string.IsNullOrEmpty(dtl.AccountCode))
                                formatcode = dtl.AccountCode;

                            BusinessObjects.Accounts OACT = ObjectSpace.FindObject<BusinessObjects.Accounts>(new BinaryOperator("BoCode", formatcode, BinaryOperatorType.Equal));
                            //if (OACT == null)
                            //{
                            //    if (GeneralSettings.oCompany.InTransaction)
                            //    {
                            //        GeneralSettings.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                            //    }
                            //    showMsg("Failed", formatcode + " is not found in Chart of Account.", InformationType.Error);
                            //    return -1;
                            //}
                            //else
                            acctcode = OACT.SystemCode;

                            oDoc.Lines.AccountCode = acctcode;

                            if (!string.IsNullOrEmpty(dtl.DepartmentCode))
                            {
                                if (GeneralSettings.B1DeptDimension == 1)
                                    oDoc.Lines.CostingCode = dtl.DepartmentCode;
                                else if (GeneralSettings.B1DeptDimension == 2)
                                    oDoc.Lines.CostingCode2 = dtl.DepartmentCode;
                                else if (GeneralSettings.B1DeptDimension == 3)
                                    oDoc.Lines.CostingCode3 = dtl.DepartmentCode;
                                else if (GeneralSettings.B1DeptDimension == 4)
                                    oDoc.Lines.CostingCode4 = dtl.DepartmentCode;
                                else if (GeneralSettings.B1DeptDimension == 5)
                                    oDoc.Lines.CostingCode5 = dtl.DepartmentCode;
                            }
                            else if (dtl.Department != null)
                            {
                                if (GeneralSettings.B1DeptDimension == 1)
                                    oDoc.Lines.CostingCode = dtl.Department.PostCode;
                                else if (GeneralSettings.B1DeptDimension == 2)
                                    oDoc.Lines.CostingCode2 = dtl.Department.PostCode;
                                else if (GeneralSettings.B1DeptDimension == 3)
                                    oDoc.Lines.CostingCode3 = dtl.Department.PostCode;
                                else if (GeneralSettings.B1DeptDimension == 4)
                                    oDoc.Lines.CostingCode4 = dtl.Department.PostCode;
                                else if (GeneralSettings.B1DeptDimension == 5)
                                    oDoc.Lines.CostingCode5 = dtl.Department.PostCode;
                            }
                            if (!string.IsNullOrEmpty(dtl.DivisionCode))
                            {
                                if (GeneralSettings.B1DivDimension == 1)
                                    oDoc.Lines.CostingCode = dtl.DivisionCode;
                                else if (GeneralSettings.B1DivDimension == 2)
                                    oDoc.Lines.CostingCode2 = dtl.DivisionCode;
                                else if (GeneralSettings.B1DivDimension == 3)
                                    oDoc.Lines.CostingCode3 = dtl.DivisionCode;
                                else if (GeneralSettings.B1DivDimension == 4)
                                    oDoc.Lines.CostingCode4 = dtl.DivisionCode;
                                else if (GeneralSettings.B1DivDimension == 5)
                                    oDoc.Lines.CostingCode5 = dtl.DivisionCode;
                            }
                            else if (dtl.Division != null)
                            {
                                if (GeneralSettings.B1DivDimension == 1)
                                    oDoc.Lines.CostingCode = dtl.Division.PostCode;
                                else if (GeneralSettings.B1DivDimension == 2)
                                    oDoc.Lines.CostingCode2 = dtl.Division.PostCode;
                                else if (GeneralSettings.B1DivDimension == 3)
                                    oDoc.Lines.CostingCode3 = dtl.Division.PostCode;
                                else if (GeneralSettings.B1DivDimension == 4)
                                    oDoc.Lines.CostingCode4 = dtl.Division.PostCode;
                                else if (GeneralSettings.B1DivDimension == 5)
                                    oDoc.Lines.CostingCode5 = dtl.Division.PostCode;
                            }

                            if (!string.IsNullOrEmpty(dtl.ProjectCode))
                                oDoc.Lines.ProjectCode = dtl.ProjectCode;
                            else if (dtl.Project != null)
                                oDoc.Lines.ProjectCode = dtl.Project.BoCode;

                            //if (!string.IsNullOrEmpty(dtl.RefNo))
                            //    if (!string.IsNullOrEmpty(GeneralSettings.B1RefCol))
                            //        oDoc.Lines.UserFields.Fields.Item(GeneralSettings.B1RefCol).Value = dtl.Oid;

                        }
                    }
                    if (oTargetDoc.ClaimTrxAttachment != null && oTargetDoc.ClaimTrxAttachment.Count > 0)
                    {
                        cnt = 0;
                        SAPbobsCOM.Attachments2 oAtt = (SAPbobsCOM.Attachments2)GeneralSettings.oCompany.GetBusinessObject(BoObjectTypes.oAttachments2);
                        foreach (FT_EClaim.Module.BusinessObjects.ClaimTrxAttachments dtl in oTargetDoc.ClaimTrxAttachment)
                        {

                            cnt++;
                            if (cnt == 1)
                            {
                                if (oAtt.Lines.Count == 0)
                                    oAtt.Lines.Add();
                            }
                            else
                                oAtt.Lines.Add();

                            string attfile = "";
                            string[] fexe = dtl.File.FileName.Split('.');
                            if (fexe.Length <= 2)
                                attfile = fexe[0];
                            else
                            {
                                for (int x = 0; x < fexe.Length - 1; x++)
                                {
                                    if (attfile == "")
                                        attfile = fexe[x];
                                    else
                                        attfile += "." + fexe[x];
                                }
                            }
                            oAtt.Lines.FileName = g.ToString() + attfile;
                            if (fexe.Length > 1)
                                oAtt.Lines.FileExtension = fexe[fexe.Length - 1];
                            string path = GeneralSettings.B1AttachmentPath;
                            path = path.Replace("\\\\", "\\");
                            path = path.Substring(0, path.Length - 1);
                            oAtt.Lines.SourcePath = path;
                            oAtt.Lines.Override = SAPbobsCOM.BoYesNoEnum.tYES;
                        }
                        int iAttEntry = -1;
                        if (oAtt.Add() == 0)
                        {
                            iAttEntry = int.Parse(GeneralSettings.oCompany.GetNewObjectKey());
                        }
                        else
                        {
                            temp = GeneralSettings.oCompany.GetLastErrorDescription();
                            if (GeneralSettings.oCompany.InTransaction)
                            {
                                GeneralSettings.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                            }
                            showMsg("Failed", temp, InformationType.Error);
                            return -1;
                        }
                        oDoc.AttachmentEntry = iAttEntry;
                    }

                    int rc = oDoc.Add();
                    if (rc != 0)
                    {
                        temp = GeneralSettings.oCompany.GetLastErrorDescription();
                        if (GeneralSettings.oCompany.InTransaction)
                        {
                            GeneralSettings.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                        }
                        showMsg("Failed", temp, InformationType.Error);
                        return -1;
                    }
                    #region check post data exist
                    string docentry = "";
                    GeneralSettings.oCompany.GetNewObjectCode(out docentry);
                    if (docentry == "")
                    {
                        temp = "Unknown Error! Please try again!";
                        if (GeneralSettings.oCompany.InTransaction)
                        {
                            GeneralSettings.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                        }
                        showMsg("Failed", temp, InformationType.Error);
                        return -1;
                    }

                    SAPbobsCOM.JournalEntries getDoc = (SAPbobsCOM.JournalEntries)GeneralSettings.oCompany.GetBusinessObject(BoObjectTypes.oJournalEntries);

                    if (!getDoc.GetByKey(int.Parse(docentry)))
                    {
                        temp = "Unknown Error! Please try again!";
                        if (GeneralSettings.oCompany.InTransaction)
                        {
                            GeneralSettings.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                        }
                        oDoc = null;
                        return -1;
                    }
                    #endregion

                    return int.Parse(docentry);
                }
                return 0;
            }
            catch (Exception ex)
            {
                showMsg("Error", ex.Message, InformationType.Error);
                return -1;
            }

        }
        public int PostJVtoSAP(FT_EClaim.Module.BusinessObjects.ClaimTrxs oTargetDoc)
        {
            showMsg("Error", "JV not allowed", InformationType.Error);
            return -1;
        }
        public int PostJEtoSAP(FT_EClaim.Module.BusinessObjects.ClaimTrxs oTargetDoc)
        {
            // return 0 = post nothing
            // return -1 = posting error
            // return > 0 = posting successful
            try
            {
                string temp = "";
                string remarks = "";
                if (oTargetDoc.IsSAPPosted) return 0;

                if (oTargetDoc.IsClosed && !oTargetDoc.IsPosted)
                {
                    bool found = false;
                    foreach (FT_EClaim.Module.BusinessObjects.ClaimTrxPostDetails dtl in oTargetDoc.ClaimTrxPostDetail)
                    {
                        if (dtl.Amount != 0)
                        {
                            found = true;
                        }
                    }
                    if (!found) return 0;


                    //int sapempid = 0;
                    //DateTime reqdate = DateTime.Today;
                    //foreach (PMSClaimEF.Module.BusinessObjects.ClaimTrxDocStatuses dtl in oTargetDoc.ClaimTrxDocStatus)
                    //{
                    //    if (dtl.DocStatus == BusinessObjects.DocumentStatus.DocPassed)
                    //    {
                    //        reqdate = (DateTime)dtl.CreateDate;
                    //    }
                    //}

                    string jememo = oTargetDoc.getJERemarks();

                    SAPbobsCOM.JournalEntries oDoc = null;
                    oDoc = (SAPbobsCOM.JournalEntries)GeneralSettings.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries);

                    //oDoc.UserFields.Fields.Item(GeneralSettings.B1PRCol).Value = "Y";
                    //oDoc.UserFields.Fields.Item(GeneralSettings.B1PRNoCol).Value = oTargetDoc.DocNum;
                    //oDoc.UserFields.Fields.Item(GeneralSettings.B1WONoCol).Value = oTargetDoc.WorkOrder.DocNum;
                    oDoc.TaxDate = oTargetDoc.DocDate;
                    oDoc.ReferenceDate = (DateTime)oTargetDoc.PaidDate;

                    remarks = jememo;
                    //if (remarks.Length > 50)
                    //    remarks = remarks.Substring(0, 50);
                    oDoc.Memo = remarks;

                    if (!string.IsNullOrEmpty(oTargetDoc.RefNo))
                        oDoc.Reference = oTargetDoc.RefNo;
                    oDoc.Reference3 = oTargetDoc.DocNum;

                    if (oTargetDoc.Company.ClaimSeriesNo > 0)
                        oDoc.Series = oTargetDoc.Company.ClaimSeriesNo;

                    int cnt = 0;
                    string acctcode = "";
                    string formatcode = "";
                    foreach (FT_EClaim.Module.BusinessObjects.ClaimTrxPostDetails dtl in oTargetDoc.ClaimTrxPostDetail)
                    {
                        if (dtl.Amount != 0)
                        {
                            cnt++;
                            if (cnt == 1)
                            {
                            }
                            else
                            {
                                oDoc.Lines.Add();
                                oDoc.Lines.SetCurrentLine(oDoc.Lines.Count - 1);
                            }

                            remarks = jememo;

                            if (!string.IsNullOrEmpty(dtl.Remarks))
                                remarks = dtl.IsLineJERemarks ? jememo : dtl.Remarks;
                            //if (remarks.Length > 50)
                            //    remarks = remarks.Substring(0, 50);

                            oDoc.Lines.LineMemo = remarks;

                            if (dtl.Amount > 0)
                                oDoc.Lines.Debit = (double)dtl.Amount;
                            else
                                oDoc.Lines.Credit = (double)dtl.Amount * -1;

                            if (dtl.Tax != null && dtl.TaxAmount > 0)
                            {
                                oDoc.Lines.TaxCode = dtl.Tax.BoCode;
                                oDoc.Lines.VatAmount = (double)dtl.TaxAmount;
                            }

                            if (!string.IsNullOrEmpty(dtl.AccountCode))
                                formatcode = dtl.AccountCode;

                            BusinessObjects.Accounts OACT = ObjectSpace.FindObject<BusinessObjects.Accounts>(new BinaryOperator("BoCode", formatcode, BinaryOperatorType.Equal));
                            //if (OACT == null)
                            //{
                            //    if (GeneralSettings.oCompany.InTransaction)
                            //    {
                            //        GeneralSettings.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                            //    }
                            //    showMsg("Failed", formatcode + " is not found in Chart of Account.", InformationType.Error);
                            //    return -1;
                            //}
                            //else
                            acctcode = OACT.SystemCode;

                            oDoc.Lines.AccountCode = acctcode;

                            if (!string.IsNullOrEmpty(dtl.DepartmentCode))
                            {
                                if (GeneralSettings.B1DeptDimension == 1)
                                    oDoc.Lines.CostingCode = dtl.DepartmentCode;
                                else if (GeneralSettings.B1DeptDimension == 2)
                                    oDoc.Lines.CostingCode2 = dtl.DepartmentCode;
                                else if (GeneralSettings.B1DeptDimension == 3)
                                    oDoc.Lines.CostingCode3 = dtl.DepartmentCode;
                                else if (GeneralSettings.B1DeptDimension == 4)
                                    oDoc.Lines.CostingCode4 = dtl.DepartmentCode;
                                else if (GeneralSettings.B1DeptDimension == 5)
                                    oDoc.Lines.CostingCode5 = dtl.DepartmentCode;
                            }
                            else if (dtl.Department != null)
                            {
                                if (GeneralSettings.B1DeptDimension == 1)
                                    oDoc.Lines.CostingCode = dtl.Department.PostCode;
                                else if (GeneralSettings.B1DeptDimension == 2)
                                    oDoc.Lines.CostingCode2 = dtl.Department.PostCode;
                                else if (GeneralSettings.B1DeptDimension == 3)
                                    oDoc.Lines.CostingCode3 = dtl.Department.PostCode;
                                else if (GeneralSettings.B1DeptDimension == 4)
                                    oDoc.Lines.CostingCode4 = dtl.Department.PostCode;
                                else if (GeneralSettings.B1DeptDimension == 5)
                                    oDoc.Lines.CostingCode5 = dtl.Department.PostCode;
                            }
                            if (!string.IsNullOrEmpty(dtl.DivisionCode))
                            {
                                if (GeneralSettings.B1DivDimension == 1)
                                    oDoc.Lines.CostingCode = dtl.DivisionCode;
                                else if (GeneralSettings.B1DivDimension == 2)
                                    oDoc.Lines.CostingCode2 = dtl.DivisionCode;
                                else if (GeneralSettings.B1DivDimension == 3)
                                    oDoc.Lines.CostingCode3 = dtl.DivisionCode;
                                else if (GeneralSettings.B1DivDimension == 4)
                                    oDoc.Lines.CostingCode4 = dtl.DivisionCode;
                                else if (GeneralSettings.B1DivDimension == 5)
                                    oDoc.Lines.CostingCode5 = dtl.DivisionCode;
                            }
                            else if (dtl.Division != null)
                            {
                                if (GeneralSettings.B1DivDimension == 1)
                                    oDoc.Lines.CostingCode = dtl.Division.PostCode;
                                else if (GeneralSettings.B1DivDimension == 2)
                                    oDoc.Lines.CostingCode2 = dtl.Division.PostCode;
                                else if (GeneralSettings.B1DivDimension == 3)
                                    oDoc.Lines.CostingCode3 = dtl.Division.PostCode;
                                else if (GeneralSettings.B1DivDimension == 4)
                                    oDoc.Lines.CostingCode4 = dtl.Division.PostCode;
                                else if (GeneralSettings.B1DivDimension == 5)
                                    oDoc.Lines.CostingCode5 = dtl.Division.PostCode;
                            }
                            if (!string.IsNullOrEmpty(dtl.BrandCode))
                            {
                                if (GeneralSettings.B1BrandDimension == 1)
                                    oDoc.Lines.CostingCode = dtl.BrandCode;
                                else if (GeneralSettings.B1BrandDimension == 2)
                                    oDoc.Lines.CostingCode2 = dtl.BrandCode;
                                else if (GeneralSettings.B1BrandDimension == 3)
                                    oDoc.Lines.CostingCode3 = dtl.BrandCode;
                                else if (GeneralSettings.B1BrandDimension == 4)
                                    oDoc.Lines.CostingCode4 = dtl.BrandCode;
                                else if (GeneralSettings.B1BrandDimension == 5)
                                    oDoc.Lines.CostingCode5 = dtl.BrandCode;
                            }
                            else if (dtl.Brand != null)
                            {
                                if (GeneralSettings.B1BrandDimension == 1)
                                    oDoc.Lines.CostingCode = dtl.Brand.PostCode;
                                else if (GeneralSettings.B1BrandDimension == 2)
                                    oDoc.Lines.CostingCode2 = dtl.Brand.PostCode;
                                else if (GeneralSettings.B1BrandDimension == 3)
                                    oDoc.Lines.CostingCode3 = dtl.Brand.PostCode;
                                else if (GeneralSettings.B1BrandDimension == 4)
                                    oDoc.Lines.CostingCode4 = dtl.Brand.PostCode;
                                else if (GeneralSettings.B1BrandDimension == 5)
                                    oDoc.Lines.CostingCode5 = dtl.Brand.PostCode;
                            }

                            if (!string.IsNullOrEmpty(dtl.ProjectCode))
                                oDoc.Lines.ProjectCode = dtl.ProjectCode;
                            else if (dtl.Project != null)
                                oDoc.Lines.ProjectCode = dtl.Project.PostCode;

                            //if (!string.IsNullOrEmpty(dtl.RefNo))
                            //    if (!string.IsNullOrEmpty(GeneralSettings.B1RefCol))
                            //        oDoc.Lines.UserFields.Fields.Item(GeneralSettings.B1RefCol).Value = dtl.Oid;

                        }
                    }


                    int rc = oDoc.Add();
                    if (rc != 0)
                    {
                        temp = GeneralSettings.oCompany.GetLastErrorDescription();
                        if (GeneralSettings.oCompany.InTransaction)
                        {
                            GeneralSettings.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                        }
                        showMsg("Failed", "[" + oTargetDoc.DocNum + "] " + temp, InformationType.Error);
                        return -1;
                    }
                    #region check post data exist
                    string docentry = "";
                    GeneralSettings.oCompany.GetNewObjectCode(out docentry);
                    if (docentry == "")
                    {
                        temp = "Unknown Error! Please try again!";
                        if (GeneralSettings.oCompany.InTransaction)
                        {
                            GeneralSettings.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                        }
                        showMsg("Failed", "[" + oTargetDoc.DocNum + "] " + temp, InformationType.Error);
                        return -1;
                    }

                    SAPbobsCOM.JournalEntries getDoc = (SAPbobsCOM.JournalEntries)GeneralSettings.oCompany.GetBusinessObject(BoObjectTypes.oJournalEntries);

                    if (!getDoc.GetByKey(int.Parse(docentry)))
                    {
                        temp = "[" + oTargetDoc.DocNum + "] " +  "Unknown Error! Please try again!";
                        if (GeneralSettings.oCompany.InTransaction)
                        {
                            GeneralSettings.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                        }
                        oDoc = null;
                        return -1;
                    }
                    #endregion

                    return int.Parse(docentry);
                }
                return 0;
            }
            catch (Exception ex)
            {
                showMsg("Error", "[" + oTargetDoc.DocNum + "] " + ex.Message, InformationType.Error);
                return -1;
            }

        }

        public int SendEmail(string MailSubject, string MailBody, List<string> ToEmails)
        {
            try
            {
                // return 0 = sent nothing
                // return -1 = sent error
                // return 1 = sent successful
                if (!GeneralSettings.EmailSend) return 0;
                if (ToEmails.Count <= 0) return 0;

                MailMessage mailMsg = new MailMessage();

                mailMsg.From = new MailAddress(GeneralSettings.Email, GeneralSettings.EmailName);

                foreach (string ToEmail in ToEmails)
                {
                    mailMsg.To.Add(ToEmail);
                }

                mailMsg.Subject = MailSubject;
                //mailMsg.SubjectEncoding = System.Text.Encoding.UTF8;
                //mailMsg.IsBodyHtml = true;
                mailMsg.Body = MailBody;

                SmtpClient smtpClient = new SmtpClient
                {
                    EnableSsl = GeneralSettings.EmailSSL,
                    UseDefaultCredentials = GeneralSettings.EmailUseDefaultCredential,
                    Host = GeneralSettings.EmailHost,
                    Port = int.Parse(GeneralSettings.EmailPort),
                };

                if (Enum.IsDefined(typeof(SmtpDeliveryMethod), GeneralSettings.DeliveryMethod))
                    smtpClient.DeliveryMethod = (SmtpDeliveryMethod)Enum.Parse(typeof(SmtpDeliveryMethod), GeneralSettings.DeliveryMethod);

                if (!smtpClient.UseDefaultCredentials)
                {
                    if (string.IsNullOrEmpty(GeneralSettings.EmailHostDomain))
                        smtpClient.Credentials = new NetworkCredential(GeneralSettings.Email, GeneralSettings.EmailPassword);
                    else
                        smtpClient.Credentials = new NetworkCredential(GeneralSettings.Email, GeneralSettings.EmailPassword, GeneralSettings.EmailHostDomain);
                }
                //if (GeneralSettings.EmailHost.ToLower() == "smtp.office365.com")
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                smtpClient.Send(mailMsg);

                mailMsg.Dispose();
                smtpClient.Dispose();

                return 1;
            }
            catch (Exception ex)
            {
                showMsg("Cannot send email", ex.Message, InformationType.Error);
                return -1;
            }
        }

        public int SendEmail_By_Object(EmailSents emailobj)
        {
            try
            {
                // return 0 = sent nothing
                // return -1 = sent error
                // return 1 = sent successful
                if (emailobj.EmailSentDetail.Count <= 0) return 0;

                List<string> ToEmails = new List<string>();
                foreach (EmailSentDetails ToEmail in emailobj.EmailSentDetail)
                {
                    ToEmails.Add(ToEmail.EmailAddress);
                }

                return SendEmail(emailobj.EmailSubject, emailobj.EmailBody, ToEmails);
            }
            catch (Exception ex)
            {
                showMsg("Cannot send email", ex.Message, InformationType.Error);
                return -1;
            }
        }
        public void duplicateclaim(ref ClaimTrxs claimtrx, ClaimTrxs obj, IObjectSpace os)
        {
            claimtrx.Company = os.FindObject<Companies>(new BinaryOperator("BoCode", obj.Company.BoCode));
            claimtrx.DocType = os.FindObject<DocTypes>(new BinaryOperator("BoCode", obj.DocType.BoCode));
            claimtrx.Employee = os.FindObject<Employees>(new BinaryOperator("BoCode", obj.Employee.BoCode));
            claimtrx.RefNo = obj.RefNo;
            claimtrx.Remarks = obj.Remarks;

            if (obj.Department != null)
                claimtrx.Department = os.GetObjectByKey<Departments>(obj.Department.Oid);
            else
            {
                if (claimtrx.Employee.Department != null)
                    claimtrx.Department = os.GetObjectByKey<Departments>(claimtrx.Employee.Department.Oid);
            }
            if (obj.Division != null)
                claimtrx.Division = os.GetObjectByKey<Divisions>(obj.Division.Oid);
            else
            {
                if (claimtrx.Employee.Division != null)
                    claimtrx.Division = os.GetObjectByKey<Divisions>(claimtrx.Employee.Division.Oid);
            }
            if (obj.Brand != null)
                claimtrx.Brand = os.GetObjectByKey<Brands>(obj.Brand.Oid);
            else
            {
                if (claimtrx.Employee.Brand != null)
                    claimtrx.Brand = os.GetObjectByKey<Brands>(claimtrx.Employee.Brand.Oid);
            }

            if (obj.Project != null)
                claimtrx.Project = os.GetObjectByKey<Projects>(obj.Project.Oid);
            if (obj.Currency != null)
                claimtrx.Currency = os.GetObjectByKey<BusinessObjects.Currencies>(obj.Currency.Oid);

            claimtrx.FCRate = obj.FCRate;
            claimtrx.FCAmount = obj.FCAmount;
            claimtrx.AdvanceAmount = obj.AdvanceAmount;

            claimtrx.DocDate = DateTime.Today;

        }
        public void duplicatedetail(ref ClaimTrxDetails newdtl, ClaimTrxDetails dtl, IObjectSpace os)
        {
            if (dtl.ClaimType != null)
                newdtl.ClaimType = newdtl.Session.GetObjectByKey<ClaimTypes>(dtl.ClaimType.Oid);
            if (dtl.Department != null)
                newdtl.Department = newdtl.Session.GetObjectByKey<Departments>(dtl.Department.Oid);
            if (dtl.Project != null)
                newdtl.Project = newdtl.Session.GetObjectByKey<Projects>(dtl.Project.Oid);
            if (dtl.Division != null)
                newdtl.Division = newdtl.Session.GetObjectByKey<Divisions>(dtl.Division.Oid);
            if (dtl.Brand != null)
                newdtl.Brand = newdtl.Session.GetObjectByKey<Brands>(dtl.Brand.Oid);
            if (dtl.Tax != null)
                newdtl.Tax = newdtl.Session.GetObjectByKey<Taxes>(dtl.Tax.Oid);
            if (dtl.Currency != null)
                newdtl.Currency = newdtl.Session.GetObjectByKey<BusinessObjects.Currencies>(dtl.Currency.Oid);

            //newdtl.DocDate = dtl.DocDate;
            newdtl.RefNo = dtl.RefNo;
            newdtl.Remarks = dtl.Remarks;
            newdtl.Amount = dtl.Amount;
            newdtl.TaxAmount = dtl.TaxAmount;
            newdtl.FCRate = dtl.FCRate;
            newdtl.FCAmount = dtl.FCAmount;

            if (dtl.AdditionalInfo != null)
            {
                newdtl.AdditionalInfo = os.CreateObject<ClaimAdditionalInfos>();
                newdtl.AdditionalInfo.TypeOfEntertainment = dtl.AdditionalInfo.TypeOfEntertainment;
                newdtl.AdditionalInfo.PersonEntertainment = dtl.AdditionalInfo.PersonEntertainment;
                newdtl.AdditionalInfo.Destination = dtl.AdditionalInfo.Destination;
                newdtl.AdditionalInfo.Recipient = dtl.AdditionalInfo.Recipient;
                newdtl.AdditionalInfo.TypeofExpense = dtl.AdditionalInfo.TypeofExpense;

                if (dtl.AdditionalInfo.Relationship != null)
                    newdtl.AdditionalInfo.Relationship = newdtl.AdditionalInfo.Session.GetObjectByKey<AddInfoCompanyRelationship>(dtl.AdditionalInfo.Relationship.Oid);
                if (dtl.AdditionalInfo.Employee != null)
                    newdtl.AdditionalInfo.Employee = newdtl.AdditionalInfo.Session.GetObjectByKey<Employees>(dtl.AdditionalInfo.Employee.Oid);
                if (dtl.AdditionalInfo.Department != null)
                    newdtl.AdditionalInfo.Department = newdtl.AdditionalInfo.Session.GetObjectByKey<Departments>(dtl.AdditionalInfo.Department.Oid);
                if (dtl.AdditionalInfo.Purpose != null)
                    newdtl.AdditionalInfo.Purpose = newdtl.AdditionalInfo.Session.GetObjectByKey<AddInfoPurpose>(dtl.AdditionalInfo.Purpose.Oid);
                if (dtl.AdditionalInfo.CompanyRelationship != null)
                    newdtl.AdditionalInfo.CompanyRelationship = newdtl.AdditionalInfo.Session.GetObjectByKey<AddInfoCompanyRelationship>(dtl.AdditionalInfo.CompanyRelationship.Oid);
            }
        }
        public void duplicatedetailnote(ref ClaimTrxDetailNotes newdtl, ClaimTrxDetailNotes dtl, IObjectSpace os)
        {

            if (dtl.Department != null)
                newdtl.Department = newdtl.Session.GetObjectByKey<Departments>(dtl.Department.Oid);
            if (dtl.Project != null)
                newdtl.Project = newdtl.Session.GetObjectByKey<Projects>(dtl.Project.Oid);
            if (dtl.Division != null)
                newdtl.Division = newdtl.Session.GetObjectByKey<Divisions>(dtl.Division.Oid);
            if (dtl.Brand != null)
                newdtl.Brand = newdtl.Session.GetObjectByKey<Brands>(dtl.Brand.Oid);
            if (dtl.Tax != null)
                newdtl.Tax = newdtl.Session.GetObjectByKey<Taxes>(dtl.Tax.Oid);
            if (dtl.Currency != null)
                newdtl.Currency = newdtl.Session.GetObjectByKey<BusinessObjects.Currencies>(dtl.Currency.Oid);

            //newdtl.DocDate = dtl.DocDate;

            newdtl.RefNo = dtl.RefNo;
            newdtl.Remarks = dtl.Remarks;
            newdtl.Amount = dtl.Amount;
            newdtl.TaxAmount = dtl.TaxAmount;
            newdtl.FCRate = dtl.FCRate;
            newdtl.FCAmount = dtl.FCAmount;

            if (dtl.AdditionalInfo != null)
            {
                newdtl.AdditionalInfo = os.CreateObject<ClaimAdditionalInfos>();
                newdtl.AdditionalInfo.TypeOfEntertainment = dtl.AdditionalInfo.TypeOfEntertainment;
                newdtl.AdditionalInfo.PersonEntertainment = dtl.AdditionalInfo.PersonEntertainment;
                newdtl.AdditionalInfo.Destination = dtl.AdditionalInfo.Destination;
                newdtl.AdditionalInfo.Recipient = dtl.AdditionalInfo.Recipient;
                newdtl.AdditionalInfo.TypeofExpense = dtl.AdditionalInfo.TypeofExpense;

                if (dtl.AdditionalInfo.Relationship != null)
                    newdtl.AdditionalInfo.Relationship = newdtl.AdditionalInfo.Session.GetObjectByKey<AddInfoCompanyRelationship>(dtl.AdditionalInfo.Relationship.Oid);
                if (dtl.AdditionalInfo.Employee != null)
                    newdtl.AdditionalInfo.Employee = newdtl.AdditionalInfo.Session.GetObjectByKey<Employees>(dtl.AdditionalInfo.Employee.Oid);
                if (dtl.AdditionalInfo.Department != null)
                    newdtl.AdditionalInfo.Department = newdtl.AdditionalInfo.Session.GetObjectByKey<Departments>(dtl.AdditionalInfo.Department.Oid);
                if (dtl.AdditionalInfo.Purpose != null)
                    newdtl.AdditionalInfo.Purpose = newdtl.AdditionalInfo.Session.GetObjectByKey<AddInfoPurpose>(dtl.AdditionalInfo.Purpose.Oid);
                if (dtl.AdditionalInfo.CompanyRelationship != null)
                    newdtl.AdditionalInfo.CompanyRelationship = newdtl.AdditionalInfo.Session.GetObjectByKey<AddInfoCompanyRelationship>(dtl.AdditionalInfo.CompanyRelationship.Oid);
            }
        }

        public void duplicatedetailItem(ref ClaimTrxItems newdtl, ClaimTrxItems dtl, IObjectSpace os)
        {

            if (dtl.ClaimItem != null)
                newdtl.ClaimItem = newdtl.Session.GetObjectByKey<ClaimItems>(dtl.ClaimItem.Oid);
            if (dtl.Department != null)
                newdtl.Department = newdtl.Session.GetObjectByKey<Departments>(dtl.Department.Oid);
            if (dtl.Project != null)
                newdtl.Project = newdtl.Session.GetObjectByKey<Projects>(dtl.Project.Oid);
            if (dtl.Division != null)
                newdtl.Division = newdtl.Session.GetObjectByKey<Divisions>(dtl.Division.Oid);
            if (dtl.Brand != null)
                newdtl.Brand = newdtl.Session.GetObjectByKey<Brands>(dtl.Brand.Oid);
            if (dtl.Tax != null)
                newdtl.Tax = newdtl.Session.GetObjectByKey<Taxes>(dtl.Tax.Oid);
            if (dtl.Currency != null)
                newdtl.Currency = newdtl.Session.GetObjectByKey<BusinessObjects.Currencies>(dtl.Currency.Oid);

            //newdtl.DocDate = dtl.DocDate;
            newdtl.RefNo = dtl.RefNo;
            newdtl.Remarks = dtl.Remarks;
            newdtl.Amount = dtl.Amount;
            newdtl.TaxAmount = dtl.TaxAmount;
            newdtl.FCRate = dtl.FCRate;
            newdtl.FCAmount = dtl.FCAmount;

            if (dtl.AdditionalInfo != null)
            {
                newdtl.AdditionalInfo = os.CreateObject<ClaimAdditionalInfos>();
                newdtl.AdditionalInfo.TypeOfEntertainment = dtl.AdditionalInfo.TypeOfEntertainment;
                newdtl.AdditionalInfo.PersonEntertainment = dtl.AdditionalInfo.PersonEntertainment;
                newdtl.AdditionalInfo.Destination = dtl.AdditionalInfo.Destination;
                newdtl.AdditionalInfo.Recipient = dtl.AdditionalInfo.Recipient;
                newdtl.AdditionalInfo.TypeofExpense = dtl.AdditionalInfo.TypeofExpense;

                if (dtl.AdditionalInfo.Relationship != null)
                    newdtl.AdditionalInfo.Relationship = newdtl.AdditionalInfo.Session.GetObjectByKey<AddInfoCompanyRelationship>(dtl.AdditionalInfo.Relationship.Oid);
                if (dtl.AdditionalInfo.Employee != null)
                    newdtl.AdditionalInfo.Employee = newdtl.AdditionalInfo.Session.GetObjectByKey<Employees>(dtl.AdditionalInfo.Employee.Oid);
                if (dtl.AdditionalInfo.Department != null)
                    newdtl.AdditionalInfo.Department = newdtl.AdditionalInfo.Session.GetObjectByKey<Departments>(dtl.AdditionalInfo.Department.Oid);
                if (dtl.AdditionalInfo.Purpose != null)
                    newdtl.AdditionalInfo.Purpose = newdtl.AdditionalInfo.Session.GetObjectByKey<AddInfoPurpose>(dtl.AdditionalInfo.Purpose.Oid);
                if (dtl.AdditionalInfo.CompanyRelationship != null)
                    newdtl.AdditionalInfo.CompanyRelationship = newdtl.AdditionalInfo.Session.GetObjectByKey<AddInfoCompanyRelationship>(dtl.AdditionalInfo.CompanyRelationship.Oid);
            }
        }
        public void duplicatedetailImileage(ref ClaimTrxMileages newdtl, ClaimTrxMileages dtl, IObjectSpace os)
        {

            if (dtl.Mileage != null)
                newdtl.Mileage = newdtl.Session.GetObjectByKey<Mileages>(dtl.Mileage.Oid);
            if (dtl.Department != null)
                newdtl.Department = newdtl.Session.GetObjectByKey<Departments>(dtl.Department.Oid);
            if (dtl.Project != null)
                newdtl.Project = newdtl.Session.GetObjectByKey<Projects>(dtl.Project.Oid);
            if (dtl.Division != null)
                newdtl.Division = newdtl.Session.GetObjectByKey<Divisions>(dtl.Division.Oid);
            if (dtl.Brand != null)
                newdtl.Brand = newdtl.Session.GetObjectByKey<Brands>(dtl.Brand.Oid);

            //newdtl.DocDate = dtl.DocDate;
            newdtl.RefNo = dtl.RefNo;
            newdtl.Remarks = dtl.Remarks;
            newdtl.KM = dtl.KM;
        }

    }
}
