using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoVersionScheduler.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem("Patients")]
    [XafDisplayName("Doctor Room")]
    public class Room : BaseObject, IResource
    {
        public Room(Session session) : base(session)
        { }


        string caption;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Caption
        {
            get => caption;
            set => SetPropertyValue(nameof(Caption), ref caption, value);
        }

        [NonPersistent, Browsable(false)]
        public object Id => Oid;

        [Association("Event-Resource")]
        public XPCollection<Patients> Events
        {
            get { return GetCollection<Patients>(nameof(Events)); }
        }

        public int OleColor => 1;
    }
}
