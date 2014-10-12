using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalUi
{
    class DataSources
    {
        static List<Service> _Services;
        static List<ZONE> _Zones;
        static List<City> _Cities;
        static List<State> _States;
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
        public static List<ZONE> ZoneCopy
        {
            get
            {
                if (_Zones == null)
                    initialize();
                List<ZONE> ZoneCopy = new List<ZONE>();
                ZoneCopy.AddRange(_Zones);
                return ZoneCopy;
            }
        }
        public static List<City> CityCopy
        {
            get
            {
                if (_Cities == null)
                    initialize();
                List<City> CityCopy = new List<City>();
                CityCopy.AddRange(_Cities);
                return CityCopy;
            }
        }
        public static List<State> StateCopy
        {
            get
            {
                if (_States == null)
                    initialize();
                List<State> StatesCopy = new List<State>();
                StatesCopy.AddRange(_States);
                return StatesCopy;
            }
        }
        public static void initialize()
        {
            BillingDataDataContext db = new BillingDataDataContext();
            _Services = db.Services.ToList();
            _Zones = db.ZONEs.ToList();
            _Cities = db.Cities.ToList();
            _States = db.States.ToList();
        }
    }
}
