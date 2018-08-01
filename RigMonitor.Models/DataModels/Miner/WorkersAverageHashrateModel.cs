namespace RigMonitor.Models.DataModels.Miner
{
    public class WorkersAverageHashrateModel : NanopoolEth
    {
        public WorkersAverageHashrateData Data { get; set; }
        public int Hours { get; set; } //Parameter
    }
}