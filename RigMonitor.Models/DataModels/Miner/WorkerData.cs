namespace RigMonitor.Models.DataModels
{
    public class WorkerData
    {
        public string Id { get; set; } 
        public int Uid { get; set; } 
        public double Hashrate { get; set; } //Worker Current Hashrate
        public int LastShare { get; set; } //Last Share Date of Worker
        public int Rating { get; set; } //Last Share Date of Worker
        public double Avg_h1 { get; set; }
        public double Avg_h3 { get; set; }
        public double Avg_h6 { get; set; }
        public double Avg_h12 { get; set; }
        public double Avg_h24 { get; set; }
    }
}