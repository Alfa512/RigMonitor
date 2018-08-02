using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RigMonitor.Api.Nanopool.ETH;

namespace RigMonitor.Services
{
    public class RigMonitorService
    {
        public EthController EthController;

        public RigMonitorService()
        {
            EthController = new EthController();
        }

        public async void RestartByWatchDogIfNoReport(string workerId, int targetHashrate)
        {
            var workersData = EthController.GetAllWorkersData();
            var worker = workersData.FirstOrDefault(r => r.Id.ToLower().Equals(workerId.ToLower()));
            
        }
    }
}
