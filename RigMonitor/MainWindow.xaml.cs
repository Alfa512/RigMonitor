using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using RigMonitor.Api.Nanopool.ETH;
using RigMonitor.Services;
using SimpleSample;

namespace RigMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public EthController EthController;
        public Thread RigRestarter;
        public Thread RigRestarterRig4;
        public LoggerService LoggerService;
        public string DefaultXpointer { get; set; }
        public string DefaultYpointer { get; set; }
        public string Rig4Xpointer { get; set; }
        public string Rig4Ypointer { get; set; }
        public string Rig4Xoffset { get; set; }
        public string Rig4Yoffset { get; set; }
        private int ProcessCount { get; set; }
        private int ProcessCountRig4 { get; set; }
        public int ResetInterval;

        public List<RigMonitorService> Rigs;

        public MainWindow()
        {
            EthController = new EthController();
            InitializeComponent();
            LoggerService = new LoggerService("MainInterfaceLogger");
            DefaultXpointer = ConfigurationSettings.AppSettings["PointerX"];
            DefaultYpointer = ConfigurationSettings.AppSettings["PointerY"];
            Rig4Xpointer = ConfigurationSettings.AppSettings["PointerXRig4"];
            Rig4Ypointer = ConfigurationSettings.AppSettings["PointerYRig4"];
            Rig4Xoffset = ConfigurationSettings.AppSettings["PointerXOffsetRig4"];
            Rig4Yoffset = ConfigurationSettings.AppSettings["PointerYOffsetRig4"];
            ResetInterval = Convert.ToInt32(ConfigurationSettings.AppSettings["ResetInterval"]);
            XPointerTextBoxRig0.Text = DefaultXpointer ?? XPointerTextBoxRig0.Text;
            YPointerTextBoxRig0.Text = DefaultYpointer ?? YPointerTextBoxRig0.Text;
            XPointerTextBoxRig4.Text = Rig4Xpointer ?? XPointerTextBoxRig4.Text;
            YPointerTextBoxRig4.Text = Rig4Ypointer ?? YPointerTextBoxRig4.Text;
            XPointerOffsetTbRig4.Text = Rig4Xoffset ?? XPointerOffsetTbRig4.Text;
            YPointerOffsetTbRig4.Text = Rig4Yoffset ?? YPointerOffsetTbRig4.Text;

            InitElements();
            //Auto start
            StartOnlineMonitor(0);
            StartOnlineMonitorRig4(4);
        }

        private void InitElements()
        {
            ProcessCount = 0;
            Rigs = new List<RigMonitorService>();
            this.Dispatcher.Invoke(() =>
            {
                MonStartedRig0.Visibility = Visibility.Hidden;
                MonStoppedRig0.Visibility = Visibility.Hidden;

                
            });
        }

        private void StartBtnRig0_Click(object sender, RoutedEventArgs e)
        {
            StartOnlineMonitor(0);
        }

        private void StopBtnRig0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StoppedRig0();
                if (RigRestarter?.IsAlive == true)
                {
                    RigRestarter.Abort();
                    ProcessCount--;
                }
            }
            catch (Exception ex)
            {
                var text = $"{DateTime.Now.ToString("G")} : Error: {ex.Message} \r\n {ex.InnerException?.Message} \r\n";
                LoggerService.LogError(text);
                AddToConsoleNewLine(text);
            }
            finally
            {
                SetProcesses(ProcessCount);
            }
        }

        private void RefreshBtnRig0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var refreshData = new Thread(RefreshData);
                refreshData.Start();
            }
            catch (Exception ex)
            {
                var text = $"{DateTime.Now.ToString("G")} : Error: {ex.Message} \r\n {ex.InnerException?.Message} \r\n";
                LoggerService.LogError(text);
                AddToConsoleNewLine(text);
            }
        }

        public void LoadingSpinnerShow()
        {
            this.Dispatcher.Invoke(() =>
            {
                LoadingSpinnerRig0.Visibility = Visibility.Visible;
            });
        }

        public void LoadingSpinnerHide()
        {
            this.Dispatcher.Invoke(() =>
            {
                LoadingSpinnerRig0.Visibility = Visibility.Hidden;
            });
        }
        public void StartedRig0()
        {
            this.Dispatcher.Invoke(() =>
            {
                MonStartedRig0.Visibility = Visibility.Visible;
                MonStoppedRig0.Visibility = Visibility.Hidden;
            });
        }

        public void StoppedRig0()
        {
            this.Dispatcher.Invoke(() =>
            {
                MonStartedRig0.Visibility = Visibility.Hidden;
                MonStoppedRig0.Visibility = Visibility.Visible;
            });
        }
        public void SetProcesses(int count)
        {
            this.Dispatcher.Invoke(() => { ProcessesCountRig0.Content = count; });
        }

        private void SetConsoleTextBox(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;
            this.Dispatcher.Invoke(() => { ConsoleTextBox.Text = text; });
        }
        private void AddToConsoleNewLine(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;
            this.Dispatcher.Invoke(() =>
            {
                ConsoleTextBox.Text += "\r\n" + text;
            });
        }

        private void StartOnlineMonitor(int rigId)
        {
            try
            {
                LoadingSpinnerShow();

                var x = !string.IsNullOrEmpty(XPointerTextBoxRig0.Text) ? Convert.ToInt32(XPointerTextBoxRig0.Text) : 800;
                var y = !string.IsNullOrEmpty(YPointerTextBoxRig0.Text) ? Convert.ToInt32(YPointerTextBoxRig0.Text) : 500;
                var targetHashrate = !string.IsNullOrEmpty(TargetHashrateTextBoxRig0.Text)
                    ? Convert.ToInt32(TargetHashrateTextBoxRig0.Text)
                    : 370;
                this.Rigs.Add(new RigMonitorService
                {
                    RigId = rigId,
                    WorkerId = "Rig" + rigId
                });
                var rig = Rigs.First(r => r.RigId == rigId);
                rig.XPointer = x;
                rig.YPointer = y;
                rig.TargetHashrate = targetHashrate;
                rig.WorkerId = "Rig" + rig.RigId;
                rig.ResetInterval = ResetInterval;
                if (RigRestarter != null && RigRestarter.IsAlive)
                {
                    RigRestarter.Abort();
                    ProcessCount--;
                }
                RigRestarter = new Thread(rig.RestartByWatchDogIfNoReport);
                RigRestarter.Start();
                LoadingSpinnerHide();
                StartedRig0();
                ProcessCount++;
            }
            catch (Exception ex)
            {
                var text = $"{DateTime.Now.ToString("G")} : Error: {ex.Message} \r\n {ex.InnerException?.Message} \r\n";
                LoggerService.LogError(text);
                AddToConsoleNewLine(text);
                StoppedRig0();
            }
            finally
            {
                SetProcesses(ProcessCount);
            }
        }

        public void RefreshData()
        {
            try
            {
                LoadingSpinnerShow();
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
                LoadingSpinnerHide();
            }
            catch (Exception ex)
            {
                LoadingSpinnerHide();
                var text = $"{DateTime.Now.ToString("G")} : Error: {ex.Message} \r\n {ex.InnerException?.Message} \r\n";
                LoggerService.LogError(text);
                AddToConsoleNewLine(text);
            }
        }

        private void TestBtnRig0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var x = !string.IsNullOrEmpty(XPointerTextBoxRig0.Text) ? Convert.ToInt32(XPointerTextBoxRig0.Text) : 800;
                var y = !string.IsNullOrEmpty(YPointerTextBoxRig0.Text) ? Convert.ToInt32(YPointerTextBoxRig0.Text) : 500;
                var xTab = x - 50;
                var yTab = y - 55;
                //WinAPI.MouseMove(x, y);
                //WinAPI.MouseClick("left");
                var rig0 = new RigMonitorService();
                rig0.XPointer = xTab;
                rig0.YPointer = yTab;
                var rigRestarter = new Thread(rig0.MouseMoveTest);
                rigRestarter.Start();
                Thread.Sleep(200);
                if (rigRestarter.IsAlive)
                    rigRestarter.Abort();
                rig0.XPointer = x;
                rig0.YPointer = y;
                rigRestarter = new Thread(rig0.MouseMoveTest);
                rigRestarter.Start();
                Thread.Sleep(1000);
                if (rigRestarter.IsAlive)
                    rigRestarter.Abort();
            }
            catch (Exception ex)
            {
                var text = $"{DateTime.Now.ToString("G")} : Error: {ex.Message} \r\n {ex.InnerException?.Message} \r\n";
                LoggerService.LogError(text);
                AddToConsoleNewLine(text);
            }
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TestResetBtnRig0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var x = !string.IsNullOrEmpty(XPointerTextBoxRig0.Text) ? Convert.ToInt32(XPointerTextBoxRig0.Text) : 800;
                var y = !string.IsNullOrEmpty(YPointerTextBoxRig0.Text) ? Convert.ToInt32(YPointerTextBoxRig0.Text) : 500;
                var xTab = x - 50;
                var yTab = y - 55;
                //WinAPI.MouseMove(x, y);
                //WinAPI.MouseClick("left");
                //WinAPI.MouseClick("left");
                //WinAPI.MouseClick("left");
                var rig0 = new RigMonitorService();
                rig0.XPointer = xTab;
                rig0.YPointer = yTab;
                var rigRestarter = new Thread(rig0.RestartRig);
                rigRestarter.Start();
                Thread.Sleep(200);
                if (rigRestarter.IsAlive)
                    rigRestarter.Abort();
                rig0.XPointer = x;
                rig0.YPointer = y;
                rigRestarter = new Thread(rig0.RestartRig);
                rigRestarter.Start();
                Thread.Sleep(1000);
                if (rigRestarter.IsAlive)
                    rigRestarter.Abort();
            }
            catch (Exception ex)
            {
                var text = $"{DateTime.Now.ToString("G")} : Error: {ex.Message} \r\n {ex.InnerException?.Message} \r\n";
                LoggerService.LogError(text);
                AddToConsoleNewLine(text);
            }
        }

        #region Rig4

        /**** Rig4 Region ****/

        private void StartBtnRig4_Click(object sender, RoutedEventArgs e)
        {
            StartOnlineMonitorRig4(4);
        }

        private void StopBtnRig4_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StoppedRig4();
                if (RigRestarterRig4?.IsAlive == true)
                {
                    RigRestarterRig4.Abort();
                    ProcessCountRig4--;
                }
            }
            catch (Exception ex)
            {
                var text = $"{DateTime.Now.ToString("G")} : Error: {ex.Message} \r\n {ex.InnerException?.Message} \r\n";
                LoggerService.LogError(text);
                AddToConsoleNewLine(text);
            }
            finally
            {
                SetProcessesRig4(ProcessCountRig4);
            }
        }

        public void SetProcessesRig4(int count)
        {
            this.Dispatcher.Invoke(() => { ProcessesCountRig4.Content = count; });
        }

        public void StartedRig4()
        {
            this.Dispatcher.Invoke(() =>
            {
                MonStartedRig4.Visibility = Visibility.Visible;
                MonStoppedRig4.Visibility = Visibility.Hidden;
            });
        }

        public void StoppedRig4()
        {
            this.Dispatcher.Invoke(() =>
            {
                MonStartedRig4.Visibility = Visibility.Hidden;
                MonStoppedRig4.Visibility = Visibility.Visible;
            });
        }

        private void TestBtnRig4_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var x = !string.IsNullOrEmpty(XPointerTextBoxRig4.Text) ? Convert.ToInt32(XPointerTextBoxRig4.Text) : 800;
                var y = !string.IsNullOrEmpty(YPointerTextBoxRig4.Text) ? Convert.ToInt32(YPointerTextBoxRig4.Text) : 500;
                var xOffset = !string.IsNullOrEmpty(XPointerOffsetTbRig4.Text) ? Convert.ToInt32(XPointerOffsetTbRig4.Text) : 50;
                var yOffset = !string.IsNullOrEmpty(YPointerOffsetTbRig4.Text) ? Convert.ToInt32(YPointerOffsetTbRig4.Text) : 50;
                var xTab = x - xOffset;
                var yTab = y - yOffset;
                //WinAPI.MouseMove(x, y);
                //WinAPI.MouseClick("left");
                var rig = new RigMonitorService();
                rig.XPointerRig4 = xTab;
                rig.YPointerRig4 = yTab;
                var rigRestarter = new Thread(rig.MouseMoveTestRig4);
                rigRestarter.Start();
                Thread.Sleep(200);
                if (rigRestarter.IsAlive)
                    rigRestarter.Abort();
                rig.XPointerRig4 = x;
                rig.YPointerRig4 = y;
                rig.XPointerOffsetRig4 = xOffset;
                rig.YPointerOffsetRig4 = yOffset;
                rigRestarter = new Thread(rig.MouseMoveTestRig4);
                rigRestarter.Start();
                Thread.Sleep(1000);
                if (rigRestarter.IsAlive)
                    rigRestarter.Abort();
            }
            catch (Exception ex)
            {
                var text = $"{DateTime.Now.ToString("G")} : Error: {ex.Message} \r\n {ex.InnerException?.Message} \r\n";
                LoggerService.LogError(text);
                AddToConsoleNewLine(text);
            }
        }


        private void TestResetBtnRig4_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var x = !string.IsNullOrEmpty(XPointerTextBoxRig4.Text) ? Convert.ToInt32(XPointerTextBoxRig4.Text) : 800;
                var y = !string.IsNullOrEmpty(YPointerTextBoxRig4.Text) ? Convert.ToInt32(YPointerTextBoxRig4.Text) : 500;
                var xOffset = !string.IsNullOrEmpty(XPointerOffsetTbRig4.Text) ? Convert.ToInt32(XPointerOffsetTbRig4.Text) : 50;
                var yOffset = !string.IsNullOrEmpty(YPointerOffsetTbRig4.Text) ? Convert.ToInt32(YPointerOffsetTbRig4.Text) : 50;
                var xTab = x - xOffset;
                var yTab = y - yOffset;
                //WinAPI.MouseMove(x, y);
                //WinAPI.MouseClick("left");
                //WinAPI.MouseClick("left");
                //WinAPI.MouseClick("left");
                var rig = new RigMonitorService();
                rig.XPointerRig4 = xTab;
                rig.YPointerRig4 = yTab;
                rig.XPointerOffsetRig4 = xOffset;
                rig.YPointerOffsetRig4 = yOffset;
                var rigRestarter = new Thread(rig.RestartRig4);
                rigRestarter.Start();
                Thread.Sleep(1000);
                if (rigRestarter.IsAlive)
                    rigRestarter.Abort();
                rig.XPointerRig4 = x;
                rig.YPointerRig4 = y;
                rig.XPointerOffsetRig4 = xOffset;
                rig.YPointerOffsetRig4 = yOffset;
                rigRestarter = new Thread(rig.RestartRig4);
                rigRestarter.Start();
                Thread.Sleep(1000);
                if (rigRestarter.IsAlive)
                    rigRestarter.Abort();
            }
            catch (Exception ex)
            {
                var text = $"{DateTime.Now.ToString("G")} : Error: {ex.Message} \r\n {ex.InnerException?.Message} \r\n";
                LoggerService.LogError(text);
                AddToConsoleNewLine(text);
            }
        }

        private void StartOnlineMonitorRig4(int rigId)
        {
            try
            {
                LoadingSpinnerShow();

                var x = !string.IsNullOrEmpty(XPointerTextBoxRig4.Text) ? Convert.ToInt32(XPointerTextBoxRig4.Text) : 800;
                var y = !string.IsNullOrEmpty(YPointerTextBoxRig4.Text) ? Convert.ToInt32(YPointerTextBoxRig4.Text) : 500;
                var xOffset = !string.IsNullOrEmpty(XPointerOffsetTbRig4.Text) ? Convert.ToInt32(XPointerOffsetTbRig4.Text) : 50;
                var yOffset = !string.IsNullOrEmpty(YPointerOffsetTbRig4.Text) ? Convert.ToInt32(YPointerOffsetTbRig4.Text) : 50;

                var targetHashrate = !string.IsNullOrEmpty(TargetHashrateTextBoxRig4.Text)
                    ? Convert.ToInt32(TargetHashrateTextBoxRig4.Text)
                    : 309;
                Rigs.Add(new RigMonitorService
                {
                    RigId = rigId,
                    WorkerId = "Rig" + rigId
                });
                var rig = Rigs.First(r => r.RigId == rigId);
                rig.XPointerRig4 = x;
                rig.YPointerRig4 = y;
                rig.XPointerOffsetRig4 = xOffset;
                rig.YPointerOffsetRig4 = yOffset;
                rig.TargetHashrate = targetHashrate;
                rig.WorkerId = "Rig" + rig.RigId;
                rig.ResetInterval = ResetInterval;
                if (RigRestarterRig4 != null && RigRestarterRig4.IsAlive)
                {
                    RigRestarterRig4.Abort();
                    ProcessCountRig4--;
                }
                RigRestarterRig4 = new Thread(rig.RestartByWatchDogIfNoReportRig4);
                RigRestarterRig4.Start();
                LoadingSpinnerHide();
                StartedRig4();
                ProcessCountRig4++;
            }
            catch (Exception ex)
            {
                var text = $"{DateTime.Now.ToString("G")} : Error: {ex.Message} \r\n {ex.InnerException?.Message} \r\n";
                LoggerService.LogError(text);
                AddToConsoleNewLine(text);
                StoppedRig4();
            }
            finally
            {
                SetProcessesRig4(ProcessCountRig4);
            }
        }

        #endregion
    }
}
