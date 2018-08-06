namespace RigMonitor.Models.DataModels.Miner
{
    public class WorkerData
    {
        public string Id { get; set; } 
        public int Uid { get; set; } 
        public string Hashrate { get; set; } //Worker Current Hashrate
        public string ReportedHashrate { get; set; } //Worker Reported Hashrate
        public int LastShare { get; set; } //Last Share Date of Worker
        public int Rating { get; set; } //Last Share Date of Worker
        public string H1 { get; set; }
        public string H3 { get; set; }
        public string H6 { get; set; }
        public string H12 { get; set; }
        public string H24 { get; set; }
    }
}