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
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Web;

namespace FT_EClaim.Module.Web.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class MyWebListViewController : ViewController<ListView>
    {
        public MyWebListViewController()
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
            ASPxGridListEditor gridListEditor = View.Editor as ASPxGridListEditor;
            if (gridListEditor != null)
            {
                if (View.ObjectTypeInfo.Type == typeof(FT_EClaim.Module.BusinessObjects.ClaimTrxs))
                #region select page rows
                if (gridListEditor.Model.DataAccessMode == CollectionSourceDataAccessMode.Server)
                {
                    foreach (var column in gridListEditor.Grid.Columns)
                    {
                        var commandColumn = column as GridViewCommandColumn;
                        if (commandColumn != null && commandColumn.ShowSelectCheckbox)
                        {
                            commandColumn.SelectAllCheckboxMode = GridViewSelectAllCheckBoxMode.Page;
                        }
                    }
                }
                #endregion

                //gridListEditor.Grid.SettingsPager.Mode = GridViewPagerMode.EndlessPaging;
                gridListEditor.Grid.SettingsPager.PageSize = 50;
                //gridListEditor.Grid.Width = Unit.Percentage(100);
                //gridListEditor.Grid.Settings.VerticalScrollableHeight = 500;
            }
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
