using DemoVersionScheduler.Module.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Scheduler.Win;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraScheduler;
using System;
using System.Drawing;
using System.Linq;

namespace DemoVersionScheduler.Module.Win.Controllers
{
    public class SchedulerDashboardController : ViewController<DashboardView>
    {
        public const string TimetableDashboardController = "Scheduler";
        public const string CalendarId = "Calendar";
        public const string ListId = "List";
        public const string ListViewId = "PatientsWithoutRoom";
        public const string CalendarViewId = "Patients_ListView_Calendar";

        private GridHitInfo downHitInfo;
        private SchedulerStorageBase schedulerStorage;

        public SchedulerDashboardController()
        {
            TargetViewId = TimetableDashboardController;
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            if (View.FindItem(CalendarId) != null)
            {
                View.FindItem(CalendarId).ControlCreated += DashboardViewItem_ControlCreated;
            }
            if (View.FindItem(ListId) != null)
            {
                View.FindItem(ListId).ControlCreated += DashboardViewItem_ControlCreated;
            }
        }

        private void DashboardViewItem_ControlCreated(object sender, EventArgs e)
        {
            ((DashboardViewItem)sender).InnerView.ControlsCreated += InnerView_ControlsCreated;
        }


        private void InnerView_ControlsCreated(object sender, EventArgs e)
        {
            var innerView = (ListView)sender;

            if (innerView.Id == ListViewId)
            {
                GridListEditor gridListEditor = (GridListEditor)innerView.Editor;
                gridListEditor.GridView.MouseMove += GridView_MouseMove;
                gridListEditor.GridView.MouseDown += GridView_MouseDown;
            }
            else if (innerView.Id == CalendarViewId)
            {
                var schedulerListEditor = (SchedulerListEditor)innerView.Editor;
                schedulerStorage = (SchedulerStorageBase)schedulerListEditor.StorageBase;
                schedulerListEditor.SchedulerControl.AppointmentDrop += SchedulerControl_AppointmentDrop;
                schedulerListEditor.SchedulerControl.OptionsCustomization.AllowAppointmentDragBetweenResources = UsedAppointmentType.All;
                schedulerListEditor.SchedulerControl.OptionsCustomization.AllowAppointmentDrag = UsedAppointmentType.All;
                schedulerListEditor.SchedulerControl.GroupType = SchedulerGroupType.Resource;
            }
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

            object keyPatient = e.SourceAppointment.Id == null ? e?.SourceAppointment?.LabelKey : e.SourceAppointment.Id;
            if (keyPatient != null)
            {
                var patient = ObjectSpace.GetObjectByKey<Patients>(keyPatient);
                if (patient != null)
                {
                    patient.StartOn = e.EditedAppointment.Start;
                    patient.EndOn = e.EditedAppointment.End;

                    if (e.EditedAppointment.ResourceId != null)
                    {
                        var room = ObjectSpace.GetObjectByKey<Room>(e.EditedAppointment.ResourceId);
                        if (room != null && !patient.Resources.Contains(room))
                        {
                            patient.Resources.Add(room);
                            patient.UpdateResources(true);
                        }
                    }
                }
            }

            ObjectSpace.CommitChanges();
        }

        private void GridView_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            GridView view = sender as GridView;
            downHitInfo = null;

            GridHitInfo hitInfo = view.CalcHitInfo(new Point(e.X, e.Y));
            if (System.Windows.Forms.Control.ModifierKeys != System.Windows.Forms.Keys.None)
            {
                return;
            }

            if (e.Button == System.Windows.Forms.MouseButtons.Left && hitInfo.InRow && hitInfo.HitTest != GridHitTest.RowIndicator)
            {
                downHitInfo = hitInfo;
            }
        }

        private void GridView_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Button == System.Windows.Forms.MouseButtons.Left && downHitInfo != null)
            {
                Size dragSize = System.Windows.Forms.SystemInformation.DragSize;
                Rectangle dragRect = new Rectangle(new Point(downHitInfo.HitPoint.X - dragSize.Width / 2,
                    downHitInfo.HitPoint.Y - dragSize.Height / 2), dragSize);

                if (!dragRect.Contains(new Point(e.X, e.Y)))
                {
                    view.GridControl.DoDragDrop(GetDragData(view), System.Windows.Forms.DragDropEffects.All);
                    downHitInfo = null;
                }
            }
        }

        private SchedulerDragData GetDragData(GridView view)
        {
            int[] selection = view.GetSelectedRows();
            if (selection == null)
            {
                return null;
            }

            AppointmentBaseCollection appointments = new AppointmentBaseCollection();
            int count = selection.Length;
            for (int i = 0; i < count; i++)
            {
                Patients obj = (Patients)view.GetRow(selection[i]);
                Appointment apt = schedulerStorage.CreateAppointment(AppointmentType.Normal);
                apt.Subject = obj.Subject;
                apt.Start = DateTime.Now;
                apt.LabelKey = obj.Oid;
                appointments.Add(apt);
            }

            return new SchedulerDragData(appointments, 0);
        }
    }
}
