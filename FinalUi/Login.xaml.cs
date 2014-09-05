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
    /// Interaction logic for Login.xaml
    /// </summary>
    /// 
    //TODO: Apply validators in the form
    public partial class Login : Window
    {
        bool loginFlag;
        public Login()
        {
            InitializeComponent();
            /*  Testing Code  */
            /********************* Must Delete Afterwards **********************/
            
           // window = new MainWindow("dharmendra");
            window = new MainWindow();
            window.Show();
            this.Close();
            loginFlag = false;
        }
        MainWindow window;
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (!loginFlag)
            {
                string userName = UserName.Text;
                string passWord = Password.Password;
                if (SecurityModule.authenticate(userName, passWord))
                {
                    window = new MainWindow();
                    window.Show();
                    this.Close();
                    loginFlag = true;
                }
                else
                {
                    MessageBox.Show("Invalid Credentials");
                }
            }
        }

        private void SetupButton_Click_1(object sender, RoutedEventArgs e)
        {
            InitializeScript scripts = new InitializeScript();
            string error = scripts.intializeDatabase(UserName.Text, Password.Password, InstanceName.Text);
            if (error != "")
            {
                MessageBox.Show("Error : " + error);
            }
            else
                MessageBox.Show("Successfull");
        }
    }
}
