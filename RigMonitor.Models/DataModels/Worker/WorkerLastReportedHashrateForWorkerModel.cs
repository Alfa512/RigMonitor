namespace RigMonitor.Models.DataModels.Worker
{
    public class WorkerLastReportedHashrateForWorkerModel : NanopoolEth
    {
        public string Worker { get; set; } //Worker ID: Parameter
        public double Data { get; set; } //Reported Hashrate [Mh/s]
    }
}