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
      static LoadResources()
        {
            if(Configs.Default.IsFreshOrReset == true)
            {
                Setup window = new Setup();
                window.ShowDialog();
            }
            AppDomain currentDomain = AppDomain.CurrentDomain;
        //     currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);
            try
            {
                //  throw new Exception("1");
            }
            catch (Exception e)
            {
                //  Console.WriteLine("Catch clause caught : {0} \n", e.Message);
            }
        }
      static void MyHandler(object sender, UnhandledExceptionEventArgs args)
          {
              Exception e = (Exception)args.ExceptionObject;
              MessageBox.Show("Vortex Crashed. You can help us by contacting with message mentioned under \n Error Message: " + e.Message + "\n Application will be restarting now", "Oops!!");
              System.Windows.Forms.Application.Restart();
          }
      public static string getConString()
        {
            string con = Configs.Default.BillingDatabaseConnectionString;
             con = Configs.Default.BillingDatabaseConnectionString;
            //MessageBox.Show(con);
            return con;
        }
      public static void getLatestVer()
      {
      }
    }
}