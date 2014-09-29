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
    /// Interaction logic for PreferenceWindow.xaml
    /// </summary>
    public partial class PreferenceWindow : Window
    {
        enum themes
        {
            Blue,
            Gray,
        };
        enum dataFormat
        {
           f1 = "mm/dd/yyyy",
           f2 = "dd/mm/yyyy",
           f3 = "yyyy/dd/mm",
        };
        public PreferenceWindow()
        {
            InitializeComponent();
            FillDetails();
        }
        public void FillDetails()
        {
            this.serviceTaxBox.Text = Configs.Default.ServiceTax;
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Configs.Default.ServiceTax = this.serviceTaxBox.Text;
            Configs.Default.Save();
            this.Close();
        }

        private void DefaultButton_Click(object sender, RoutedEventArgs e)
        {
            Configs.Default.Reset();
            FillDetails();
        }
    }
}