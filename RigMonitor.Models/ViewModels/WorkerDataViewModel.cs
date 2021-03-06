﻿using System;
using System.Globalization;
using RigMonitor.Models.DataModels.Miner;

namespace RigMonitor.Models.ViewModels
{
    public class WorkerDataViewModel
    {
        public WorkerDataViewModel(string id, int uid, double currentHashrate, double reportedHashrate, DateTime lastShare, int rating, double avgH1, double avgH3, double avgH6, double avgH12, double avgH24)
        {
            Id = id;
            Uid = uid;
            CurrentHashrate = currentHashrate;
            ReportedHashrate = reportedHashrate;
            LastShare = lastShare;
            Rating = rating;
            Avg_h1 = avgH1;
            Avg_h3 = avgH3;
            Avg_h6 = avgH6;
            Avg_h12 = avgH12;
            Avg_h24 = avgH24;
        }

        public WorkerDataViewModel(WorkerData workerData)
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            provider.NumberGroupSeparator = ",";
            provider.NumberGroupSizes = new [] { 3 };

            Id = workerData.Id;
            Uid = workerData.Uid;
            try
            {
                CurrentHashrate = Convert.ToDouble(workerData.Hashrate, provider);
            }
            catch (FormatException)
            {
                CurrentHashrate = 0;
            }

            try
            {
                ReportedHashrate = Convert.ToDouble(workerData.ReportedHashrate, provider);
            }
            catch (FormatException)
            {
                ReportedHashrate = 0;
            }
            
            LastShare = new DateTime().AddSeconds(workerData.LastShare);
            LastShareString = LastShare.ToShortTimeString();
            Rating = workerData.Rating;

            try
            {
                Avg_h1 = Convert.ToDouble(workerData.H1, provider);
            }
            catch (FormatException)
            {
                Avg_h1 = 0;
            }

            try
            {
                Avg_h3 = Convert.ToDouble(workerData.H3, provider);
            }
            catch (FormatException)
            {
                Avg_h3 = 0;
            }

            try
            {
                Avg_h6 = Convert.ToDouble(workerData.H6, provider);
            }
            catch (FormatException)
            {
                Avg_h6 = 0;
            }
            
            try
            {
                Avg_h12 = Convert.ToDouble(workerData.H12, provider);
            }
            catch (FormatException)
            {
                Avg_h12 = 0;
            }
            
            try
            {
                Avg_h24 = Convert.ToDouble(workerData.H24, provider);
            }
            catch (FormatException)
            {
                Avg_h24 = 0;
            }
        }

        public WorkerDataViewModel()
        {
        }

        public string Id { get; set; }
        public int Uid { get; set; }
        public double CurrentHashrate { get; set; } //Worker Current Hashrate
        public double ReportedHashrate { get; set; } //Worker Reported Hashrate
        public DateTime LastShare { get; set; } //Last Share Date of Worker
        public string LastShareString { get; set; } //Last Share Date of Worker
        public int Rating { get; set; } //Last Share Date of Worker
        public double Avg_h1 { get; set; }
        public double Avg_h3 { get; set; }
        public double Avg_h6 { get; set; }
        public double Avg_h12 { get; set; }
        public double Avg_h24 { get; set; }
    }
}