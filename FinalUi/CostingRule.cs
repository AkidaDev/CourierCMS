using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace FinalUi
{
    /// <summary>
    /// Class for storing CostingRule
    /// PropertyString = {"What":{"DoxAmount":value(float),"NonDoxAmount"=value(float),"Type":R/S,"StepWeight":value,"DoxStartValue":value,"NonDoxStartValue":value}},
    /// };When={{ServiceIs={S1,S2,..SN}},{ZoneIs={}},{CityIs={}},{Weight={From={},To={}}}}
    /// </summary>
    public class CostingRule : IRule
    {
        public int Id { get; set; }

        #region When to apply
        public List<string> ServiceGroupList { get; set; }
        public List<string> ServiceList { get; set; }
        public List<string> ZoneList { get; set; }
        public List<string> CityList { get; set; }
        public List<string> StateList { get; set; }
        public double startW { get; set; }
        public double endW { get; set; }
        public bool fromstart { get; set; }
        #endregion

        #region What to apply
        public char type { get; set; }
        public double doxAmount { get; set; }
        public double ndoxAmount { get; set; }
        public double stepWeight { get; set; }
        public double dStartValue { get; set; }
        public double ndStartValue { get; set; }
        #endregion

        #region Constructorss
        public CostingRule()
            : base()
        {
            BillingDataDataContext db = new BillingDataDataContext();
        }
        public CostingRule(string propertyString)
            : base()
        {
            CostingRule rule = (new JavaScriptSerializer()).Deserialize<CostingRule>(propertyString);
            this.CityList = rule.CityList;
            this.doxAmount = rule.doxAmount;
            this.dStartValue = rule.dStartValue;
            this.endW = rule.endW;
            this.ndoxAmount = rule.ndoxAmount;
            this.ndStartValue = rule.ndStartValue;
            this.ServiceList = rule.ServiceList;
            this.startW = rule.startW;
            this.StateList = rule.StateList;
            this.stepWeight = rule.stepWeight;
            this.type = rule.type;
            this.ZoneList = rule.ZoneList;
        }
        public void decodeString()
        {
        }
        public CostingRule(List<string> ServiceList, List<string> ZoneList, List<string> CityList, List<string> StateList, double startW, double endW, char type, double doxAmount, double ndoxAmount)
        {
            this.ServiceList = ServiceList;
            this.ZoneList = ZoneList;
            this.CityList = CityList;
            this.StateList = StateList;
            this.startW = startW;
            this.endW = endW;
            this.type = type;
            this.doxAmount = doxAmount;
            this.ndoxAmount = ndoxAmount;
        }
        public CostingRule(List<string> ServiceList, List<string> ZoneList, List<string> CityList, List<string> StateList, double startW, double endW, char type, double doxAmount, double ndoxAmount, double stepWeight, double ndStartValue, double dStartValue)
            : this(ServiceList, ZoneList, CityList, StateList, startW, endW, type, doxAmount, ndoxAmount)
        {
            this.stepWeight = stepWeight;
            this.dStartValue = dStartValue;
            this.ndStartValue = ndStartValue;
        }
        #endregion
        #region Reporting properties
        public string rateTypeReporting
        {
            get
            {
                if (type == 'S')
                    return "Additional";
                if (type == 'M')
                    return "Bulk";
                if (type == 'R')
                    return "Range";
                return "";
            }
        }
       
        public string zoneListReporting
        {
            get
            {
                string zones;
                zones = "";

                if (CityList != null)
                {
                    CityList.ForEach(x =>
                    {
                        zones += x + " ";
                    });
                }
                if (StateList != null)
                {
                    StateList.ForEach(x =>
                    {
                        zones += x + " ";
                    });
                }
                if (ZoneList != null)
                {
                    ZoneList.ForEach(x =>
                    {
                        zones += x + " ";
                    });
                }

                return zones;
            }
        }
        public string serviceGroupReporting
        {
            get
            {
                string groups = "";
                if (ServiceGroupList != null)
                {
                    ServiceGroupList.ForEach(x =>
                        {
                            groups += x + " ";
                        });
                }
                groups = groups + " " + serviceListReporting;
                return groups;
            }
        }
        public string serviceListReporting
        {
            get
            {
                string services = "";

                if (ServiceList != null)
                {
                    ServiceList.ForEach(x =>
                    {
                        services += x + " ";
                    });
                }
                return services;
            }
        }
        public int serviceCount
        {
            get
            {
                if (ServiceList != null)
                    return ServiceList.Count;
                else
                    return 0;
            }
        }
        public int zoneCount
        {
            get
            {
                if (ZoneList != null)
                    return ZoneList.Count;
                else
                    return 0;
            }
        }
        public int CityCount
        {
            get
            {
                if (CityList != null)
                    return CityList.Count;
                else
                    return 0;
            }
        }
        public int StateCount
        {
            get
            {
                if (StateList != null)
                    return StateList.Count;
                else
                    return 0;
            }
        }
        public string range
        {
            get
            {
                return startW.ToString() + " to " + endW.ToString();
            }
        }
        public string StepS
        {
            get
            {
                if (type == 'R')
                    return "N/A";
                else
                    return stepWeight.ToString();
            }
        }
        public string DoxStartS
        {
            get
            {
                if (type == 'R')
                    return "N/A";
                else
                    return dStartValue.ToString();
            }
        }
        public string nDoxStartS
        {
            get
            {
                if (type == 'R')
                    return "N/A";
                else
                    return ndStartValue.ToString();
            }
        }
        #endregion
        public bool applyRule(object obj, double billedWeight)
        {
            RuntimeData trans = (RuntimeData)obj;
            if (type == 'R')
            {
                if (char.ToUpper((char)trans.DOX) == 'D')
                {
                    trans.FrAmount = Convert.ToDecimal(doxAmount);
                    return true;
                }
                else
                    trans.FrAmount = Convert.ToDecimal(ndoxAmount);
                return true;
            }
            if (type == 'S')
            {
                double multiAmount, startAmount;
                if (char.ToUpper((char)trans.DOX) == 'D')
                {
                    multiAmount = doxAmount;
                    startAmount = dStartValue;
                }
                else
                {
                    multiAmount = ndoxAmount;
                    startAmount = ndStartValue;
                }
                int steps = 0;
                double restWeight = billedWeight - startW;
                steps = (int)(restWeight / stepWeight) + 1;
                trans.FrAmount = (decimal)(startAmount + steps * multiAmount);
                return false;
            }
            if (type == 'M')
            {
                double multiAmount;
                if (char.ToUpper((char)trans.DOX) == 'D')
                {
                    multiAmount = doxAmount;
                }
                else
                {
                    multiAmount = ndoxAmount;
                }
                int steps = 0;
                steps = (int)Math.Ceiling((decimal)(billedWeight / stepWeight));
                trans.FrAmount = (decimal)(steps * multiAmount);
                return true;
            }
            return false;
        }
        public CostingRule getCostingRule()
        {
            return new CostingRule();
        }
        public void encodeString()
        {
        }
        #region whereRules
        #endregion
    }
}
