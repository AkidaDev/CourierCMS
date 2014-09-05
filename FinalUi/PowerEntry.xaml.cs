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
        public PowerEntry(List<String> connNos, List<String> ClientCodes) : this()
        {
            startConnNo.DataContext = connNos;
            endConnNo.DataContext = connNos;
            clientCode.DataContext = ClientCodes;
        }
        PowerEntry()
        {
            InitializeComponent();
        }

        private void SubmitRecords_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
