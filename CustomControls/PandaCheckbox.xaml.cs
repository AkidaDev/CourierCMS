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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CustomControls
{
    /// <summary>
    /// Interaction logic for PandaCheckbox.xaml
    /// </summary>
    public partial class PandaCheckbox : UserControl
    {
        /*public RoutedEventHandler Click
        {
            set
            {
                MainButton.Click += value;
            }
        }
        public void RemoveRoutedEventhandler(RoutedEventHandler eve)
        {
            MainButton.Click -= eve;
        }
         * */
        public CheckBox Value
        {
            get
            {
                return ThisCheckBox;
            }
        }

        public PandaCheckbox()
        {

            InitializeComponent();

        }
        public bool? Checked
        {
            get
            {
                return ThisCheckBox.IsChecked;
            }
            set
            {
                if (value == false)
                {
                    this.checkbox_selected.Visibility = Visibility.Hidden;
                    ThisCheckBox.IsChecked = false;
                    this.checkbox_unselected.Visibility = Visibility.Visible;
                }
                else
                {
                    this.checkbox_selected.Visibility = Visibility.Visible;
                    ThisCheckBox.IsChecked = true;
                    this.checkbox_unselected.Visibility = Visibility.Hidden;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.checkbox_unselected.Visibility == Visibility.Hidden)
            {
                this.checkbox_selected.Visibility = Visibility.Hidden;
                ThisCheckBox.IsChecked = false;
                this.checkbox_unselected.Visibility = Visibility.Visible;
            }
            else
            {
                this.checkbox_selected.Visibility = Visibility.Visible;
                ThisCheckBox.IsChecked = true;
                this.checkbox_unselected.Visibility = Visibility.Hidden;
            }
        }
    }
}
