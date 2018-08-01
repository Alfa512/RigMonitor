using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using RigMonitor.Api.Nanopool.ETH;

namespace RigMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public EthController EthController;
        public MainWindow()
        {
            EthController = new EthController();
            InitializeComponent();
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
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
    }
}
