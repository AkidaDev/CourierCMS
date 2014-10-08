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
        public static string getConString()
        {
            string con = "Data Source=" + System.Environment.MachineName + ";Initial Catalog=BillingDatabase;Persist Security Info=True;User ID=sa;Password=Alver!22";
            return con;
        }
    }
}
