using System;
using RigMonitor.Models.ViewModels;

namespace RigMonitor.Models.DataModels
{
    public class EthNanopoolControl
    {
        public EthNanopoolControl Prev { get; set; }
        public EthNanopoolControl Next { get; set; }
        public WorkerDataViewModel Worker { get; set; }
        public DateTime TimeMoment { get; set; }
        public DateTime LastShare { get; set; }
        public double CurrentCalculatedHashrate { get; set; } //Worker Current Calculated Hashrate
        public double ReportedHashrate { get; set; } //Worker Reported Hashrate

        public EthNanopoolControl(WorkerDataViewModel worker)
        {
            Worker = worker;
        }
        public EthNanopoolControl(EthNanopoolControl data)
        {
            Worker = data?.Worker;
            Prev = data?.Prev;
            Next = data?.Next;
            TimeMoment = data?.TimeMoment ?? DateTime.Now;
            LastShare = data?.LastShare ?? Prev?.LastShare ?? DateTime.Now;
            CurrentCalculatedHashrate = data?.CurrentCalculatedHashrate ?? 0;
            ReportedHashrate = data?.ReportedHashrate ?? 0;
        }
    }
}