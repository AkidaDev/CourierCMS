using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalUi
{
    /// <summary>
    /// Class for storing CostingRule
    /// PropertyString = {"What":{"DoxAmount":value(float),"NonDoxAmount"=value(float),"Type":R/S,"StepWeight":value,"DoxStartValue":value,"NonDoxStartValue":value}},
    /// };When={{ServiceIs={S1,S2,..SN}},{ZoneIs={}},{CityIs={}},{Weight={From={},To={}}}}
    /// </summary>
    class CostingRule:Rule,IRule
    {
        public void decodeString()
        {}

        #region State
        #endregion

        #region When to apply
        public List<string> ServiceList;
        public List<string> ZoneList;
        public List<string> CityList;
        public List<string> StateList;
        public  double startW;
        public double endW;
        #endregion

        #region What to apply
        char type;
        double doxAmount;
        double ndoxAmount;
        double stepWeight;
        double dStartValue;
        double ndStartValue;
        #endregion

        #region Constructors
        public CostingRule(string propertyString)
            : base()
        {
            this.Properties = propertyString;
        }
        public CostingRule(List<string> ServiceList, List<string> ZoneList, List<string> CityList,List<string> StateList, double startW, double endW, char type, double doxAmount, double ndoxAmount)
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
        public CostingRule(List<string> ServiceList, List<string> ZoneList, List<string> CityList,List<string> StateList, double startW, double endW, char type, double doxAmount, double ndoxAmount, double stepWeight, double ndStartValue, double dStartValue)
            : this(ServiceList, ZoneList, CityList,StateList, startW, endW, type, doxAmount, ndoxAmount)
        {
            this.stepWeight = stepWeight;
            this.dStartValue = dStartValue;
            this.ndStartValue = ndStartValue;
        }
        #endregion

        public override void applyRule(object obj)
        {
            base.applyRule(obj);
            RuntimeData trans = (RuntimeData)obj;
            if(type == 'R')
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
        {}
        #region whereRules
        #endregion
    }
}
