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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace FinalUi
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        Update update;
        string file;
        WebClient webClient;
        Uri uri;
        bool startdownload = false;
        public About()
        {
            InitializeComponent();
            this.VortexVer.Text = Configs.Default.ver;
            update = new Update();
            update.getLatestVer();
            file = System.IO.Path.GetTempPath() + @"vortex.exe";
            webClient = new WebClient();
            string url = "http://api.vortex.sltintegrity.com/download/vortex_" + update.vers.ToString() + ".exe";
            this.Closed += Updater_Closed;
            HttpWebResponse response;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException ex)
            {
                response = ex.Response as HttpWebResponse;
            }
            this.ContentRendered += Updater_ContentRendered;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                uri = new Uri(url);
                webClient.DownloadFileCompleted += Completed;
                webClient.DownloadProgressChanged += ProgressChanged;
                startdownload = true;
            }
            Debug.Print("\n" + file);
        }
        private void update_CheckedUnChecked(object sender, RoutedEventArgs e)
        {
            if (this.checkbox_unselected.Visibility == Visibility.Hidden)
            {
                this.checkbox_selected.Visibility = Visibility.Hidden;
                this.checkbox_unselected.Visibility = Visibility.Visible;
            }
            else
            {
                this.checkbox_selected.Visibility = Visibility.Visible;
                this.checkbox_unselected.Visibility = Visibility.Hidden;
            }
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }
        void Updater_Closed(object sender, EventArgs e)
        {
        }

        void Updater_Closing(object sender, CancelEventArgs e)
        {
            throw new NotImplementedException();
        }
        void Updater_ContentRendered(object sender, EventArgs e)
        {
            if (startdownload)
            {
                try
                {
                    webClient.DownloadFileAsync(uri, file);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    //MessageBox.Show("Sorry Error Occured. Retry Later" );
                }
            }
            else { MessageBox.Show("Unable to Update vortex"); this.Close(); }
        }
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgressbar.Value = e.ProgressPercentage;
            this.updater.Visibility = Visibility.Visible;
        }
        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            if (File.Exists(file))
            {
                Process.Start(file);
                Application.Current.Shutdown();
            }
            else
            {
                MessageBox.Show("Sorry Error Occured. Retry Later");
                this.Close();
            }
        }
    }
}
