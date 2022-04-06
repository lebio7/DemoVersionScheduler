using DemoVersionScheduler.Module.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Scheduler.Win;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.XtraScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoVersionScheduler.Module.Win.Controllers
{
    public class SchedulerController : ObjectViewController<ListView, Patients>
    {

        protected override void OnActivated()
        {
            base.OnActivated();

            if (View.Editor is SchedulerListEditor scheduler && scheduler != null)
            {
                SchedulerCompatibility.Base64XmlObjectSerialization = false;
            }
        }

    }
}
