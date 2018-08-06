using System;
using System.Linq;
using System.Threading;
using RigMonitor.Api.Nanopool.ETH;
using RigMonitor.Models.DataModels;

namespace RigMonitor.Services
{
    public class RigMonitorService
    {
        public EthController EthController;
        public EthNanopoolControlList WorkersControlList;
        public string WorkerId;
        public int TargetHashrate;
        public int XPointer;
        public int YPointer;

        public RigMonitorService()
        {
            EthController = new EthController();
            WorkersControlList = new EthNanopoolControlList();
            XPointer = 960;
            YPointer = 540;
        }

        public async void RestartByWatchDogIfNoReport()
        {
            DateTime restartedTime = DateTime.Now.AddMinutes(-15);
            while (true)
            {
                var workersData = EthController.GetAllWorkersData();
                var worker = workersData.FirstOrDefault(r => r.Id.ToLower().Equals(WorkerId.ToLower()));
                if (worker == null)
                    continue;
                var controlData = new EthNanopoolControl(worker);
                controlData.TimeMoment = DateTime.Now;
                controlData.LastShare = worker.LastShare;
                controlData.ReportedHashrate = worker.ReportedHashrate;
                WorkersControlList.Add(controlData);

                var result = CheckRig(WorkerId, TargetHashrate);
                if (result && restartedTime < DateTime.Now.AddMinutes(-20))
                {
                    RestartRig(XPointer, YPointer);
                    restartedTime = DateTime.Now;
                }
                if (WorkersControlList.GetHead().TimeMoment < DateTime.Now.AddHours(-5))
                    WorkersControlList.Remove(WorkersControlList.GetHead());
                Thread.Sleep(300000);
            }
        }

        public bool CheckRig(string workerId, int targetHashrate)
        {
            if (WorkersControlList.Count < 20)
                return false;
            targetHashrate = targetHashrate - (targetHashrate/10);
            var now = WorkersControlList.GetTail();
            var tenMinutesAgo = WorkersControlList.SearchEarliest(DateTime.Now.AddMinutes(-10));
            var twentyMinutesAgo = WorkersControlList.SearchEarliest(DateTime.Now.AddMinutes(-20));
            if (targetHashrate > now.ReportedHashrate && targetHashrate > tenMinutesAgo.ReportedHashrate && targetHashrate > twentyMinutesAgo.ReportedHashrate)
                return false;
            return true;
        }

        public void RestartRig(int x, int y)
        {
            WinAPI.MouseMove(x,y);
            WinAPI.MouseClick("left");
        }

        public double GetAverageHashrateForPeriod()
        {
            return 1;
        }

       

    }
}
