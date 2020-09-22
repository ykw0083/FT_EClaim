namespace FT_EClaim.Module.Controllers
{
    partial class ClaimTrxPostControllers
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
            this.PostClaim = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.PostClaimV2 = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // PostClaim
            // 
            this.PostClaim.Caption = "Post Claim";
            this.PostClaim.ConfirmationMessage = "Posting is unreversable. Are you sure you want to continue?";
            this.PostClaim.Id = "PostClaim";
            this.PostClaim.ToolTip = null;
            this.PostClaim.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.PostClaim_Execute);
            // 
            // PostClaimV2
            // 
            this.PostClaimV2.AcceptButtonCaption = null;
            this.PostClaimV2.CancelButtonCaption = null;
            this.PostClaimV2.Caption = ".Post Claim.";
            this.PostClaimV2.ConfirmationMessage = null;
            this.PostClaimV2.Id = "PostClaimV2";
            this.PostClaimV2.ToolTip = null;
            this.PostClaimV2.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.PostClaimV2_CustomizePopupWindowParams);
            this.PostClaimV2.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.PostClaimV2_Execute);
            // 
            // ClaimTrxPostControllers
            // 
            this.Actions.Add(this.PostClaim);
            this.Actions.Add(this.PostClaimV2);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction PostClaim;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction PostClaimV2;
    }
}
