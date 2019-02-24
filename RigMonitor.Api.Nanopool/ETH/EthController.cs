using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;
using RigMonitor.Models.DataModels.Miner;
using RigMonitor.Models.DataModels.Worker;
using RigMonitor.Models.ViewModels;


namespace RigMonitor.Api.Nanopool.ETH
{
    public class EthController
    {
        public EthController()
        {
            Address = ConfigurationSettings.AppSettings["Address"];
            BaseUrl = "https://api.nanopool.org/v1/eth/";
        }

        public string BaseUrl { get; set; }
        private string Address { get; set; }

        public void SetAddress(string address)
        {
            Address = address;
        }

        public GeneralInfoModel GetGeneralInfo()
        {
            try
            {
                using (var wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    var result = wc.DownloadString(BaseUrl + "user/" + Address);
                    var generalInfo = new JavaScriptSerializer().Deserialize<GeneralInfoModel>(result);
                    return generalInfo;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return new GeneralInfoModel();
        }

        public BalanceModel GetBalance()
        {
            try
            {
                using (var wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    var result = wc.DownloadString(BaseUrl + "balance/" + Address);
                    var balance = new JavaScriptSerializer().Deserialize<BalanceModel>(result);
                    return balance;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return new BalanceModel();
        }

        public ListOfWorkerModel GetListOfWorkers()
        {
            try
            {
                using (var wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    var result = wc.DownloadString(BaseUrl + $"workers/{Address}");
                    var data = new JavaScriptSerializer().Deserialize<ListOfWorkerModel>(result);
                    return data;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return new ListOfWorkerModel();
        }

        public WorkerLastReportedHashrateForWorkerModel GetLastReportedHashrateForWorker(string workerId)
        {
            try
            {
                using (var wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    var result = wc.DownloadString(BaseUrl + $"reportedhashrate/{Address}/{workerId}");
                    var data = new JavaScriptSerializer().Deserialize<WorkerLastReportedHashrateForWorkerModel>(result);
                    return data;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return new WorkerLastReportedHashrateForWorkerModel();
        }

        public List<WorkerDataViewModel> GetAllWorkersData()
        {
            try
            {
                //var workersList = GetListOfWorkers();
                var generalInfo = GetGeneralInfo();
                var workersData = generalInfo.Data.Workers.Select(worker => new WorkerDataViewModel(worker)).ToList();
                foreach (var workerData in workersData)
                {
                    var workerHashrate = GetLastReportedHashrateForWorker(workerData.Id);
                    if (workerHashrate.Status)
                    {
                        workerData.ReportedHashrate = workerHashrate.Data;
                    }
                }
                return workersData;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return new List<WorkerDataViewModel>();
        }

    }
}