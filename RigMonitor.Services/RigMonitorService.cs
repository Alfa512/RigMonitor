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
        public LoggerService LoggerService;


        public RigMonitorService()
        {
            EthController = new EthController();
            WorkersControlList = new EthNanopoolControlList();
            XPointer = 960;
            YPointer = 540;
            LoggerService = new LoggerService("RigMonitorServiceLogger");

            //var config = new NLog.Config.LoggingConfiguration();

            //var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "logging.log" };
            //var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            //config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);
            //config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            //LoggerService.Configuration = config;
        }

        public void RestartByWatchDogIfNoReport()
        {
            DateTime restartedTime = DateTime.Now.AddMinutes(-10);
            while (true)
            {
                try
                {
                    //LoggerService.LogError($"{DateTime.Now.ToString("G")} : Test: Iterarion {WorkersControlList.Count}");
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
                    if (result && restartedTime < DateTime.Now.AddMinutes(-15))
                    {
                        RestartRig(XPointer, YPointer);
                        restartedTime = DateTime.Now;
                        LoggerService.LogInfo($"{controlData.Worker.Id} restarted: {restartedTime:G};\r\n");
                    }
                    if (WorkersControlList.GetHead().TimeMoment < DateTime.Now.AddHours(-5))
                        WorkersControlList.Remove(WorkersControlList.GetHead());
                }
                catch (Exception e)
                {
                    LoggerService.LogError($"{DateTime.Now:G} : Error: {e.Message} \r\n {e.InnerException?.Message} \r\n");
                }

                Thread.Sleep(240000);
            }
        }

        public bool CheckRig(string workerId, int targetHashrate)
        {
            double reported = 0;
            double calculated = 0;
            try
            {
                if (WorkersControlList.Count < 4)
                    return false;
                targetHashrate = targetHashrate - (targetHashrate / 10);
                var now = WorkersControlList.GetTail();
                var tenMinutesAgo = WorkersControlList.SearchEarliest(DateTime.Now.AddMinutes(-8));
                var twentyMinutesAgo = WorkersControlList.SearchEarliest(DateTime.Now.AddMinutes(-12));
                reported = now.ReportedHashrate;
                calculated = now.CurrentCalculatedHashrate;
                if (targetHashrate > now.ReportedHashrate && targetHashrate > tenMinutesAgo.ReportedHashrate && targetHashrate > twentyMinutesAgo.ReportedHashrate)
                    return true;
                //LoggerService.LogInfo($"{DateTime.Now:G} - {workerId} cheched: OK\r\n");
            }
            catch (Exception e)
            {
                LoggerService.LogError($"{DateTime.Now:G} : Error: {e.Message} \r\n {e.InnerException?.Message}\r\n");
                return false;
            }
            LoggerService.LogInfo($"{DateTime.Now:G} - {workerId} cheched: OK; Reported: {reported}; Calculated: {calculated}\r\n");
            return false;
        }

        public void RestartRig(int x, int y)
        {
            try
            {
                WinAPI.MouseMove(x, y);
                WinAPI.MouseClick("left");
            }
            catch (Exception e)
            {
                LoggerService.LogError($"{DateTime.Now.ToString("G")} : Error: {e.Message} \r\n {e.InnerException?.Message} \r\n");
            }
        }

        public double GetAverageHashrateForPeriod()
        {
            return 1;
        }



    }
}
