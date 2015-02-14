using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
    /// Interaction logic for Updater.xaml
    /// </summary>
    public partial class Updater : Window
    {
        Update update;
        string file;
        WebClient webClient;
        Uri uri;
        bool startdownload = false;
        public Updater()
        {
            InitializeComponent();
            update = new Update();
            update.getLatestVer();
            file = System.IO.Path.GetTempPath() + @"vortex.exe";
            MessageBox.Show(file);
            webClient = new WebClient();
            string url = "http://api.vortex.sltintegrity.com/download/beta/vortex_" + update.vers.ToString() + ".exe";
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