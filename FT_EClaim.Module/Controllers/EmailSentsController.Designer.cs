namespace FT_EClaim.Module.Controllers
{
    partial class EmailSentsController
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
            this.SendEmail = new DevExpress.ExpressApp.Actions.ParametrizedAction(this.components);
            // 
            // SendEmail
            // 
            this.SendEmail.Caption = "Test Email";
            this.SendEmail.ConfirmationMessage = null;
            this.SendEmail.Id = "SendEmail";
            this.SendEmail.NullValuePrompt = null;
            this.SendEmail.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
            this.SendEmail.ShortCaption = null;
            this.SendEmail.ToolTip = null;
            this.SendEmail.Execute += new DevExpress.ExpressApp.Actions.ParametrizedActionExecuteEventHandler(this.SendEmail_Execute);
            // 
            // EmailSentsController
            // 
            this.Actions.Add(this.SendEmail);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.ParametrizedAction SendEmail;
    }
}
