namespace FT_EClaim.Module.Controllers
{
    partial class ClaimTrxControllers
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.PassDoc = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.CancelDoc = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.RejectDoc = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.AcceptDoc = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.CloseDoc = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.ReopenDoc = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.Approval = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.DuplicateDoc = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.SetPaidDate = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.ApprovalApprove = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.ApprovalReject = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.ApproveSelected = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.CloseSelected = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // PassDoc
            // 
            this.PassDoc.AcceptButtonCaption = null;
            this.PassDoc.CancelButtonCaption = null;
            this.PassDoc.Caption = "Pass";
            this.PassDoc.ConfirmationMessage = null;
            this.PassDoc.Id = "PassDoc";
            this.PassDoc.ToolTip = null;
            this.PassDoc.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.StringParametersCustomizePopupWindowParams);
            this.PassDoc.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.PassDoc_Execute);
            // 
            // CancelDoc
            // 
            this.CancelDoc.AcceptButtonCaption = null;
            this.CancelDoc.CancelButtonCaption = null;
            this.CancelDoc.Caption = "Cancel";
            this.CancelDoc.ConfirmationMessage = null;
            this.CancelDoc.Id = "CancelDoc";
            this.CancelDoc.ToolTip = null;
            this.CancelDoc.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.StringParametersCustomizePopupWindowParams);
            this.CancelDoc.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.CancelDoc_Execute);
            // 
            // RejectDoc
            // 
            this.RejectDoc.AcceptButtonCaption = null;
            this.RejectDoc.CancelButtonCaption = null;
            this.RejectDoc.Caption = "Reject Doc";
            this.RejectDoc.ConfirmationMessage = null;
            this.RejectDoc.Id = "RejectDoc";
            this.RejectDoc.ToolTip = "Return to User";
            this.RejectDoc.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.StringParametersCustomizePopupWindowParams);
            this.RejectDoc.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.RejectDoc_Execute);
            // 
            // AcceptDoc
            // 
            this.AcceptDoc.AcceptButtonCaption = null;
            this.AcceptDoc.CancelButtonCaption = null;
            this.AcceptDoc.Caption = "Accept";
            this.AcceptDoc.ConfirmationMessage = null;
            this.AcceptDoc.Id = "AcceptDoc";
            this.AcceptDoc.ToolTip = null;
            this.AcceptDoc.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.StringParametersCustomizePopupWindowParams);
            this.AcceptDoc.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.AcceptDoc_Execute);
            // 
            // CloseDoc
            // 
            this.CloseDoc.AcceptButtonCaption = null;
            this.CloseDoc.CancelButtonCaption = null;
            this.CloseDoc.Caption = "Close";
            this.CloseDoc.ConfirmationMessage = null;
            this.CloseDoc.Id = "CloseDoc";
            this.CloseDoc.ToolTip = null;
            this.CloseDoc.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.StringParametersCustomizePopupWindowParams);
            this.CloseDoc.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.CloseDoc_Execute);
            // 
            // ReopenDoc
            // 
            this.ReopenDoc.AcceptButtonCaption = null;
            this.ReopenDoc.CancelButtonCaption = null;
            this.ReopenDoc.Caption = "Re-open";
            this.ReopenDoc.ConfirmationMessage = null;
            this.ReopenDoc.Id = "ReopenDoc";
            this.ReopenDoc.ToolTip = null;
            this.ReopenDoc.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.StringParametersCustomizePopupWindowParams);
            this.ReopenDoc.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.ReopenDoc_Execute);
            // 
            // Approval
            // 
            this.Approval.AcceptButtonCaption = null;
            this.Approval.CancelButtonCaption = null;
            this.Approval.Caption = "Approval";
            this.Approval.ConfirmationMessage = null;
            this.Approval.Id = "Approval";
            this.Approval.ToolTip = null;
            this.Approval.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.Approval_CustomizePopupWindowParams);
            this.Approval.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.Approval_Execute);
            // 
            // DuplicateDoc
            // 
            this.DuplicateDoc.Caption = "Duplicate Doc";
            this.DuplicateDoc.ConfirmationMessage = null;
            this.DuplicateDoc.Id = "DuplicateDoc";
            this.DuplicateDoc.ToolTip = null;
            this.DuplicateDoc.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.DuplicateDoc_Execute);
            // 
            // SetPaidDate
            // 
            this.SetPaidDate.AcceptButtonCaption = null;
            this.SetPaidDate.CancelButtonCaption = null;
            this.SetPaidDate.Caption = "Paid Date";
            this.SetPaidDate.ConfirmationMessage = null;
            this.SetPaidDate.Id = "SetPaidDate";
            this.SetPaidDate.ToolTip = null;
            this.SetPaidDate.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.SetPaidDate_CustomizePopupWindowParams);
            this.SetPaidDate.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.SetPaidDate_Execute);
            // 
            // ApprovalApprove
            // 
            this.ApprovalApprove.AcceptButtonCaption = null;
            this.ApprovalApprove.CancelButtonCaption = null;
            this.ApprovalApprove.Caption = "Approve";
            this.ApprovalApprove.ConfirmationMessage = null;
            this.ApprovalApprove.Id = "ApprovalApprove";
            this.ApprovalApprove.ToolTip = null;
            this.ApprovalApprove.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.Approval_CustomizePopupWindowParams);
            this.ApprovalApprove.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.Approval_Execute);
            // 
            // ApprovalReject
            // 
            this.ApprovalReject.AcceptButtonCaption = null;
            this.ApprovalReject.CancelButtonCaption = null;
            this.ApprovalReject.Caption = "Reject";
            this.ApprovalReject.ConfirmationMessage = null;
            this.ApprovalReject.Id = "ApprovalReject";
            this.ApprovalReject.ToolTip = null;
            this.ApprovalReject.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.Approval_CustomizePopupWindowParams);
            this.ApprovalReject.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.Approval_Execute);
            // 
            // ApproveSelected
            // 
            this.ApproveSelected.AcceptButtonCaption = null;
            this.ApproveSelected.CancelButtonCaption = null;
            this.ApproveSelected.Caption = "Approve Selected";
            this.ApproveSelected.ConfirmationMessage = null;
            this.ApproveSelected.Id = "ApproveSelected";
            this.ApproveSelected.ToolTip = null;
            this.ApproveSelected.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.Approval_CustomizePopupWindowParams);
            this.ApproveSelected.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.ApproveSelected_Execute);
            // 
            // CloseSelected
            // 
            this.CloseSelected.AcceptButtonCaption = null;
            this.CloseSelected.CancelButtonCaption = null;
            this.CloseSelected.Caption = "Close Selected";
            this.CloseSelected.ConfirmationMessage = null;
            this.CloseSelected.Id = "CloseSelected";
            this.CloseSelected.ToolTip = null;
            this.CloseSelected.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.StringParametersCustomizePopupWindowParams);
            this.CloseSelected.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.CloseSelected_Execute);
            // 
            // ClaimTrxControllers
            // 
            this.Actions.Add(this.PassDoc);
            this.Actions.Add(this.CancelDoc);
            this.Actions.Add(this.RejectDoc);
            this.Actions.Add(this.AcceptDoc);
            this.Actions.Add(this.CloseDoc);
            this.Actions.Add(this.ReopenDoc);
            this.Actions.Add(this.Approval);
            this.Actions.Add(this.DuplicateDoc);
            this.Actions.Add(this.SetPaidDate);
            this.Actions.Add(this.ApprovalApprove);
            this.Actions.Add(this.ApprovalReject);
            this.Actions.Add(this.ApproveSelected);
            this.Actions.Add(this.CloseSelected);

        }

        #endregion
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction PassDoc;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction CancelDoc;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction RejectDoc;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction AcceptDoc;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction CloseDoc;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction ReopenDoc;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction Approval;
        private DevExpress.ExpressApp.Actions.SimpleAction DuplicateDoc;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction SetPaidDate;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction ApprovalApprove;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction ApprovalReject;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction ApproveSelected;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction CloseSelected;
    }
}
