using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace FinalUi
{
  static class LoadResources
    {
      public  static int hey;
      static LoadResources()
        {
            if(Configs.Default.IsFreshOrReset == true)
            {
                Setup window = new Setup();
                window.ShowDialog();
            }
        }
        public static string getConString()
        {
            string con = Configs.Default.BillingDatabaseConnectionString;
             con = Configs.Default.BillingDatabaseConnectionString;
            //MessageBox.Show(con);
            return con;
        }
    }
}
