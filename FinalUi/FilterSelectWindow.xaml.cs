using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for FilterSelectWindow.xaml
    /// </summary>
    public partial class FilterSelectWindow : Window
    {
        CollectionViewSource clientList;
        List<String> SelectedClientList;
        CollectionViewSource ConnNoList;
        public FilterSelectWindow(IEnumerable<string> ConnNo):this()
        {
            ConnNoList = new CollectionViewSource();
            ConnNoList.Source = ConnNo;
        }
         FilterSelectWindow()
        {
            SelectedClientList = new List<string>();
            InitializeComponent();
            
            BillingDataDataContext db = new BillingDataDataContext();
            clientList = (CollectionViewSource)MainGrid.FindResource("ClientList");
            clientList.Source = db.Clients.ToList();
        }

        private void GetFilter_Click(object sender, RoutedEventArgs e)
        {
        } 

        private void CheckBox_SelectClient_Checked(object sender, RoutedEventArgs e)
        {
           
            CheckBox thisCheckBox = (CheckBox)sender;
            if ((bool)thisCheckBox.IsChecked)
            {
                SelectedClientList.Add((string)thisCheckBox.Tag);
            }
        }
    }
}
