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
        public string Address { get; set; }

        public void SetAddress(string address)
        {
            Address = address;
        }

        public GeneralInfoModel GetGeneralInfo()
        {
            using (var wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var result = wc.DownloadString(BaseUrl + "user/" + Address);
                var generalInfo= new JavaScriptSerializer().Deserialize<GeneralInfoModel>(result);
                return generalInfo;
            }
        }

        public BalanceModel GetBalance()
        {
            using (var wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var result = wc.DownloadString(BaseUrl + "balance/" + Address);
                var balance = new JavaScriptSerializer().Deserialize<BalanceModel>(result);
                return balance;
            }
        }

        public ListOfWorkerModel GetListOfWorkers()
        {
            using (var wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var result = wc.DownloadString(BaseUrl + $"workers/{Address}");
                var data = new JavaScriptSerializer().Deserialize<ListOfWorkerModel>(result);
                return data;
            }
        }

        public WorkerLastReportedHashrateForWorkerModel GetLastReportedHashrateForWorker(string workerId)
        {
            using (var wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var result = wc.DownloadString(BaseUrl + $"reportedhashrate/{Address}/{workerId}");
                var data = new JavaScriptSerializer().Deserialize<WorkerLastReportedHashrateForWorkerModel>(result);
                return data;
            }
        }

        public List<WorkerDataViewModel> GetAllWorkersData()
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

    }
}