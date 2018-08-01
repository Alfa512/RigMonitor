using System.Collections.Generic;
using RigMonitor.Models.DataModels.Miner;

namespace RigMonitor.Models.DataModels.Worker
{
    public class WorkerChartDataModel : NanopoolEth
    {
        public string Worker { get; set; } //Worker ID: Parameter
        public List<ChartData> Data { get; set; }
    }
}