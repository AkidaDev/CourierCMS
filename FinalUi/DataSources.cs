﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        static List<Stock> _Stocks;
        static List<ServiceGroup> _Groups;
        static List<ServiceGroupAssignment> _ServiceGroupAssignments;
        #endregion
        #region ReadOnlyCopies
        public static List<Stock> StockStatic
        {
            get
            {
                if (_Stocks == null)
                    initialize();
                return _Stocks;
            }
        }
        public static List<ServiceGroup> ServiceGroupStatic
        {
            get
            {
                if (_Groups == null)
                    initialize();
                return _Groups;
            }
        }
        public static List<ServiceGroupAssignment> ServiceGroupAssignStatic
        {
            get
            {
                if (_ServiceGroupAssignments == null)
                    initialize();
                return _ServiceGroupAssignments;
            }
        }
        public static List<Client> ClientsStatic
        {
            get
            {
                if (_Client == null)
                    initialize();
                return _Client;
            }
        }
        public static string ClientNameFromCode(string clcode)
        {
            if (_Client == null)
                initialize();
            Client clnt = _Client.SingleOrDefault(x => x.CLCODE == clcode);
            if (clnt == null)
                return "";
            else
                return clnt.CLNAME;
        }
        public static decimal FuelAmountFromCode(string clcode)
        {
            if (_Client == null)
                initialize();
            Client clnt = _Client.SingleOrDefault(x => x.CLCODE == clcode);
            if (clnt == null)
                return 0;
            else
                return (decimal)(clnt.FUEL ?? 0);
        }
        public static decimal STaxAmountFromCode(string clcode)
        {
            if (_Client == null)
                initialize();
            Client clnt = _Client.SingleOrDefault(x => x.CLCODE == clcode);
            if (clnt == null)
                return 0;
            else
                return (decimal)(clnt.STAX ?? 0);
        }
        #endregion
        #region CopyLists
        public static List<Service> ServicesCopy
        {
            get
            {
                if (_Services == null)
                    initialize();
                List<Service> ServiceCopy = new List<Service>();
                ServiceCopy.AddRange(_Services.OrderBy(x=>x.NameAndCode));
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
                ZoneCopy.AddRange(_Zones.OrderBy(x=>x.NameAndCode));
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
                CityCopy.AddRange(_Cities.OrderBy(x=>x.NameAndCode));
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
                StatesCopy.AddRange(_States.OrderBy(x=>x.NameAndCode));
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
                ClientCopy.AddRange(_Client.OrderBy(x=>x.NameAndCode));
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
                empCopy.AddRange(_Employee.OrderBy(x=>x.Name));
                return empCopy;
            }
        }
        public static List<ServiceGroup> ServiceGroupCopy
        {
            get
            {
                if (_Groups == null)
                    initialize();
                List<ServiceGroup> groupCopy = new List<ServiceGroup>();
                groupCopy.AddRange(_Groups.OrderBy(x => x.GroupName));
                return groupCopy;
            }
        }
        #endregion
        #region Refreshers
        public static void refreshStockList()
        {
            BillingDataDataContext db = new BillingDataDataContext();
            _Stocks = db.Stocks.ToList();
        }
        public static void refreshEmployeeList()
        {
            BillingDataDataContext db = new BillingDataDataContext();
            _Employee = db.Employees.Where(x=>x.Status == 'A').ToList();
        }
        public static void refreshClientList()
        {
            BillingDataDataContext db = new BillingDataDataContext();
            _Client = db.Clients.Where(x=>x.Status == 'A').ToList();
        }
        public static void refreshCityList()
        {
            BillingDataDataContext db = new BillingDataDataContext();
          _Cities = db.Cities.Where(x=>x.Status == 'A').ToList();

        }
        #endregion
        public static void initialize()
        {
            db = new BillingDataDataContext();
            _Services = db.Services.Where(x=>x.SER_TYPE_STATUS == "A").ToList();
            _Zones = db.ZONEs.ToList();
            _States = db.States.ToList();
            _Groups = db.ServiceGroups.ToList();
            _ServiceGroupAssignments = db.ServiceGroupAssignments.ToList();
            refreshCityList();
            refreshClientList();
            refreshEmployeeList();
            refreshStockList();
        }
        public static void  UnloadAllData()
        {
            _Services = null;
            _Zones = null;
            _Stocks = null;
            _States = null;
            _Client = null;
            _Employee = null;
            _Groups = null;
        }
    }
}