using System.Collections.Generic;

namespace RigMonitor.Models.DataModels
{
    public class GeneralInfoData
    {
        public string Balance { get; set; }
        public string Unconfirmed_balance { get; set; }
        public double Hashrate { get; set; } //Current Hashrate
        public AverageHashrateData Avghashrate { get; set; } //Current Hashrate
        public List<WorkerData> Worker { get; set; } //Current Hashrate
    }
}