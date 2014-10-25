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
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
        }
        private void update_CheckedUnChecked(object sender, RoutedEventArgs e)
        {
            if (this.checkbox_selected.Visibility == Visibility.Hidden)
            {
                this.checkbox_selected.Visibility = Visibility.Visible;
                this.checkbox_unselected.Visibility = Visibility.Hidden;
            }
            else
            {
                this.checkbox_selected.Visibility = Visibility.Hidden;
                this.checkbox_unselected.Visibility = Visibility.Visible;
            }
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
