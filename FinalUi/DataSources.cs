using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalUi
{
    class DataSources
    {
        static List<Service> _Services;
        public static List<Service> ServicesCopy
        {
            get
            {
                if (_Services == null)
                    initialize();
                List<Service> ServiceCopy = new List<Service>();
                ServiceCopy.AddRange(_Services);
                return ServiceCopy;
            }
        }
     
        public static void initialize()
        {
            BillingDataDataContext db = new BillingDataDataContext();
            _Services = db.Services.ToList();
        }
    }
}
