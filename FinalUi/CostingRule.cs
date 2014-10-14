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
        public void decodeString()
        { }

        #region When to apply
        public List<string> ServiceList { get; set; }
        public List<string> ZoneList { get; set; }
        public List<string> CityList { get; set; }
        public List<string> StateList { get; set; }
        public double startW { get; set; }
        public double endW { get; set; }
        #endregion

        #region What to apply
        public char type { get; set; }
        public double doxAmount {get; set;}
        public double ndoxAmount{get; set;}
        public double stepWeight {get; set;}
        public double dStartValue{get; set;}
        public double ndStartValue {get; set;}
        #endregion

        #region Constructorss
        public CostingRule()
            : base()
        { }
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
        public void applyRule(object obj)
        {
            RuntimeData trans = (RuntimeData)obj;
            if (type == 'R')
            {
                if (char.ToUpper((char)trans.DOX) == 'D')
                {
                    trans.FrAmount = Convert.ToDecimal(doxAmount);
                }
                else
                    trans.FrAmount = Convert.ToDecimal(ndoxAmount);
            }
            else
            {
                if (char.ToUpper((char)trans.DOX) == 'D')
                    trans.FrAmount = (decimal)(dStartValue + ((trans.BilledWeight - startW) / stepWeight) * doxAmount);
                else
                    trans.FrAmount = (decimal)(ndStartValue + ((trans.BilledWeight - startW) / stepWeight) * ndoxAmount);
            }
        }
        public void encodeString()
        {
        }
        #region whereRules
        #endregion
    }
}
