using System.Collections.Generic;

namespace RigMonitor.Models.DataModels.Miner
{
    public class GeneralInfoData
    {
        public string Balance { get; set; }
        public string Unconfirmed_balance { get; set; }
        public string Hashrate { get; set; } //Current Hashrate
        public AverageHashrateData Avghashrate { get; set; } //Current Hashrate
        public List<WorkerData> Workers { get; set; } //Current Hashrate
    }
}