﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
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
    /// Interaction logic for RecalculatePriceWindow.xaml
    /// </summary>
    public partial class RecalculatePriceWindow : Window
    {

        BackgroundWorker worker;
        CollectionViewSource ClientSource;

        public RecalculatePriceWindow()
        {
            InitializeComponent();
            ClientSource = (CollectionViewSource)FindResource("ClientListSource");
            ClientSource.Source = DataSources.ClientCopy;
            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.WorkerSupportsCancellation = true;

        }
        bool isSortingByDate = false;
        bool isSortingByConnNo = false;
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProgressBarTask.Value = 0;
            if (e.Error == null)
                MessageBox.Show(e.Result.ToString());
            else
                MessageBox.Show("Completed with errors...");
            this.ButtonClick.Text = " Start";
            this.Path.Visibility = Visibility.Visible;
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBarTask.Value = e.ProgressPercentage;
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Func<Transaction, bool> whereQuery = (Func<Transaction, bool>)e.Argument;
            worker.ReportProgress(1);
            BillingDataDataContext db = new BillingDataDataContext();

            List<Transaction> transactions = db.Transactions.Where(whereQuery).ToList();
          double transCount = transactions.Count;

            double i = 0;
            foreach (Transaction trans in transactions)
            {
                if (trans.ConnsignmentNo == "X10477603")
                {
                    Debug.WriteLine("ABC");
                }
                trans.AmountCharged = (decimal)UtilityClass.getCost(trans.CustCode, trans.BilledWeight ?? 0, trans.Destination, trans.Type.Trim(), trans.DOX);
                if (trans.Insurance != null)
                    trans.AmountCharged = trans.AmountCharged + (decimal)trans.Insurance;
                
                worker.ReportProgress((int)((i / transCount) * 94 + 1));
                i++;
            }
            ChangeSet changeSet = db.GetChangeSet();
            db.SubmitChanges();
            worker.ReportProgress(100);
            e.Result = "Completed for " + ((int)transCount).ToString() + " transactions";
        }
        void reportErrorInTooltip(Control Sender, Control ToolTipTarget, object Tooltip)
        {
            Point TargetPoint = ToolTipTarget.PointToScreen(new Point(0d, 0d));
            double HorizontalOffset = TargetPoint.X;
            double VerticalOffset = TargetPoint.Y + ToolTipTarget.ActualHeight;
            System.Windows.Controls.ToolTip toolTip = new ToolTip()
            {
                Content = Tooltip,
                Placement = System.Windows.Controls.Primitives.PlacementMode.Absolute,
                HorizontalOffset = HorizontalOffset,
                VerticalOffset = VerticalOffset,
            };
            var tempToolTip = (ToolTip)ToolTipTarget.ToolTip;
            if (tempToolTip != null)
                tempToolTip.IsOpen = false;
            ToolTipTarget.ToolTip = toolTip;

            ((ToolTip)ToolTipTarget.ToolTip).IsOpen = true;
            Timer tooltipClosingTImer = new Timer();
            ElapsedEventHandler timerIntervalElapsedAction = (obj, eveArg) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ((System.Windows.Controls.ToolTip)ToolTipTarget.ToolTip).IsOpen = false;
                    }));
                tooltipClosingTImer.Stop();
                tooltipClosingTImer.Dispose();
            };
            tooltipClosingTImer.Interval = 2000;
            tooltipClosingTImer.Elapsed += timerIntervalElapsedAction;
            tooltipClosingTImer.Start();
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (worker.IsBusy == true)
            {
                if (MessageBox.Show("Are you sure you want to stop? No changes will be done", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (worker.IsBusy)
                    {
                        worker.CancelAsync();
                        MessageBox.Show("Cancelled..");
                    }
                    this.ButtonClick.Text = " Start";
                    this.Path.Visibility = Visibility.Visible;
                }
                return;
            }
            string ClientCode = ((Client)ClientComboBox.SelectedItem).CLCODE;
            Func<Transaction, bool> whereQuery = x => x.CustCode == ClientCode && x.RecalculateEnabled == 'T';
            if (DateCheckBox.Checked == true || ConnsignmentNoCheckBox.Checked == true)
            {
                if (DateCheckBox.Checked== true)
                    isSortingByDate = true;
                else
                    isSortingByDate = false;
                if (ConnsignmentNoCheckBox.Checked == true)
                    isSortingByConnNo = true;
                else
                    isSortingByConnNo = false;
                if (isSortingByDate == true)
                {
                    bool isError = false;
                    if (FromDatePicker.SelectedDate == null)
                    {
                        reportErrorInTooltip((Control)sender, FromDatePicker, "Select Date...");
                        isError = true;
                    }
                    if (ToDatePicker.SelectedDate == null)
                    {

                        reportErrorInTooltip((Control)sender, ToDatePicker, "Select Date...");
                        isError = true;
                    }
                    if (FromDatePicker.SelectedDate > ToDatePicker.SelectedDate)
                    {
                        reportErrorInTooltip((Control)sender, DateCheckBox, "From date should be before to date");
                        isError = true;
                    }
                    if (isError)
                        return;
                    DateTime FromDate = (DateTime)FromDatePicker.SelectedDate;
                    DateTime ToDate = (DateTime)ToDatePicker.SelectedDate;
                    Func<Transaction, bool> tempFunc = whereQuery;
                    whereQuery = x => tempFunc(x) && x.BookingDate <= ToDate && x.BookingDate >= FromDate;
                }
                if (isSortingByConnNo == true)
                {
                    bool isError = false;
                    if (ToConnNoTextBox.Text == "")
                    {
                        reportErrorInTooltip((Control)sender, ToConnNoTextBox, "Cannot be empty");
                        isError = true;
                    }
                    if (FromConnNoTextBox.Text == "")
                    {
                        reportErrorInTooltip((Control)sender, FromConnNoTextBox, "Cannot be empty");
                        isError = true;
                    }
                    if (isError)
                        return;
                    if (ToConnNoTextBox.Text.Length != FromConnNoTextBox.Text.Length)
                    {
                        reportErrorInTooltip((Control)sender, FromConnNoTextBox, "Lenght of the consignment number are not equal...");
                        isError = true;
                    }
                    if (String.Compare(ToConnNoTextBox.Text, FromConnNoTextBox.Text) < 0)
                    {
                        reportErrorInTooltip((Control)sender, FromConnNoTextBox, "To Consignment Number should be greater than from Consignment number...");
                        isError = true;
                    }

                    if (isError)
                        return;
                    string FromConnNo = FromConnNoTextBox.Text;
                    string ToConnNo = ToConnNoTextBox.Text;
                    Func<Transaction, bool> tempFun = whereQuery;
                    whereQuery = x => tempFun(x) && String.Compare(x.ConnsignmentNo, FromConnNo) >= 0 && String.Compare(x.ConnsignmentNo, ToConnNo) <= 0;
                }
                this.ButtonClick.Text = " Cancel";
                this.Path.Visibility = Visibility.Collapsed;
                worker.RunWorkerAsync(whereQuery);
            }
            else
            {
                reportErrorInTooltip((Control)sender, DateCheckBox, "Please select atleast one checkbox...");
                reportErrorInTooltip((Control)sender, ConnsignmentNoCheckBox, "Please select alteast one checkbox...");
            }
        }
    }
}
