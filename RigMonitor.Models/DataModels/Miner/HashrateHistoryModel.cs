using System.Collections.Generic;

namespace RigMonitor.Models.DataModels.Miner
{
    public class HashrateHistoryModel : NanopoolEth
    {
        public List<ChartData> Data { get; set; } //Date and Hashrate only
    }
}