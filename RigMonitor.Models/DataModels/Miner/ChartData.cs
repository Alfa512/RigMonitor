namespace RigMonitor.Models.DataModels.Miner
{
    public class ChartData
    {
        public int Date { get; set; }
        public int Shares { get; set; }
        public int Hashrate { get; set; } //Miner Reported Hashrate [Mh/s]
    }
}