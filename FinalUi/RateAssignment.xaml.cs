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
    /// Interaction logic for RateAssignment.xaml
    /// </summary>
    public partial class RateAssignment : Window
    {
        CollectionViewSource ClientList;
        CollectionViewSource RateList;
        CollectionViewSource ZoneList;
        CollectionViewSource ServiceList;
        public Assignment assignment;
        public bool isEdited = false;
        public RateAssignment(Assignment assignment)
        {
            BillingDataDataContext db = new BillingDataDataContext();
            this.assignment = assignment;
            InitializeComponent();
            InitializeDataSources();
            RateCodeName.Text = assignment.RateCode;
            RateDescription.Text = db.Rates.Single(x => x.RateCode == assignment.RateCode).Description;
            if (assignment.ClientCode != null)
                ComboBoxClient.Text = assignment.ClientCode;
            if (assignment.ServiceCode != null)
                ComboBoxService.Text = assignment.ServiceCode;
            if (assignment.ZoneCode != null)
                ComboBoxZone.Text = assignment.ZoneCode;
        }
        private void InitializeDataSources()
        {
            ClientList = (CollectionViewSource)FindResource("ClientList");
            RateList = (CollectionViewSource)FindResource("RateList");
            ZoneList = (CollectionViewSource)FindResource("ZoneList");
            ServiceList = (CollectionViewSource)FindResource("ServiceList");
            BillingDataDataContext db = new BillingDataDataContext();
            ClientList.Source = db.Clients;
            RateList.Source = db.Rates;
            ZoneList.Source = db.ZONEs;
            ServiceList.Source = db.Services;
        }

        private void AssignButton_Click(object sender, RoutedEventArgs e)
        {
            assignment.ClientCode = ComboBoxClient.Text;
            assignment.ZoneCode = ComboBoxZone.Text;
            assignment.ServiceCode = ComboBoxService.Text;
            isEdited = true;
            this.Close();
        }
		private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
            //  this.Owner.Effect = null;
            this.Close();
        }

        private void DragthisWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
