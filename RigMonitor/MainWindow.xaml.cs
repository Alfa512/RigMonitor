using System;
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
        public MainWindow()
        {
            EthController = new EthController();
            InitializeComponent();
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
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

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            if (RigRestarter?.IsAlive == true)
                RigRestarter.Abort();
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            var info = EthController.GetGeneralInfo();
            CurrentCalculatedHashrateTextBox.Text = Convert.ToString(info.Data.Hashrate, CultureInfo.InvariantCulture);
            BalanceTextBox.Text = info.Data.Balance;
            AverageHashrate6HoursTextBox.Text = info.Data.Avghashrate.H6;

            var workersData = EthController.GetAllWorkersData();
            WorkersTable.ItemsSource = workersData;
            var lastReportedHashrate = workersData.Sum(r => r.ReportedHashrate);
            LastReportedHashrateTextBox.Text = lastReportedHashrate.ToString("N2");
        }

        private void TestBtn_Click(object sender, RoutedEventArgs e)
        {
            var x = !string.IsNullOrEmpty(XPointerTextBox.Text) ? Convert.ToInt32(XPointerTextBox.Text) : 800;
            var y = !string.IsNullOrEmpty(YPointerTextBox.Text) ? Convert.ToInt32(YPointerTextBox.Text) : 500;
            WinAPI.MouseMove(x,y);
            //WinAPI.MouseClick("left");
        }
    }
}
