using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalUi
{
    public partial class RuleContainer: Quotation
    {
        List<Rule> rulesList;
        List<CostingRule> costingRules;
        
        Rule lastCostingRuleApplied;
        Rule lastServiceRuleApplied;
        Rule lastInvoiceRuleApplied;
        
        public void applyCostingRulesOnTransaction(RuntimeData trans)
        {   
            if(costingRules == null)
            {
                initializeRules();
            }
            List<CostingRule> RulesApplied = costingRules.Where(x => x.startW <= trans.BilledWeight && x.endW >= trans.BilledWeight).ToList();
            if(RulesApplied.Where(x=>x.CityList.Contains(trans.Destination)).Count() > 0)
            {
                RulesApplied = RulesApplied.Where(x => x.CityList.Contains(trans.Destination)).ToList();
            }
            else
            {
                State state = UtilityClass.getStateFromCity(trans.Destination);
                if(RulesApplied.Where(x=>x.StateList.Contains(state.STATE_CODE)).Count() >0)
                {
                    RulesApplied = RulesApplied.Where(x => x.StateList.Contains(state.STATE_CODE)).ToList();
                }
                else
                {
                    ZONE zone = UtilityClass.getZoneFromCityCode(trans.Destination);
                    if(RulesApplied.Where(x=>x.ZoneList.Contains(zone.zcode)).Count() > 0 )
                    {
                        RulesApplied = RulesApplied.Where(x => x.ZoneList.Contains(zone.zcode)).ToList();
                    }
                }
            }
            RulesApplied = RulesApplied.Where(x => x.ServiceList.Contains(trans.Type)).ToList();
            decimal price = 0;
            RulesApplied.ForEach((x) => {
                x.applyRule(trans);
                if (price < trans.FrAmount)
                {
                    price = (decimal)trans.FrAmount;
                    lastCostingRuleApplied = x;
                }
            });
            trans.FrAmount = price;
        }

        private void initializeRules()
        {
            BillingDataDataContext db = new BillingDataDataContext();
            rulesList = db.Rules.Where(x => x.QID == this.Id).ToList() ;
            costingRules = rulesList.Where(x => x.Type == 1).Cast<CostingRule>().ToList();

        }
        public void applyServiceRulesOnTransaction(Transaction trans)
        {}
    }
    class Extensions
    {
        public Rate rate { get; set; }
        public List<RateDetail> rateDetails {get; set;}
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
    public partial class Rule
    {
       public virtual void applyRule(Object obj)
        {  }
    }
}
