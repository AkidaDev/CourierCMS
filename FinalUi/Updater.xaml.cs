using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        public Updater()
        {
            InitializeComponent();
            update = new Update();
            update.getLatestVer();
            file = System.AppDomain.CurrentDomain.BaseDirectory + @"temp\vortex.exe";
            webClient = new WebClient();
            string url = "http://testapi.sltintegrity.com/download/vortex_" + update.vers.ToString() + ".exe";
            uri = new Uri(url);
            webClient.DownloadFileCompleted += Completed;
            webClient.DownloadProgressChanged += ProgressChanged;
            this.ContentRendered += Updater_ContentRendered;
        }
        void Updater_ContentRendered(object sender, EventArgs e)
        {
            try
            {
                webClient.DownloadFileAsync(uri, file);
            }
            catch (Exception ex)
            {
                webClient.DownloadFileCompleted -= Completed;
                MessageBox.Show("Sorry Error Occured. Retry Later");
            }
        }
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgressbar.Value = e.ProgressPercentage;
        }
        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            Process.Start(file);
            Application.Current.Shutdown();
        }
    }
}