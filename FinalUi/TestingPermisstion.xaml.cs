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
    /// Interaction logic for TestingPermisstion.xaml
    /// </summary>
    public partial class TestingPermisstion : Window
    {
        public TestingPermisstion()
        {
            InitializeComponent();
            if (SecurityModule.hasPermission(Login.userid, "read"))
            {
                MessageBox.Show("Ha ha ha ha it worked");
            }
            else {
                MessageBox.Show("not worked");
            }
        }
    }
}
