using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace FinalUi
{
    public partial class Quotation
    {
        List<Rule> rulesList;
        List<CostingRule> costingRules;
        List<ServiceRule> serviceRules;
        List<InvoiceRule> invoiceRule;
        Rule lastCostingRuleApplied;
        public double applyCostingRulesOnTransaction(double billedWeight, string destination, string serviceCode, char DOX)
        {
            RuntimeData dt = new RuntimeData();
            dt.BilledWeight = billedWeight;
            dt.Destination = destination;
            dt.Type = serviceCode;
            dt.DOX = DOX;
            return applyCostingRulesOnTransaction(dt);
        }
        public double applyCostingRulesOnTransaction(RuntimeData trans)
        {
            if (costingRules == null)
            {
                initializeRules();
            }
            List<CostingRule> RulesApplied = costingRules.Where(x => x.startW <= trans.BilledWeight && x.endW >= trans.BilledWeight).ToList();
            if (RulesApplied.Where(x => x.CityList.Contains(trans.Destination)).Count() > 0)
            {
                RulesApplied = RulesApplied.Where(x => x.CityList.Contains(trans.Destination)).ToList();
            }
            else
            {
                State state = UtilityClass.getStateFromCity(trans.Destination);
                if (RulesApplied.Where(x => x.StateList.Contains(state.STATE_CODE)).Count() > 0)
                {
                    RulesApplied = RulesApplied.Where(x => x.StateList.Contains(state.STATE_CODE)).ToList();
                }
                else
                {
                    ZONE zone = UtilityClass.getZoneFromCityCode(trans.Destination);
                    if (RulesApplied.Where(x => x.ZoneList.Contains(zone.zcode)).Count() > 0)
                    {
                        RulesApplied = RulesApplied.Where(x => x.ZoneList.Contains(zone.zcode)).ToList();
                    }
                }
            }
            RulesApplied = RulesApplied.Where(x => x.ServiceList.Contains(trans.Type.Trim())).ToList();
            decimal price = 0;
            RulesApplied.ForEach((x) =>
            {
                x.applyRule(trans);
                if (price < trans.FrAmount)
                {
                    price = (decimal)trans.FrAmount;
                    lastCostingRuleApplied = null;
                }
            });
            return Convert.ToDouble(price);
        }

        public List<CostingRule> CostingRules
        {
            get
            {
                if (costingRules == null)
                {
                    initializeRules();
                }
                return costingRules;
            }
        }
        public List<ServiceRule> ServiceRules
        {
            get
            {
                if(serviceRules == null)
                {
                    initializeRules();
                }
                return serviceRules;
            }
        }
        private void initializeRules()
        {
            BillingDataDataContext db = new BillingDataDataContext();
            rulesList = db.Rules.Where(x => x.QID == this.Id).ToList();
            costingRules = rulesList.Where(x => x.Type == 1).Select(y => ((new JavaScriptSerializer()).Deserialize<CostingRule>(y.Properties))).ToList();
          serviceRules = rulesList.Where(x => x.Type == 2).Select(y => ((new JavaScriptSerializer()).Deserialize<ServiceRule>(y.Properties))).ToList();
            //invoiceRule = rulesList.Where(x => x.Type == 3).Select(y => ((new JavaScriptSerializer()).Deserialize<InvoiceRule>(y.Properties))).ToList();
        }
        public double applyServiceRulesOnTransaction(double billedWeight, string destination, string serviceCode, char DOX, decimal cost)
        {
            RuntimeData dt = new RuntimeData();
            dt.BilledWeight = billedWeight;
            dt.Destination = destination;
            dt.Type = serviceCode;
            dt.DOX = DOX;
            dt.FrAmount = cost;
            return applyServiceRulesOnTransaction(dt);
        }
        public double applyServiceRulesOnTransaction(RuntimeData trans)
        {
            double price = (double)trans.FrAmount ;
            List<ServiceRule> RulesApplied = serviceRules.Where(x => x.startW <= trans.BilledWeight && x.endW >= trans.BilledWeight).ToList();

            if (RulesApplied.Where(x => x.CityList.Contains(trans.Destination)).Count() > 0)
            {
                RulesApplied = RulesApplied.Where(x => x.CityList.Contains(trans.Destination)).ToList();
            }
            else
            {
                State state = UtilityClass.getStateFromCity(trans.Destination);
                if (RulesApplied.Where(x => x.StateList.Contains(state.STATE_CODE)).Count() > 0)
                {
                    RulesApplied = RulesApplied.Where(x => x.StateList.Contains(state.STATE_CODE)).ToList();
                }
                else
                {
                    ZONE zone = UtilityClass.getZoneFromCityCode(trans.Destination);
                    if (RulesApplied.Where(x => x.ZoneList.Contains(zone.zcode)).Count() > 0)
                    {
                        RulesApplied = RulesApplied.Where(x => x.ZoneList.Contains(zone.zcode)).ToList();
                    }
                }
            }

            RulesApplied.Where(x => x.applicable == 'O').ToList().ForEach((x) =>
                {
                    x.applyRule(trans, price);
                });
            return (double)trans.FrAmount;
        }
    }
    class Extensions
    {
        public Rate rate { get; set; }
        public List<RateDetail> rateDetails { get; set; }
        public Client client { get; set; }

    }
    public partial class Client
    {
        public string NameAndCode
        {
            get
            {
                return this.CLNAME + " (" + this.CLCODE + ")";
            }
        }
    }
    public partial class Service
    {
        public string NameAndCode
        {
            get
            {
                return this.SER_DESC + " (" + this.SER_CODE + ")";
            }
        }
    }
    public partial class ZONE
    {
        public string NameAndCode
        {
            get
            {
                return  this.Zone_name + " (" + this.zcode + ")";
            }
        }
    }
    public partial class State
    {
        public string NameAndCode
        {
            get
            {
                return this.STATE_DESC + " (" + this.STATE_CODE +")";
            }
        }
    }
    public partial class City
    {
        public string NameAndCode
        {
            get
            {
                return this.CITY_DESC + " (" + this.CITY_CODE + ")";
            }
        }
    }

}
