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
    /// Interaction logic for PowerEntry.xaml
    /// </summary>
    public partial class PowerEntry : Window
    {
        BillingDataDataContext db;
        List<RuntimeData> DataStack;
        public PowerEntry(List<RuntimeData> DataStack, List<String> ClientCodes, BillingDataDataContext db)
            : this()
        {
            startConnNo.DataContext = DataStack.Select(c => c.ConsignmentNo).ToList();
            endConnNo.DataContext = DataStack.Select(c => c.ConsignmentNo).ToList();
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
            if (startCOnnNoIndex < endConnNoIndex && startCOnnNoIndex != -1 && endConnNoIndex != -1)
            {
                for (int i = startCOnnNoIndex; i <= endConnNoIndex; i++)
                {
                    RuntimeData data = DataStack.ElementAt(i);
                    data = db.RuntimeDatas.Single(x => x.Id == data.Id);
                    data.CustCode = clientCode.SelectedValue.ToString();
                    data.FrAmount = (decimal)UtilityClass.getCost(data.CustCode, data.Destination, data.DestinationPin, data.Weight);
                    data.FrWeight = data.Weight;
                   
                    data.CustCode = clientCode.SelectedValue.ToString();
                    data.FrAmount = (decimal)UtilityClass.getCost(data.CustCode, data.Destination, data.DestinationPin, data.Weight);
                    data.FrWeight = data.Weight;
                    db.SubmitChanges();
                }
            }
        }

    }
}
