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
    public partial class CalimTrxDetailNoteController : ViewController
    {
        GenControllers genCon;
        public CalimTrxDetailNoteController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
            TargetObjectType = typeof(ClaimTrxDetailNotes);
            TargetViewNesting = Nesting.Nested;
            TargetViewType = ViewType.ListView;
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
            genCon = Frame.GetController<GenControllers>();
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
        private void DuplicateNote_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (!ObjectSpace.IsModified)
            {
                genCon.showMsg("Error", "Please edit the item 1st.", InformationType.Error);
                return;
            }
            if (((ListView)View).SelectedObjects.Count != 1)
            {
                genCon.showMsg("Error", "Only 1 record to be duplicated at once.", InformationType.Error);
            }
            else
            {
                int minoid = 0;
                foreach (ClaimTrxDetailNotes dtl in ((ListView)View).CollectionSource.List)
                {
                    if (dtl.Oid < minoid) minoid = dtl.Oid -1;
                }

                foreach (ClaimTrxDetailNotes dtl in ((ListView)View).SelectedObjects)
                {
                    ClaimTrxDetailNotes newdtl = ObjectSpace.CreateObject<ClaimTrxDetailNotes>();
                    genCon.duplicatedetailnote(ref newdtl, dtl, ObjectSpace);
                    newdtl.Oid = (dtl.ClaimTrxDetail.Oid < 0 ? dtl.ClaimTrxDetail.Oid * 100 : dtl.ClaimTrxDetail.Oid * -100) + minoid;
                    newdtl.IsDuplicated = true;
                    ((ListView)View).CollectionSource.Add(newdtl);
                }
                genCon.showMsg("Success", "Item duplicated.", InformationType.Success);

            }
        }
    }
}
