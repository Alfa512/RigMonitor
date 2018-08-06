namespace RigMonitor.Common
{
    public class LoggerConfiguration
    {
        /// <summary>
        /// Конфигурация логера
        /// </summary>
        public LoggerConfiguration()
        {
            Directories = new string[0];
        }

        /// <summary>
        /// Папки, куда пишутся логи
        /// </summary>
        public string[] Directories { get; set; }
    }
}
