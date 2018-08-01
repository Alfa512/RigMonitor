namespace RigMonitor.Models.DataModels
{
    public class WorkersAverageHashrateModel : NanopoolEth
    {
        public WorkersAverageHashrateData Data { get; set; }
        public int Hours { get; set; } //Parameter
    }
}