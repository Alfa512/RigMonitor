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
        private Thread RigRestarter;
        public RigMonitorRunService()
        {
            LoggerService = new LoggerService("ServiceRunLogger");
        }
        private void ServiceRun()
        {
            RigRestarter = new Thread(StartRigMonitor);
            RigRestarter.SetApartmentState(ApartmentState.STA);
            try
            {
                RigRestarter.Start();
                LoggerService.LogInfo("RigRestarter thread started. ");
            }
            catch (Exception e)
            {
                LoggerService.LogError("RigMonitor Run as Service failed. ");
                LoggerService.LogError(e);
                if (RigRestarter.IsAlive)
                    RigRestarter.Abort();
            }
        }

        protected override void OnStart(string[] args)
        {
            ServiceRun();
        }
        protected override void OnStop()
        {
            if (RigRestarter.IsAlive)
            {
                RigRestarter.Abort();
                LoggerService.LogInfo("RigRestarter aborted.");
            }

            LoggerService.LogInfo("RigMonitor Service stopped.");
        }

        private void StartRigMonitor()
        {
            var app = new Application();
            app.Run(new MainWindow());
            LoggerService.LogDebug("RigMonitor Started as Service from StartRigMonitor. ");
            //app.MainWindow.

            if (app.MainWindow != null && app.MainWindow.IsActive)
            {
                app.MainWindow.ShowDialog();
                LoggerService.LogDebug("StartRigMonitor: app.MainWindow.Show() called ");
            }
        }

    }
}