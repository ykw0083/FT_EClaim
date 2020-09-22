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
using DevExpress.ExpressApp.Scheduler;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using FT_EClaim.Module.BusinessObjects;

namespace FT_EClaim.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class CustomEventControllers : ViewController
    {
        private SchedulerListEditorBase schedulerEditor;
        public CustomEventControllers()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
            this.TargetViewType = ViewType.ListView;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            schedulerEditor = ((ListView)View).Editor as SchedulerListEditorBase;
            if (schedulerEditor != null)
            {
                schedulerEditor.ExceptionEventCreated +=
                    new EventHandler<ExceptionEventCreatedEventArgs>(
                       schedulerEditor_ExceptionEventCreated);
            }
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            if (schedulerEditor != null)
            {
                schedulerEditor.ExceptionEventCreated -=
                    new EventHandler<ExceptionEventCreatedEventArgs>(
                       schedulerEditor_ExceptionEventCreated);
            }
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
        void schedulerEditor_ExceptionEventCreated(object sender, ExceptionEventCreatedEventArgs e)
        {
            if (e.PatternEvent is ExtendedEvents && e.ExceptionEvent is ExtendedEvents)
            {
                ((ExtendedEvents)e.ExceptionEvent).Notes = ((ExtendedEvents)e.PatternEvent).Notes;
                Validator.RuleSet.Validate(ObjectSpace, e.ExceptionEvent, "SchedulerValidation");
            }
        }
    }
}
