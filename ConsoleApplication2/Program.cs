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
            DataClasses1DataContext db = new DataClasses1DataContext();
            List<Rule> AllRules = db.Rules.Where(x => x.Type == 1).ToList();
            JavaScriptSerializer jsS = new JavaScriptSerializer();
            int i = 0;
            Console.WriteLine("Total record to be processed: {0}",AllRules.Count);
            foreach(Rule rule in AllRules)
            {
                i++;
                CostingRule cRule = jsS.Deserialize<CostingRule>(rule.Properties);
                if(cRule.type == 'S')
                {
                    if(cRule.startW > 0.49 && cRule.startW < 0.51)
                    {
                        cRule.ndStartValue = 0;
                        cRule.dStartValue = 0;
                        
                    }
                    rule.Properties = jsS.Serialize(cRule);
                }
                if(i%250 == 0)
                    Console.WriteLine("Processed records: {0}",i);

            }
            Console.WriteLine("Trying to submit changes ..{0}",db.GetChangeSet().Updates.Count);
            db.SubmitChanges();
        }
    }
}
