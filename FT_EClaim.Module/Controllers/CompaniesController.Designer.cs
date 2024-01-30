
namespace FT_EClaim.Module.Controllers
{
    partial class CompaniesController
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
            this.SAPConnection = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // SAPConnection
            // 
            this.SAPConnection.Caption = "SAP Connection";
            this.SAPConnection.ConfirmationMessage = null;
            this.SAPConnection.Id = "SAPConnection";
            this.SAPConnection.ToolTip = null;
            this.SAPConnection.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.SAPConnection_Execute);
            // 
            // CompaniesController
            // 
            this.Actions.Add(this.SAPConnection);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction SAPConnection;
    }
}
