﻿using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using RigMonitor.Api.Nanopool.ETH;
using RigMonitor.Services;

namespace RigMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public EthController EthController;
        public Thread RigRestarter;
        public LoggerService LoggerService;
        public MainWindow()
        {
            EthController = new EthController();
            InitializeComponent();
            LoggerService = new LoggerService("MainInterfaceLogger");
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var x = !string.IsNullOrEmpty(XPointerTextBox.Text) ? Convert.ToInt32(XPointerTextBox.Text) : 800;
                var y = !string.IsNullOrEmpty(YPointerTextBox.Text) ? Convert.ToInt32(YPointerTextBox.Text) : 500;
                var targetHashrate = !string.IsNullOrEmpty(TargetHashrateTextBox.Text) ? Convert.ToInt32(TargetHashrateTextBox.Text) : 370;
                var rig0 = new RigMonitorService();
                rig0.XPointer = x;
                rig0.YPointer = y;
                rig0.TargetHashrate = targetHashrate;
                rig0.WorkerId = "Server";
                RigRestarter = new Thread(rig0.RestartByWatchDogIfNoReport);
                RigRestarter.Start();
            }
            catch (Exception ex)
            {
                LoggerService.LogError($"{DateTime.Now.ToString("G")} : Error: {ex.Message} \r\n {ex.InnerException?.Message} \r\n");
            }
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RigRestarter?.IsAlive == true)
                    RigRestarter.Abort();
            }
            catch (Exception ex)
            {
                LoggerService.LogError($"{DateTime.Now.ToString("G")} : Error: {ex.Message} \r\n {ex.InnerException?.Message} \r\n");
            }
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var refreshData = new Thread(RefreshData);
                refreshData.Start();
            }
            catch (Exception ex)
            {
                LoggerService.LogError($"{DateTime.Now.ToString("G")} : Error: {ex.Message} \r\n {ex.InnerException?.Message} \r\n");
            }
        }

        public void RefreshData()
        {
            try
            {
                var info = EthController.GetGeneralInfo();
                var workersData = EthController.GetAllWorkersData();
                this.Dispatcher.Invoke(() =>
                {
                    CurrentCalculatedHashrateTextBox.Text = Convert.ToString(info.Data.Hashrate, CultureInfo.InvariantCulture);
                    BalanceTextBox.Text = info.Data.Balance;
                    AverageHashrate6HoursTextBox.Text = info.Data.Avghashrate.H6;
                    WorkersTable.ItemsSource = workersData;
                });
                
                var lastReportedHashrate = workersData.Sum(r => r.ReportedHashrate);
                this.Dispatcher.Invoke(() =>
                {
                    LastReportedHashrateTextBox.Text = lastReportedHashrate.ToString("N2");
                });

            }
            catch (Exception ex)
            {
                LoggerService.LogError($"{DateTime.Now.ToString("G")} : Error: {ex.Message} \r\n {ex.InnerException?.Message} \r\n");
            }
        }

        private void TestBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var x = !string.IsNullOrEmpty(XPointerTextBox.Text) ? Convert.ToInt32(XPointerTextBox.Text) : 800;
                var y = !string.IsNullOrEmpty(YPointerTextBox.Text) ? Convert.ToInt32(YPointerTextBox.Text) : 500;
                //WinAPI.MouseMove(x, y);
                //WinAPI.MouseClick("left");
                var rig0 = new RigMonitorService();
                rig0.XPointer = x;
                rig0.YPointer = y;
                var rigRestarter = new Thread(rig0.MouseMoveTest);
                rigRestarter.Start();
            }
            catch (Exception ex)
            {
                LoggerService.LogError($"{DateTime.Now.ToString("G")} : Error: {ex.Message} \r\n {ex.InnerException?.Message} \r\n");
            }
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void TestResetBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var x = !string.IsNullOrEmpty(XPointerTextBox.Text) ? Convert.ToInt32(XPointerTextBox.Text) : 800;
                var y = !string.IsNullOrEmpty(YPointerTextBox.Text) ? Convert.ToInt32(YPointerTextBox.Text) : 500;
                //WinAPI.MouseMove(x, y);
                //WinAPI.MouseClick("left");
                //WinAPI.MouseClick("left");
                //WinAPI.MouseClick("left");
                var rig0 = new RigMonitorService();
                rig0.XPointer = x;
                rig0.YPointer = y;
                var rigRestarter = new Thread(rig0.RestartRig);
                rigRestarter.Start();
            }
            catch (Exception ex)
            {
                LoggerService.LogError($"{DateTime.Now.ToString("G")} : Error: {ex.Message} \r\n {ex.InnerException?.Message} \r\n");
            }
        }
    }
}
