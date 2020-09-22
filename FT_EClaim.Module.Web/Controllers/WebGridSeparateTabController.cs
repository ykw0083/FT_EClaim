using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web;

namespace MainDemo.Module.Web
{
    public class WebGridSeparateTabController : ViewController<ListView>
    {
        protected const string UrlKeyPlaceholder = "dxdkey";
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            ASPxGridListEditor listEditor = View.Editor as ASPxGridListEditor;
            if (listEditor != null && listEditor.Grid != null)
            {
                listEditor.Grid.CustomJSProperties += Grid_CustomJSProperties;
            }
        }
        protected override void OnDeactivated()
        {
            ASPxGridListEditor listEditor = View.Editor as ASPxGridListEditor;
            if (listEditor != null && listEditor.Grid != null)
            {
                listEditor.Grid.CustomJSProperties -= Grid_CustomJSProperties;
            }
            base.OnDeactivated();
        }
        protected virtual string GetDetailViewUrl()
        {
            ViewShortcut shortcut = new ViewShortcut(View.Model.DetailView.Id, UrlKeyPlaceholder);
            IHttpRequestManager requestManager = ((WebApplication)Application).RequestManager;
            string url = string.Format("{0}#{1}", WebApplication.DefaultPage, requestManager.GetQueryString(shortcut));
            return url;
        }
        private void Grid_CustomJSProperties(object sender, ASPxGridViewClientJSPropertiesEventArgs e)
        {
            e.Properties["cpNewTabViewUrl"] = GetDetailViewUrl();
            e.Properties["cpUrlKeyPlaceholder"] = UrlKeyPlaceholder;
        }
    }
}