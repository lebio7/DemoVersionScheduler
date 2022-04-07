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
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            //if (View.Editor is SchedulerListEditor scheduler && scheduler != null)
            //{
            //    SchedulerCompatibility.Base64XmlObjectSerialization = false;
            //    scheduler.ResourcesMappings.Id = "Oid";
            //    scheduler.SchedulerControl.AppointmentDrop += SchedulerControl_AppointmentDrop;
            //    scheduler.SchedulerControl.OptionsCustomization.AllowAppointmentDragBetweenResources = UsedAppointmentType.All;
            //    scheduler.SchedulerControl.OptionsCustomization.AllowAppointmentDrag = UsedAppointmentType.All;
            //    scheduler.SchedulerControl.GroupType = SchedulerGroupType.Resource;
            //}

        }

        void SchedulerControl_AppointmentDrop(object sender, DevExpress.XtraScheduler.AppointmentDragEventArgs e)
        {
            string createEventMsg = "Creating an event at {0} on {1}.";
            string moveEventMsg = "Moving the event from {0} on {1} to {2} on {3}.";

            DateTime srcStart = e.SourceAppointment.Start;
            DateTime newStart = e.EditedAppointment.Start;

            string msg = (srcStart == DateTime.MinValue) ? String.Format(createEventMsg, newStart.ToShortTimeString(), newStart.ToShortDateString()) :
                String.Format(moveEventMsg, srcStart.ToShortTimeString(), srcStart.ToShortDateString(), newStart.ToShortTimeString(), newStart.ToShortDateString());

            if (((DevExpress.ExpressApp.Win.WinApplication)Application).GetUserChoice(msg + "\r\nProceed?", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
            {
                e.Allow = false;
            }
        }
    }
}
