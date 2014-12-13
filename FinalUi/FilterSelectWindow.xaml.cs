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
        CollectionViewSource StartConnNoList;
        CollectionViewSource EndConnoList;
        Filter filterObj;

        public List<Func<RuntimeData, int, bool>> filters;
        /// <summary>
        /// Creates a new Filter Selection Window
        /// </summary>
        /// <param name="filterObj">Filter Object of current context</param>
        /// <param name="ConnNo">Connsignment Number List of current object</param>
        public FilterSelectWindow(Filter filterObj, IEnumerable<string> ConnNo)
        {
            setDataSource(filterObj,ConnNo);
        }
        public void setDataSource(Filter filterObj, IEnumerable<string> ConnNo)
        {
            InitializeComponent();
            this.filterObj = filterObj;
            BillingDataDataContext db = new BillingDataDataContext();

            StartConnNoList = (CollectionViewSource)FindResource("StartConnNoList");
            List<String> ConnNoList = ConnNo.OrderBy(x => x).ToList() ;
            StartConnNoList.Source = ConnNoList;
            EndConnoList = (CollectionViewSource)FindResource("EndConnoList");
            EndConnoList.Source = ConnNoList;
            List<Client> fClientList = db.Clients.ToList();
            SelectClientBox.AllListSource = fClientList.Where(x => !filterObj.selectedClientList.Select(y => y.CLCODE).Contains(x.CLCODE)).ToList();
            SelectClientBox.SelectedListSource= filterObj.selectedClientList;
            SelectClientBox.DisplayValuePath = "NameAndCode";
            switch (filterObj.showBilled)
            {
                case true:
                    {
                        BilledRadio.IsChecked = true;
                        break;
                    }
                case false:
                    {
                        UnBilledRadio.IsChecked = true;
                        break;
                    }
                case null:
                    {
                        AllRadio.IsChecked = true;
                        break;
                    }

            }
            if (filterObj.startConnNo != "" && ConnNo.Contains(filterObj.startConnNo))
                StartConnNo.Text = filterObj.startConnNo;
            else
                StartConnNo.Text = ConnNo.FirstOrDefault() ?? "";
            if (filterObj.endConnNo != "" && ConnNo.Contains(filterObj.endConnNo))
                EndConnNo.Text = filterObj.endConnNo;
            else
                EndConnNo.Text = ConnNo.LastOrDefault() ?? "";
            ToDate.SelectedDate = filterObj.toDate;
            FromDate.SelectedDate = filterObj.fromDate;
            StartPriceValue.Text = filterObj.startPrice.ToString();
            EndPriceValue.Text = filterObj.endPrice.ToString();
        }

        private void GetFilter_Click(object sender, RoutedEventArgs e)
        {
            string errorMsg = "";
            double temp;
            if (!double.TryParse(StartPriceValue.Text, out temp))
                errorMsg += "Enter starting price value correctly \n";
            if (!double.TryParse(EndPriceValue.Text, out temp))
                errorMsg += "Enter ending price value correctly \n";
            if (!((List<string>)EndConnoList.Source).Contains(EndConnNo.Text) && !((List<string>)StartConnNoList.Source).Contains(StartConnNo.Text))
                errorMsg += "Enter the connsignments correctly \n";
            if (ToDate.SelectedDate < FromDate.SelectedDate)
                errorMsg += "Enter the date correctly \n";
            if (errorMsg != "")
                MessageBox.Show("Please correct the following errors: \n" + errorMsg);
            else
            {
                filterObj.endConnNo = EndConnNo.Text;
                filterObj.fromDate = (DateTime)FromDate.SelectedDate;
                filterObj.selectedClientList = SelectClientBox.SelectedListSource.Cast<Client>().ToList();
                if (BilledRadio.IsChecked == true)
                    filterObj.showBilled = true;
                if (UnBilledRadio.IsChecked == true)
                    filterObj.showBilled = false;
                if (AllRadio.IsChecked == true)
                    filterObj.showBilled = null;
                filterObj.startConnNo = StartConnNo.Text;
                filterObj.toDate = (DateTime)ToDate.SelectedDate;
                filterObj.startPrice = double.Parse(StartPriceValue.Text);
                filterObj.endPrice = double.Parse(EndPriceValue.Text);

            }
            this.Close();
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
