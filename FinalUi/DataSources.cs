using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalUi
{
    class DataSources
    {
        #region OrigLists
        static List<Service> _Services;
        static List<ZONE> _Zones;
        static List<City> _Cities;
        static List<State> _States;
        static List<Client> _Client;
        static List<Employee> _Employee;
        static BillingDataDataContext db;
        #endregion
        #region CopyLists
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
        public static List<Client> ClientCopy
        {
            get
            {
                if (_Client == null)
                    initialize();
                List<Client> ClientCopy = new List<Client>();
                ClientCopy.AddRange(_Client);
                return ClientCopy;
            }
        }
        public static List<Employee> EmployeeCopy
        {
            get
            {
                if(_Employee == null)
                {
                    initialize();
                }
                List<Employee> empCopy = new List<Employee>();
                empCopy.AddRange(_Employee);
                return empCopy;
            }
        }
        #endregion
        #region Refreshers
        public static void refreshEmployeeList()
        {
            BillingDataDataContext db = new BillingDataDataContext();
          
           // _Employee = db.Employees.Where(x=>x.Status == 'A').ToList();
            _Employee = db.Employees.ToList();
        }
        public static void refreshClientList()
        {
            BillingDataDataContext db = new BillingDataDataContext();

           // _Client = db.Clients.Where(x=>x.Status == 'A').ToList();
            _Client = db.Clients.ToList();
        }
        public static void refreshCityList()
        {
            BillingDataDataContext db = new BillingDataDataContext();
          //  _Cities = db.Cities.Where(x=>x.Status == 'A').ToList();
            _Cities = db.Cities.ToList();

        }
        #endregion
        public static void initialize()
        {
            db = new BillingDataDataContext();
            _Services = db.Services.ToList();
            _Zones = db.ZONEs.ToList();
            _States = db.States.ToList();
            refreshCityList();
            refreshClientList();
            refreshEmployeeList();
        }
    }
}
