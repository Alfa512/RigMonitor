using System.Linq;
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

        public double GetAverageHashrateForPeriod()
        {
            return 1;
        }

       

    }
}
