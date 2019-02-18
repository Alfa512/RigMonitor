using System;
using System.ServiceProcess;
using System.Threading;
using System.Windows;
using RigMonitor.Services;

namespace RigMonitor.Run
{
    public class RigMonitorRunService : ServiceBase
    {
        public LoggerService LoggerService;
        public RigMonitorRunService()
        {
            LoggerService = new LoggerService("ServiceRunLogger");
            ServiceRun();
        }
        private void ServiceRun()
        {
            var rigRestarter = new Thread(StartRigMonitor);
            rigRestarter.SetApartmentState(ApartmentState.STA);
            try
            {
                rigRestarter.Start();
            }
            catch (Exception e)
            {
                LoggerService.LogError("RigMonitor Run as Service failed. ");
                LoggerService.LogError(e);
                rigRestarter.Abort();
            }
        }

        private void StartRigMonitor()
        {
            var app = new Application();
            app.Run(new MainWindow());
        }

    }
}