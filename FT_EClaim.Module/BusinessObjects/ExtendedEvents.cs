using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_EClaim.Module.BusinessObjects
{
    class ExtendedEvents : Event
    {
        public ExtendedEvents(Session session) : base(session) { }

        [RuleRequiredField("", "SchedulerValidation")]
        public string Notes
        {
            get { return GetPropertyValue<string>("Notes"); }
            set { SetPropertyValue<string>("Notes", value); }
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            Validator.RuleSet.Validate(XPObjectSpace.FindObjectSpaceByObject(this), this, "SchedulerValidation");
        }

    }
}
