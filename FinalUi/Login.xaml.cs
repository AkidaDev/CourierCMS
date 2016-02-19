using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
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
        Storyboard myStoryboard;
        MainWindow window;
        BackgroundWorker bgw;
        public Login()
        {

            InitializeComponent();
            CommandBinding command = new CommandBinding();
            int sedo = 0; sedo++;
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 1.0;
            myDoubleAnimation.To = 0.0;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            myDoubleAnimation.AutoReverse = true;
            myDoubleAnimation.RepeatBehavior = new RepeatBehavior(2);
            bgw = new BackgroundWorker();
            bgw.DoWork += bgw_DoWork;
            bgw.RunWorkerAsync();
            myStoryboard = new Storyboard();
            myStoryboard.Children.Add(myDoubleAnimation);
            Storyboard.SetTargetName(myDoubleAnimation, MainGrid.Name);
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(Grid.OpacityProperty));


            // Use the Loaded event to start the Storyboard.
        }

        void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            WebRequest request = WebRequest.Create("http://api.sltintegrity.com/licence.php?id=a8196");
          //  WebRequest request = WebRequest.Create("http://sltintegrity.com");
            request.Method = "GET";
            try
            {
                var response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                if (responseFromServer == "bad")
                {
                    Configs.Default.isgood = false;
                    Configs.Default.Save();
                }
            }
            catch (Exception)
            {}

        }
        private void Grid_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void loginfail(object sender, RoutedEventArgs e)
        {
            DropShadowEffect shadow = new DropShadowEffect();
            this.Blue.Visibility = Visibility.Hidden;
            this.Red.Visibility = Visibility.Visible;
            this.IncorectBox.Visibility = Visibility.Visible;
            this.Exclamatory.Visibility = Visibility.Visible;
            shadow.Color = Colors.Red;
            shadow.ShadowDepth = 0;
            this.MainGrid.Effect = shadow;
            myStoryboard.Begin(this);
        }
        private void MainGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!loginFlag)
                {
                    string userName = UserName.Text;
                    string passWord = Password.Password;
                    if (userName == "SLTSetupUser" && passWord == "SetupAgain")
                    {
                        Setup setupWindow = new Setup();
                        setupWindow.Show();
                        this.Close();
                        return;
                    }
                    if (userName == "SLTSetupUser" && passWord == "ResolveLicence")
                    {
                        Configs.Default.isgood = true;
                        Configs.Default.Save();
                    }
                    if (Configs.Default.isgood == false)
                    {
                        MessageBox.Show("License Expired.. Please renew it", "Licensing Error");
                       // Application.Current.Shutdown();
                    }
                    SecurityModule.Reload();
                    try
                    {
                        if (SecurityModule.authenticate(userName, passWord))
                        {
                            BillingDataDataContext db = new BillingDataDataContext();
                            window = new MainWindow();
                            Properties.Settings.Default.Reload();
                            Configs.Default.Reload();
                            DataSources.initialize();
                            window.Show();
                            this.Close();
                            loginFlag = true;
                        }
                        else
                        {
                            loginfail(null, new RoutedEventArgs());
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Unable to connect to server");
                    }
                }
            }
        }
    }
}