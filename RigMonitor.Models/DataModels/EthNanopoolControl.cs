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

    }
}