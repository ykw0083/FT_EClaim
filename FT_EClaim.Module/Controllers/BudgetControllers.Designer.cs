namespace FT_EClaim.Module.Controllers
{
    partial class BudgetControllers
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
            this.AssignBudget = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // AssignBudget
            // 
            this.AssignBudget.AcceptButtonCaption = null;
            this.AssignBudget.CancelButtonCaption = null;
            this.AssignBudget.Caption = "Assign Budget";
            this.AssignBudget.ConfirmationMessage = null;
            this.AssignBudget.Id = "AssignBudget";
            this.AssignBudget.ToolTip = null;
            this.AssignBudget.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.AssignBudget_CustomizePopupWindowParams);
            this.AssignBudget.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.AssignBudget_Execute);
            // 
            // BudgetControllers
            // 
            this.Actions.Add(this.AssignBudget);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction AssignBudget;
    }
}
