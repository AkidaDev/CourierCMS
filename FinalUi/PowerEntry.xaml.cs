using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for PowerEntry.xaml
    /// </summary>
    public partial class PowerEntry : Window
    {
        BillingDataDataContext db;
        List<RuntimeData> DataStack;
        public PowerEntry(List<RuntimeData> DataStack, List<String> ClientCodes, BillingDataDataContext db)
            : this()
        {
            List<string> connList = DataStack.OrderBy(x => x.BookingDate).ThenBy(y => y.ConsignmentNo).Select(c => c.ConsignmentNo).ToList();
            startConnNo.DataContext = connList;
            endConnNo.DataContext =connList;
            clientCode.DataContext = ClientCodes;
            this.DataStack = DataStack;
            this.db = db;
        }
        PowerEntry()
        {
            InitializeComponent();
        }

        private void SubmitRecords_Click(object sender, RoutedEventArgs e)
        {
            int startCOnnNoIndex = startConnNo.SelectedIndex;
            int endConnNoIndex = endConnNo.SelectedIndex;
            this.progressbar.Visibility = Visibility.Hidden;
            if (startCOnnNoIndex <= endConnNoIndex && startCOnnNoIndex != -1 && endConnNoIndex != -1)
            {
                for (int i = startCOnnNoIndex; i <= endConnNoIndex; i++)
                {
                    RuntimeData data = DataStack.ElementAt(i);
                    data.CustCode = clientCode.SelectedValue.ToString();
                    var c = (from m in db.Cities select m).Where(x => x.CITY_CODE== data.Destination && x.CITY_STATUS=="A").FirstOrDefault();
                    data = db.RuntimeDatas.Single(x => x.Id == data.Id);
                    data.CustCode = clientCode.SelectedValue.ToString();
                    data.FrAmount = (decimal)UtilityClass.getCost(data.CustCode, data.Destination, data.DestinationPin, data.Weight, c.ZONE, data.Type, (char)data.DOX);
                    data.FrWeight = data.Weight;
                }
                db.SubmitChanges();
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
