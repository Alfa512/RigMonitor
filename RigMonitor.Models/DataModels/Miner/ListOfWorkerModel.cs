using System.Collections.Generic;

namespace RigMonitor.Models.DataModels
{
    public class ListOfWorkerModel : NanopoolEth
    {
        public List<WorkerData> Data { get; set; } //Array of Workers
    }
}