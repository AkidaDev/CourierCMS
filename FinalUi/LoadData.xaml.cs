﻿using System;
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
    /// Interaction logic for LoadData.xaml
    /// </summary>
    public partial class LoadData : Window
    {

        public LoadData()
        {
            isNewSheet = true;
            InitializeComponent();
        }
        public bool dataLoaded { get; set; }
        public string filename1 { get; set; }
        public bool isNewSheet { get; set; }
        public List<RuntimeData> data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }
       List<RuntimeData> _data;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog file = new Microsoft.Win32.OpenFileDialog();
            file.DefaultExt = ".txt";
            file.Filter = "(.txt)|*.txt";
            Nullable<bool> result = file.ShowDialog();
            if (result == true)
            {
                filename.Text = filename1 = file.FileName;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (filename1 != null)
            {
                CSVDataLoader load = new CSVDataLoader();
                _data = load.getRuntimeDataFromCSV(filename1, '"', '\'');
                dataLoaded = true;

                this.Close();
            }
            else
            {
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBox.Show("file not selected", "warning", button, icon);
            }
        }

        private void addData_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox obj = (CheckBox)sender;
            isNewSheet = (bool)obj.IsChecked;
            isNewSheet = !isNewSheet;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if(DataToLoadDate.SelectedDate != null)
            {
                BillingDataDataContext db = new BillingDataDataContext();
                _data = UtilityClass.convertTransListToRuntimeList(db.Transactions.Where(x => x.Date == DataToLoadDate.SelectedDate).ToList());
                dataLoaded = true;
                this.Close();
            }
        }

    }
}
