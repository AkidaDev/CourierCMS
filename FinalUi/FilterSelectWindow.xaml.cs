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
        CollectionViewSource StartConnNoList;
        CollectionViewSource EndConnoList;
     

        public List<Func<RuntimeData, int,bool>> filters;
        public FilterSelectWindow(IEnumerable<string> ConnNo):this()
        {
            filters = new List<Func<RuntimeData,int, bool>>();
            StartConnNoList = (CollectionViewSource)FindResource("StartConnNoList");
            StartConnNoList.Source = ConnNo;
            EndConnoList = (CollectionViewSource)FindResource("EndConnoList");
            EndConnoList.Source = ConnNo;
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
            if(EndConnNo.SelectedIndex > StartConnNo.SelectedIndex)
            {
                Func<RuntimeData, int, bool> filter = (o, i) => i < EndConnNo.SelectedIndex && i > StartConnNo.SelectedIndex;
                filters.Add(filter);
            }
        } 

        private void CheckBox_SelectClient_Checked(object sender, RoutedEventArgs e)
        {
           
            CheckBox thisCheckBox = (CheckBox)sender;
            if ((bool)thisCheckBox.IsChecked)
            {
                SelectedClientList.Add((string)thisCheckBox.Tag);
            }
        }
        private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DragthisWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
