namespace FT_EClaim.Module.Controllers
{
    partial class SwitchViewControllers
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
            this.SwitchView = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // SwitchView
            // 
            this.SwitchView.Caption = "Switch View";
            this.SwitchView.ConfirmationMessage = null;
            this.SwitchView.Id = "SwitchView";
            this.SwitchView.ToolTip = null;
            this.SwitchView.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.SwitchView_Execute);
            // 
            // SwitchViewControllers
            // 
            this.Actions.Add(this.SwitchView);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction SwitchView;
    }
}
