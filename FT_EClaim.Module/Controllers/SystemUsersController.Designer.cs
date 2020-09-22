namespace FT_EClaim.Module.Controllers
{
    partial class SystemUsersController
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
            this.GotoEmp = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // GotoEmp
            // 
            this.GotoEmp.AcceptButtonCaption = null;
            this.GotoEmp.CancelButtonCaption = null;
            this.GotoEmp.Caption = "Goto Employee";
            this.GotoEmp.ConfirmationMessage = null;
            this.GotoEmp.Id = "GotoEmp";
            this.GotoEmp.ToolTip = null;
            this.GotoEmp.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.GotoEmp_CustomizePopupWindowParams);
            this.GotoEmp.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.GotoEmp_Execute);
            // 
            // SystemUsersController
            // 
            this.Actions.Add(this.GotoEmp);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction GotoEmp;
    }
}
