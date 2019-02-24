using System;
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
        public LoggerService LoggerService;
        public string DefaultXpointer { get; set; }
        public string DefaultYpointer { get; set; }
        private int ProcessCount { get; set; }
        public int ResetInterval;

        public MainWindow()
        {
            EthController = new EthController();
            InitializeComponent();
            LoggerService = new LoggerService("MainInterfaceLogger");
            DefaultXpointer = ConfigurationSettings.AppSettings["PointerX"];
            DefaultYpointer = ConfigurationSettings.AppSettings["PointerY"];
            ResetInterval = Convert.ToInt32(ConfigurationSettings.AppSettings["ResetInterval"]);
            XPointerTextBox.Text = DefaultXpointer ?? XPointerTextBox.Text;
            YPointerTextBox.Text = DefaultYpointer ?? YPointerTextBox.Text;
            InitElements();
            //Auto start
            StartOnlineMonitor();
        }

        private void InitElements()
        {
            ProcessCount = 0;
            this.Dispatcher.Invoke(() =>
            {
                MonStarted.Visibility = Visibility.Hidden;
                MonStopped.Visibility = Visibility.Hidden;
            });
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            StartOnlineMonitor();
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Stopped();
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

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
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
                LoadingSpinner.Visibility = Visibility.Visible;
            });
        }

        public void LoadingSpinnerHide()
        {
            this.Dispatcher.Invoke(() =>
            {
                LoadingSpinner.Visibility = Visibility.Hidden;
            });
        }
        public void Started()
        {
            this.Dispatcher.Invoke(() =>
            {
                MonStarted.Visibility = Visibility.Visible;
                MonStopped.Visibility = Visibility.Hidden;
            });
        }

        public void Stopped()
        {
            this.Dispatcher.Invoke(() =>
            {
                MonStarted.Visibility = Visibility.Hidden;
                MonStopped.Visibility = Visibility.Visible;
            });
        }
        public void SetProcesses(int count)
        {
            this.Dispatcher.Invoke(() => { ProcessesCount.Content = count; });
        }

        private void SetConsoleTextBox(string text)
        {
            if(string.IsNullOrEmpty(text))
                return;
            this.Dispatcher.Invoke(() => { ConsoleTextBox.Text = text; });
        }
        private void AddToConsoleNewLine(string text)
        {
            if(string.IsNullOrEmpty(text))
                return;
            this.Dispatcher.Invoke(() =>
            {
                ConsoleTextBox.Text += "\r\n" + text;
            });
        }

        private void StartOnlineMonitor()
        {
            try
            {
                LoadingSpinnerShow();

                var x = !string.IsNullOrEmpty(XPointerTextBox.Text) ? Convert.ToInt32(XPointerTextBox.Text) : 800;
                var y = !string.IsNullOrEmpty(YPointerTextBox.Text) ? Convert.ToInt32(YPointerTextBox.Text) : 500;
                var targetHashrate = !string.IsNullOrEmpty(TargetHashrateTextBox.Text)
                    ? Convert.ToInt32(TargetHashrateTextBox.Text)
                    : 370;
                var rig0 = new RigMonitorService();
                rig0.XPointer = x;
                rig0.YPointer = y;
                rig0.TargetHashrate = targetHashrate;
                rig0.WorkerId = "Server";
                rig0.ResetInterval = ResetInterval;
                if (RigRestarter != null && RigRestarter.IsAlive)
                {
                    RigRestarter.Abort();
                    ProcessCount--;
                }
                RigRestarter = new Thread(rig0.RestartByWatchDogIfNoReport);
                RigRestarter.Start();
                LoadingSpinnerHide();
                Started();
                ProcessCount++;
            }
            catch (Exception ex)
            {
                var text = $"{DateTime.Now.ToString("G")} : Error: {ex.Message} \r\n {ex.InnerException?.Message} \r\n";
                LoggerService.LogError(text);
                AddToConsoleNewLine(text);
                Stopped();
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

        private void TestBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var x = !string.IsNullOrEmpty(XPointerTextBox.Text) ? Convert.ToInt32(XPointerTextBox.Text) : 800;
                var y = !string.IsNullOrEmpty(YPointerTextBox.Text) ? Convert.ToInt32(YPointerTextBox.Text) : 500;
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

        private void TestResetBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var x = !string.IsNullOrEmpty(XPointerTextBox.Text) ? Convert.ToInt32(XPointerTextBox.Text) : 800;
                var y = !string.IsNullOrEmpty(YPointerTextBox.Text) ? Convert.ToInt32(YPointerTextBox.Text) : 500;
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
    }
}
