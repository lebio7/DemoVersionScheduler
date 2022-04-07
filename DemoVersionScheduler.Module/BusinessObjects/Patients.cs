using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Xml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DemoVersionScheduler.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem("Patients")]
    [XafDisplayName("Registration Room")]
    public class Patients : BaseObject, IEvent
    {
        public Patients(Session session) : base(session)
        { }


        [Persistent("ResourceIds"), Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
        private String resourceIds;

        int age;
        string name;
        DateTime endOn;
        DateTime startOn;
        int type;
        int status;
        int label;
        string location;
        bool allDay;
        string description;
        string subject;

        [Association("Event-Resource")]
        public XPCollection<Room> Resources
        {
            get { return GetCollection<Room>(nameof(Resources)); }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Name
        {
            get => name;
            set => SetPropertyValue(nameof(Name), ref name, value);
        }

        
        public int Age
        {
            get => age;
            set => SetPropertyValue(nameof(Age), ref age, value);
        }

        [ModelDefault("DisplayFormat", "{0:dd.MM.yyyy HH:mm}")]
        [ModelDefault("EditMask", "dd.MM.yyyy HH:mm")]
        public DateTime StartOn
        {
            get => startOn;
            set => SetPropertyValue(nameof(StartOn), ref startOn, value);
        }

        [ModelDefault("DisplayFormat", "{0:dd.MM.yyyy HH:mm}")]
        [ModelDefault("EditMask", "dd.MM.yyyy HH:mm")]
        public DateTime EndOn
        {
            get => endOn;
            set => SetPropertyValue(nameof(EndOn), ref endOn, value);
        }

        [Size(SizeAttribute.Unlimited)]
        public string Subject
        {
            get
            {
                return subject;
            }
            set
            {
                SetPropertyValue(nameof(subject), ref subject, value);
            }
        }

        [Size(SizeAttribute.Unlimited)]
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                SetPropertyValue(nameof(description), ref description, value);
            }
        }

        public bool AllDay
        {
            get
            {
                return allDay;
            }
            set
            {
                SetPropertyValue(nameof(AllDay), ref allDay, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Location
        {
            get
            {
                return location;
            }
            set
            {
                SetPropertyValue(nameof(Location), ref location, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public int Label
        {
            get
            {
                return label;
            }
            set
            {
                SetPropertyValue(nameof(Label), ref label, value);
            }
        }

        [ModelDefault("AllowEdit", "False")]
        [Browsable(false)]
        int IEvent.Status
        {
            get
            {
                return 1;
            }
            set
            {
                SetPropertyValue(nameof(IEvent.Status), ref status, value);
            }
        }

        [ModelDefault("AllowEdit", "False")]
        [Browsable(false)]
        public int Type
        {
            get
            {
                return 0;
            }
            set
            {
                SetPropertyValue(nameof(Type), ref type, value);
            }
        }

        [Size(SizeAttribute.Unlimited)]
        [NonPersistent]
        public string ResourceId
        {
            get
            {
                if (string.IsNullOrEmpty(resourceIds))
                {
                    UpdateResourceIds();
                }
                return resourceIds;
            }
            set
            {
                if (resourceIds != value)
                {
                    resourceIds = value;
                    UpdateResources();
                }
            }
        }

        [ModelDefault("AllowEdit", "False")]
        [Browsable(false)]
        public object AppointmentId => Oid;

        public void UpdateResourceIds()
        {
            if (Resources?.Count > 0)
            {
                resourceIds = "<ResourceIds>\r\n";
                foreach (Room resource in Resources)
                {
                    resourceIds += string.Format("<ResourceId Type=\"{0}\" Value=\"{1}\" />\r\n", resource.Id.GetType().FullName, resource.Id);
                }
                resourceIds += "</ResourceIds>";
            }
        }

        public void UpdateResources(bool refreshAllCollection = false)
        {
            Resources.SuspendChangedEvents();
            try
            {
                if (refreshAllCollection)
                {
                    UpdateResourceIds();
                }

                while (Resources.Count > 0)
                {
                    Resources.Remove(Resources[0]);
                }

                if (!String.IsNullOrEmpty(resourceIds))
                {
                    XmlDocument xmlDocument = DevExpress.Utils.SafeXml.CreateDocument(resourceIds);
                    foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
                    {
                        AppointmentResourceIdXmlLoader loader = new AppointmentResourceIdXmlLoader(xmlNode);
                        Object keyMemberValue = loader.ObjectFromXml();
                        Room resource = Session.GetObjectByKey<Room>(new Guid(keyMemberValue.ToString()));
                        if (resource != null)
                        {
                            Resources.Add(resource);
                        }
                    }
                }

            }
            finally
            {
                Resources.ResumeChangedEvents();
            }
        }

    }
}
