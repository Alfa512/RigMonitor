namespace RigMonitor.Models.DataModels.Worker
{
    public class WorkerCurrentHashrateModel : NanopoolEth
    {
        public string Worker { get; set; } //Worker ID: Parameter
        public double Data { get; set; } //Worker Hashrate [Mh/s]
    }
}