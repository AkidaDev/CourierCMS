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
        CollectionViewSource clientListToSet;
        CollectionContainer clientListToAdd;
        CollectionViewSource StartConnNoList;
        CollectionViewSource EndConnoList;
        Filter filterObj;

        public List<Func<RuntimeData, int,bool>> filters;
        /// <summary>
        /// Creates a new Filter Selection Window
        /// </summary>
        /// <param name="filterObj">Filter Object of current context</param>
        /// <param name="ConnNo">Connsignment Number List of current object</param>
        public FilterSelectWindow(Filter filterObj, IEnumerable<string> ConnNo)
        {
            setDataSource(filterObj, ConnNo);
          //  List<WrapPanel> panels = (List<WrapPanel>)SelectClient.ItemsSource;
        }
        public void setDataSource(Filter filterObj, IEnumerable<string> ConnNo)
        {
            InitializeComponent();
            this.filterObj = filterObj;
            BillingDataDataContext db = new BillingDataDataContext();
            
            StartConnNoList = (CollectionViewSource)FindResource("StartConnNoList");
            StartConnNoList.Source = ConnNo;
            EndConnoList = (CollectionViewSource)FindResource("EndConnoList");
            EndConnoList.Source = ConnNo;
        }

        private void GetFilter_Click(object sender, RoutedEventArgs e)
        {
            if(EndConnNo.SelectedIndex > StartConnNo.SelectedIndex)
            {
                Func<RuntimeData, int, bool> filter = (o, i) => i < EndConnNo.SelectedIndex && i > StartConnNo.SelectedIndex;
                filters.Add(filter);
            }
            if(filterObj.selectedClientList.Count > 0 )
            {
                Func<RuntimeData, int , bool> filter = (o,i) => filterObj.selectedClientList.Contains(o.CustCode);
                filters.Add(filter);
            }

            this.Close();
        } 

        private void CheckBox_SelectClient_Checked(object sender, RoutedEventArgs e)
        {
           
            //CheckBox thisCheckBox = (CheckBox)sender;
            //if ((bool)thisCheckBox.IsChecked)
            //{
            //    SelectedClientList.Add((string)thisCheckBox.Tag);
            //}
            //else
            //{
            //    SelectedClientList.Remove((string)thisCheckBox.Tag);
            //}
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
