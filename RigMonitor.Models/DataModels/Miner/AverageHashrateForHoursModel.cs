namespace RigMonitor.Models.DataModels.Miner
{
    public class AverageHashrateForHoursModel : NanopoolEth
    {
        public int Hours { get; set; }
        public double AverageHashrate { get; set; }
    }
}