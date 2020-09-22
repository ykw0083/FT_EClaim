using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using System.ComponentModel;

namespace FT_EClaim.Module.BusinessObjects
{
    //[DefaultClassOptions]
    //[NavigationItem("Task")]
    [DomainComponent]
    public class MyTasks
    {
        public MyTasks()
        {
            MyNotifications = new List<MyNotifications>();
        }
        [Browsable(false)]
        public int Oid { get; private set; }
        public string Subject { get; set; }
        [Aggregated]
        public virtual IList<MyNotifications> MyNotifications { get; set; }
    }
}