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
    /// Interaction logic for ManageService.xaml
    /// </summary>
    public partial class ManageService : Window
    {
        public ManageService()
        {
            InitializeComponent();
        }

        private void ReloadServiceGroup_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditServiceGroup_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddServiceGroup_Click(object sender, RoutedEventArgs e)
        {
            ServiceGrouping window = new ServiceGrouping();
            window.Show();
        }

        private void DeleteServiceGroup_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
