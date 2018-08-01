namespace RigMonitor.Models.DataModels
{
    public class NanopoolEth
    {
        public bool Status { get; set; } //Response Status
        public string Address { get; set; } //Miner account
        public string Error { get; set; } //Error Description
    }
}