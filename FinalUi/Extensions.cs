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
            RulesApplied = RulesApplied.Where(x => x.ServiceList.Contains(trans.Type)).ToList();
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
        private void initializeRules()
        {
            BillingDataDataContext db = new BillingDataDataContext();
            rulesList = db.Rules.Where(x => x.QID == this.Id).ToList();
            costingRules = rulesList.Where(x => x.Type == 1).Select(y => ((new JavaScriptSerializer()).Deserialize<CostingRule>(y.Properties))).ToList();
           // serviceRules = rulesList.Where(x => x.Type == 1).Select(y => ((new JavaScriptSerializer()).Deserialize<ServiceRule>(y.Properties))).ToList();
            //invoiceRule = rulesList.Where(x => x.Type == 3).Select(y => ((new JavaScriptSerializer()).Deserialize<InvoiceRule>(y.Properties))).ToList();
        }
        public double applyServiceRulesOnTransaction(double billedWeight, string destination, string serviceCode, char DOX, char cost)
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
            double price = 0;
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
                    if (x.type == 'S')
                    {
                        /*
                         * work to be done not all fields are available right now
                         * */
                    }
                    if(x.type == 'W')
                    {
                        if (x.mode == 'A')
                            price += x.amount;
                        else
                            price += (price * 0);
                    }
                });
            return price;
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
                return "<" + this.CLCODE + ">" + this.CLNAME;
            }
        }
    }
    public partial class Service
    {
        public string NameAndCOde
        {
            get
            {
                return this.SER_CODE + ":" + this.SER_DESC;
            }
        }
    }
    public partial class ZONE
    {
        public string NameAndCode
        {
            get
            {
                return this.zcode + ":" + this.Zone_name;
            }
        }
    }
    public partial class State
    {
        public string NameAndCode
        {
            get
            {
                return this.STATE_CODE + ":" + this.STATE_DESC;
            }
        }
    }
    public partial class City
    {
        public string NameAndCode
        {
            get
            {
                return this.CITY_CODE + ":" + this.CITY_DESC;
            }
        }
    }

}
