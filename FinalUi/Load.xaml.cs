using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FinalUi
{
    /// <summary>
    /// Interaction logic for Load.xaml
    /// </summary>
    public partial class Load : Window
    {
        public Load()
        {
         //   MessageBox.Show("Con String :" + Configs.Default.BillingDatabaseConnectionString);
            if(Configs.Default.IsFirst)
            {
                Setup win = new Setup();
                win.Show();
            }
            else
            { 
                string provider = "System.Data.SqlClient"; // for example
                DbProviderFactory factory = DbProviderFactories.GetFactory(provider);
                using (DbConnection conn = factory.CreateConnection())
                {
                    conn.ConnectionString = Configs.Default.BillingDatabaseConnectionString;
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Unable to connect to server. Please Contact System Administrator"); Configs.Default.IsFirst = true; Configs.Default.Save(); System.Windows.Forms.Application.Restart(); Application.Current.Shutdown();
                    }
                }
                Login win = new Login();
                win.Show();
            }
            this.Close();
        }
    }
}