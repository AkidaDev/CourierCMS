using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalUi;
using System.Web;
using System.Web.Script.Serialization;
namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            DataClasses1DataContext d1 = new DataClasses1DataContext();
            DataClasses2DataContext d2 = new DataClasses2DataContext();
            Console.WriteLine("Attempting to load data from mi database....");
            List<DATAENTRY> dataEntries = d2.ExecuteQuery<DATAENTRY>(@"
                SELECT [AWBNO]
      ,[BKDATE]
      ,[DEST]
      ,[CLCODE]
      ,[CONSIGNEE]
      ,[CNEADDRESS]
      ,d.[TYPE]
      ,[BMODE]
      ,[MMODE]
      ,[PKGS]
      ,[BWT]
      ,[MWT]
      ,[PMODE]
      ,[AMOUNT]
      ,[POD_DATE]
      ,[POD_TIME]
      ,[STATUS]
      ,[REMARKS]
      ,[USERNAME]
      ,[BILLNO]
      ,[network]
      ,d.[destination]
      ,[clname]
      ,[tdesc]
      ,[tamt]
      ,[ntdesc]
      ,[ntamt]
      ,[fname]
      ,[famt]
      ,[fcode]
      ,[adsns]
      ,[state]
      ,[NETNO]
      ,[dlvboy]
      ,[mainbilno]
      ,[BRANCH]
      ,[MFNO]
      ,[MFDATE]
      ,[MFWD]
      ,[Actualdate]
      ,[upload]
      ,[Shpchrg]
      ,[dtdamt]
      ,[zone]
  FROM [mi].[dbo].[DATAENTRY] d 
  join [BillingDatabase].dbo.[Transaction] t 
  on t.ConnsignmentNo COLLATE DATABASE_DEFAULT = d.AWBNO COLLATE DATABASE_DEFAULT

            ").ToList();
            Console.WriteLine("Total records loaded : {0}", dataEntries.Count);
            Console.WriteLine("Attempting to load data from BillingDatabase...");
            List<Transaction> transactions = d1.ExecuteQuery<Transaction>(@"
    SELECT [ID]
      ,[AmountPayed]
      ,[AmountCharged]
      ,[ConnsignmentNo]
      ,[Weight]
      ,[WeightByFranchize]
      ,t.[Destination]
      ,[DestinationPin]
      ,[UserId]
      ,[BookingDate]
      ,[AddDate]
      ,[LastModified]
      ,t.[Type]
      ,[Mode]
      ,[DOX]
      ,[ServiceTax]
      ,[SplDisc]
      ,[InvoiceNo]
      ,[InvoiceDate]
      ,[CustCode]
      ,[TransMF_No]
      ,[BilledWeight]
      ,[ConsigneeName]
      ,[ConsigneeAddress]
      ,[ConsignerName]
      ,[ConsignerAddress]
      ,[SubClient]
      ,[DeliveryStatus]
  FROM [BillingDatabase].[dbo].[Transaction] t
  join mi.dbo.DATAENTRY d
  on
  d.AWBNO COLLATE DATABASE_DEFAULT = t.ConnsignmentNo COLLATE DATABASE_DEFAULT
").ToList();
            Console.WriteLine("Total records loaded from BillingDatabase: {0} ", transactions.Count);
            List<String> clients = d1.Clients.Select(x => x.CLCODE).ToList();
            Console.WriteLine("Attempting to modify data..... ");
            int i = 0;
            foreach (Transaction transaction in transactions)
            {
                DATAENTRY dataentry = dataEntries.SingleOrDefault(x => x.AWBNO == transaction.ConnsignmentNo);
                if (dataentry != null)
                {
                    if (transaction.Destination != null && transaction.Destination != "")
                    {
                        try
                        {
                            switch (dataentry.CLCODE.Trim())
                            {
                                case "REKR":
                                    transaction.CustCode = "MVHS";
                                    break;
                                case "SUNY":
                                    transaction.CustCode = "suny";
                                    break;
                                case "DLFC":
                                    transaction.CustCode = "DISHL";
                                    break;
                                case "TVLS":
                                    transaction.CustCode = "TRVL";
                                    break;
                                case "OFCE":
                                    transaction.CustCode = "<NONE>";
                                    break;
                                case "HYMI":
                                    transaction.CustCode = "HONY";
                                    break;
                                case "TREF":
                                    transaction.CustCode = "TREF ";
                                    break;
                                case "MANJ":
                                    transaction.CustCode = "mnoj";
                                    break;
                                default:
                                    if (clients.Contains(dataentry.CLCODE))
                                        transaction.CustCode = dataentry.CLCODE;
                                    else
                                        throw new Exception("No client found.. " + dataentry.CLCODE);
                                    break;
                            }

                            transaction.BilledWeight = dataentry.BWT;
                            transaction.AmountCharged = (decimal)UtilityClass.getCost(transaction.CustCode, transaction.BilledWeight ?? 0.0, transaction.Destination, transaction.Type, transaction.DOX);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(transaction.ConnsignmentNo + " : " + e.Message);
                        }
                    }
                    else
                    {
                        Console.WriteLine(transaction.ConnsignmentNo + " : " + "Cannot find destination ");
                    }
                }

                else
                    Console.WriteLine(transaction.ConnsignmentNo + " : " + "Cannot find this transaction in database");
                i++;
                if (i % 250 == 0)
                    Console.WriteLine(">>" + i.ToString() + "Records done....");
            }
            Console.WriteLine("Total records to be affected: " + d1.GetChangeSet().Updates.Count);
            Console.WriteLine("Attempting to save records.... ");
            d1.SubmitChanges();
            Console.WriteLine("Changes done.....");
        }

        static void mayUseThisFuncLater() //func to change multiplier to step
        {
            DataClasses1DataContext db = new DataClasses1DataContext();
            List<Rule> AllRules = db.Rules.Where(x => x.Type == 1).ToList();
            JavaScriptSerializer jsS = new JavaScriptSerializer();
            int i = 0;
            Console.WriteLine("Total record to be processed: {0}", AllRules.Count);
            foreach (Rule rule in AllRules)
            {
                i++;
                CostingRule cRule = jsS.Deserialize<CostingRule>(rule.Properties);
                if (cRule.type == 'S')
                {
                    if (cRule.startW > 0.49 && cRule.startW < 0.51)
                    {
                        cRule.ndStartValue = 0;
                        cRule.dStartValue = 0;

                    }
                    rule.Properties = jsS.Serialize(cRule);
                }
                if (i % 250 == 0)
                    Console.WriteLine("Processed records: {0}", i);

            }
            Console.WriteLine("Trying to submit changes ..{0}", db.GetChangeSet().Updates.Count);
            db.SubmitChanges();
        }
    }
}
