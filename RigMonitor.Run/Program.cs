using System;
using System.ServiceProcess;
using System.Threading;
using System.Windows;
using RigMonitor.Services;

namespace RigMonitor.Run
{
    class RigMonitor : ServiceBase
    {
        public LoggerService LoggerService;

        static void Main(string[] args)
        {
            if (args.Length == 1 && (args[0].Equals("--console") || args[0].Equals("-c")))
            {
                new RigMonitor().ConsoleRun();
            }
            else
            {
                ServiceBase.Run(new RigMonitorRunService());
            }
        }

        private void ConsoleRun()
        {
            var rigRestarter = new Thread(StartRigMonitor);
            LoggerService = new LoggerService("ConsoleRunLogger");
            try
            {
                Console.WriteLine(string.Format("{0}::starting...", GetType().FullName));

                OnStart(null);
                rigRestarter.SetApartmentState(ApartmentState.STA);
                rigRestarter.Start();

                Console.WriteLine(string.Format("{0}::ready (ENTER to exit)", GetType().FullName));
                Console.ReadLine();

                OnStop();

                Console.WriteLine(string.Format("{0}::stopped", GetType().FullName));
            }
            catch (Exception e)
            {
                LoggerService.LogError("RigMonitor start as console failed...");
                LoggerService.LogError(e);
                Console.WriteLine("RigMonitor start as console failed...");
                Console.WriteLine(e);
                Console.ReadLine();
                rigRestarter.Abort();
            }

        }
        private void StartRigMonitor()
        {
            try
            {
                var app = new Application();
                app.Run(new MainWindow());
            }
            catch (Exception e)
            {
                LoggerService.LogError("Open RigMonitor window failed...");
                LoggerService.LogError(e);
                Console.WriteLine("Open RigMonitor window failed...");
                Console.WriteLine(e);
            }

        }

    }
}
