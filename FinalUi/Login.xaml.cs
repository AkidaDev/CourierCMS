using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfAnimatedGif;

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

           // ImageBehavior.SetRepeatBehavior(LoadingGif, RepeatBehavior.Forever);
            CommandBinding command = new CommandBinding();

            /*  Testing Code  */
            /********************* Must Delete Afterwards **********************/
            /* 
           // window = new MainWindow("dharmendra");
            window = new MainWindow();
            window
             * 
             * 
             * 
             * 
            this.Close();
            loginFlag = false;
           */
        }
        MainWindow window;

        private void Grid_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CloseButton_MouseLeave_1(object sender, MouseEventArgs e)
        {
            //    this.CloseButton.Foreground = new SolidColorBrush(Colors.White);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void loginfail()
        {
            DropShadowEffect shadow = new DropShadowEffect();
            this.Blue.Visibility = Visibility.Hidden;
            this.Red.Visibility = Visibility.Visible;
            this.IncorectBox.Visibility = Visibility.Visible;
            this.Exclamatory.Visibility = Visibility.Visible;
            shadow.Color = Colors.Red;
            shadow.ShadowDepth = 0;
            this.MainGrid.Effect = shadow;
        }

        private void MainGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!loginFlag)
                {
                  //  LoadingGifPanel.Visibility = Visibility.Visible;
                    string userName = UserName.Text;
                    string passWord = Password.Password;
                    if (SecurityModule.authenticate(userName, passWord))
                    {
                       // LoadingGifPanel.Visibility = Visibility.Visible;

                        window = new MainWindow();
                        window.ShowDialog();
                        this.Close();
                        loginFlag = true;
                    }
                    else
                    {
                        loginfail();
                    }
                }
            }
        }


    }
}
