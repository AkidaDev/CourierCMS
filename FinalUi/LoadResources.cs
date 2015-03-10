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
            AppDomain currentDomain = AppDomain.CurrentDomain;
           currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);
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
    }
}