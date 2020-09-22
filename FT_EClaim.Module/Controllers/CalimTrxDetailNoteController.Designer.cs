namespace FT_EClaim.Module.Controllers
{
    partial class CalimTrxDetailNoteController
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
            this.DuplicateNote = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // DuplicateNote
            // 
            this.DuplicateNote.Caption = "Duplicate";
            this.DuplicateNote.ConfirmationMessage = null;
            this.DuplicateNote.Id = "DuplicateNote";
            this.DuplicateNote.ToolTip = null;
            this.DuplicateNote.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.DuplicateNote_Execute);
            // 
            // CalimTrxDetailNoteController
            // 
            this.Actions.Add(this.DuplicateNote);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction DuplicateNote;
    }
}
