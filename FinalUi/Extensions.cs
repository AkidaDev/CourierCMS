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
            return applyCostingRulesOnTransaction(dt, (double)dt.BilledWeight);
        }
        public double applyCostingRulesOnTransaction(RuntimeData trans, double billedWeight)
        {
            if (costingRules == null)
            {
                initializeRules();
            }
            List<CostingRule> RulesApplied = costingRules.Where(x => x.startW <= billedWeight && x.endW >= billedWeight).ToList();
            RulesApplied = RulesApplied.Where(x => x.ServiceList.Contains(trans.Type.Trim())).ToList();

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
                    string zoneCode;
                    if (zone == null)
                        zoneCode = "DEF";
                    else
                        zoneCode = zone.zcode;
                    RulesApplied = RulesApplied.Where(x => x.ZoneList.Contains(zoneCode)).ToList();
                }
            }
            decimal price = 0;
            RulesApplied.ForEach((x) =>
            {
                if (!x.applyRule(trans, billedWeight))
                {
                    CostingRule RulesApplied2 = costingRules.Where(u => u.endW < x.startW).OrderByDescending(z => z.endW).FirstOrDefault();
                    if (RulesApplied2 != null)
                        trans.FrAmount += (decimal)applyCostingRulesOnTransaction(trans, RulesApplied2.endW);
                }

                if (price < trans.FrAmount)
                {
                    price = (decimal)trans.FrAmount;
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
                if (serviceRules == null)
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
            double price = (double)trans.FrAmount;
            List<ServiceRule> RulesApplied = serviceRules.Where(x => x.startW <= trans.BilledWeight && x.endW >= trans.BilledWeight).ToList();
            RulesApplied = RulesApplied.Where(x => x.ServiceList.Contains(trans.Type.Trim())).ToList();

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
                    string zoneCode;
                    if (zone == null)
                        zoneCode = "DEF";
                    else
                        zoneCode = zone.zcode;
                    RulesApplied = RulesApplied.Where(x => x.ZoneList.Contains(zoneCode)).ToList();
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
    public partial class Invoice
    {
        public string CompactInvoiceDetail
        {
            get
            {
                return BillId + "     |     " + ClientCode + "     |     " + TotalAmount;
            }
        }
        public string CompactDate
        {
            get
            {
                return Date.ToString("dd-MMM-yyyy");
            }
        }
    }
    partial class PaymentEntry
    {
        public string CompactDate
        {
            get
            {
                return Date.ToString("dd-MMM-yyyy");
            }
        }
    }
    partial class AccountStatement
    {
        public string CompactDate
        {
            get
            {
                return this.TransactionDate.ToString("dd-MMM-yyyy");
            }
        }
        public string CompactPayAmount
        {
            get
            {
                if (this.PayAmount != null)
                    return String.Format("{0:F2}", this.PayAmount);
                else
                    return "";
            }
        }
        public string CompactRecievedAmount
        {
            get
            {
                if (this.TotalRecievedAmount != null)
                    return String.Format("{0:F2}", this.TotalRecievedAmount);
                else
                    return "";
            }
        }
    }

}
