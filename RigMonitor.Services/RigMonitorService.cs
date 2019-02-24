﻿using System;
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
        public int ResetInterval;

        public RigMonitorService()
        {
            EthController = new EthController();
            WorkersControlList = new EthNanopoolControlList();
            XPointer = 1200;
            YPointer = 90;
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
            int resetInterval = ResetInterval <= 0 ? -22 : ResetInterval * (-1);
            DateTime restartedTime = DateTime.Now.AddMinutes(resetInterval);
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
                    if (result)
                    {
                        LoggerService.LogInfo($"Restartable. Last restart: {restartedTime:G}; Time Check: {DateTime.Now.AddMinutes(resetInterval):G};");
                    }
                    if (result && restartedTime < DateTime.Now.AddMinutes(resetInterval))
                    {
                        RestartRig(XPointer-50, YPointer-55);
                        Thread.Sleep(1500);
                        RestartRig(XPointer, YPointer);
                        restartedTime = DateTime.Now;
                        LoggerService.LogInfo($"{controlData.Worker.Id} restarted: {restartedTime:G}; X:{XPointer}, Y:{YPointer};\r\n");
                    }
                    if (WorkersControlList.GetHead().TimeMoment < DateTime.Now.AddHours(-5))
                        WorkersControlList.Remove(WorkersControlList.GetHead());
                }
                catch (Exception e)
                {
                    LoggerService.LogError($"{DateTime.Now:G} : Error: {e.Message} \r\n {e.InnerException?.Message} \r\n");
                }

                Thread.Sleep(180000);
            }
        }

        public bool CheckRig(string workerId, int targetHashrate)
        {
            try
            {
                int resetInterval = ResetInterval <= 0 ? -22 : ResetInterval * (-1);

                if (WorkersControlList.Count < Convert.ToInt32(resetInterval / 3) + 1)
                    return false;
                targetHashrate = targetHashrate - (targetHashrate / 10);
                var now = WorkersControlList.GetTail();
                var firstInterval = WorkersControlList.SearchEarliest(DateTime.Now.AddMinutes(Convert.ToInt32(resetInterval *0.5)));
                var secondInterval = WorkersControlList.SearchEarliest(DateTime.Now.AddMinutes(resetInterval));
                var reported = now.ReportedHashrate;
                var calculated = now.CurrentCalculatedHashrate;
                LoggerService.LogInfo($"{DateTime.Now:G} - {workerId} checked: OK; Reported: {reported}; Calculated: {calculated}\r\n");
                if (targetHashrate > now.ReportedHashrate && targetHashrate > firstInterval.ReportedHashrate && targetHashrate > secondInterval.ReportedHashrate)
                    return true;
                LoggerService.LogInfo($"{DateTime.Now:G} - {workerId} checked: OK; {Convert.ToInt32(resetInterval * 0.5)} min: {firstInterval.ReportedHashrate}; 10 min: {secondInterval.ReportedHashrate}");

                //LoggerService.LogInfo($"{DateTime.Now:G} - {workerId} cheched: OK\r\n");
            }
            catch (Exception e)
            {
                LoggerService.LogError($"{DateTime.Now:G} : Error: {e.Message} \r\n {e.InnerException?.Message}\r\n");
                return false;
            }
            return false;
        }

        public void RestartRig(int x, int y)
        {
            try
            {
                WinAPI.MouseMove(x, y);
                Thread.Sleep(500);
                WinAPI.MouseClick("left");
                Thread.Sleep(500);
                WinAPI.MouseClick("left");
            }
            catch (Exception e)
            {
                LoggerService.LogError($"{DateTime.Now.ToString("G")} : Error: {e.Message} \r\n {e.InnerException?.Message} \r\n");
            }
        }
        public void RestartRig()
        {
            try
            {
                RestartRig(XPointer, YPointer);
            }
            catch (Exception e)
            {
                LoggerService.LogError($"{DateTime.Now.ToString("G")} : Error: {e.Message} \r\n {e.InnerException?.Message} \r\n");
            }
        }

        public void MouseMoveTest(int x, int y)
        {
            try
            {
                WinAPI.MouseMove(x, y);
                Thread.Sleep(100);
            }
            catch (Exception e)
            {
                LoggerService.LogError($"{DateTime.Now.ToString("G")} : Error: {e.Message} \r\n {e.InnerException?.Message} \r\n");
            }
        }

        public void MouseMoveTest()
        {
            try
            {
                MouseMoveTest(XPointer, YPointer);
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
