namespace RigMonitor.Models.DataModels
{
    public class AverageHashrateForHoursModel : NanopoolEth
    {
        public int Hours { get; set; }
        public double AverageHashrate { get; set; }
    }
}