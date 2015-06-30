using System;
using System.Collections.Generic;
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
    /// Interaction logic for DeleteConnsignment.xaml
    /// </summary>
    public partial class DeleteConnsignment : Window
    {
        public DeleteConnsignment()
        {
            InitializeComponent();
        }

        private void AddZoneButton_Click(object sender, RoutedEventArgs e)
        {
            BillingDataDataContext db = new BillingDataDataContext();
            List<Transaction> trans = db.Transactions.Where(x => x.ConnsignmentNo == Zonecodebox.Text).ToList();
            List<RuntimeData> runT = db.RuntimeDatas.Where(x => x.ConsignmentNo == Zonecodebox.Text).ToList();
            int userCount = runT.Select(x => x.UserId).Distinct().Count();
            int sheetCount = runT.Select(x => x.SheetNo.ToString() + x.UserId).Distinct().Count();
            if(MessageBox.Show("Transaction is loaded in " + sheetCount.ToString() + " sheets for " + userCount.ToString() + " users. Vortext must be restarted for changes to take effect. Continue?","Confirmation",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                db.RuntimeDatas.DeleteAllOnSubmit(runT);
                db.Transactions.DeleteAllOnSubmit(trans);
                try
                {
                    db.SubmitChanges();
                    MessageBox.Show("Deleted successfully.", "Information");
                }
                catch(Exception)
                {
                    MessageBox.Show("Unable to delete transactions.", "Error");
                }
            }

        }
    }
}
