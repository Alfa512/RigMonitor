namespace RigMonitor.Models.DataModels.Worker
{
    public class WorkerAverageHashrateModel : NanopoolEth
    {
        public string Worker { get; set; } //Worker ID: Parameter
        public double Data { get; set; }
        public int Hours { get; set; } //Period [hours]: Parameter
    }
}