using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using NLog.Targets.Wrappers;
using RigMonitor.Common;
using ILogger = RigMonitor.Contracts.Logging.ILogger;

namespace RigMonitor.Services
{
    public class LoggerService :  ILogger
    {
        private readonly string _loggerName;

        /// <summary>
        /// Логгер
        /// </summary>
        private readonly Logger _logger;

        /// <summary>
        /// Пытается получить ресурс для вывода лога типа T
        /// </summary>
        /// <param name="target">Ресурс для вывода лога</param>
        /// <returns>Hесурс для вывода лога типа T</returns>
        private static T GetConcreteTarget<T>(Target target) where T : Target
        {
            if (target == null)
            {
                return null;
            }

            if (target is T)
            {
                return (T)target;
            }

            if (target is WrapperTargetBase)
            {
                return GetConcreteTarget<T>(((WrapperTargetBase)target).WrappedTarget);
            }

            return null;
        }

        #region Implementation of ILogger

        /// <summary>
        /// Конфигурация логера
        /// </summary>
        public LoggerConfiguration Configuration
        {
            get
            {
                if (_logger == null)
                {
                    return new LoggerConfiguration();
                }

                List<string> directories = new List<string>();
                foreach (LoggingRule rule in _logger.Factory.Configuration.LoggingRules.Where(r => r.NameMatches(_loggerName)).ToArray())
                {
                    if (rule.Targets == null || rule.Targets.Count == 0)
                    {
                        continue;
                    }

                    foreach (Target target in rule.Targets)
                    {
                        FileTarget fileTarget = GetConcreteTarget<FileTarget>(target);
                        if (fileTarget == null)
                        {
                            continue;
                        }

                        SimpleLayout fileTargetNameLayout = fileTarget.FileName as SimpleLayout;
                        string logFileName = fileTargetNameLayout.Render(new LogEventInfo(LogLevel.Off, _loggerName, string.Empty));
                        string directoryName = Path.GetDirectoryName(logFileName);
                        if (!string.IsNullOrEmpty(directoryName))
                        {
                            directories.Add(directoryName);
                        }
                    }
                }

                return new LoggerConfiguration
                {
                    Directories = directories.Distinct(StringComparer.OrdinalIgnoreCase).ToArray()
                };
            }
        }

        /// <summary>
        /// Записать сообщение в лог
        /// </summary>
        /// <param name="eventInfo">Данные о логируемух данных</param>
        private void Log(LogEventInfo eventInfo)
        {
            if (_logger != null)
            {
                _logger.Log(typeof(LoggerService), eventInfo);
            }
        }

        /// <summary>
        /// Создаёт логер на основе логера NLog
        /// </summary>
        /// <param name="loggerName">Имя логера</param>
        public LoggerService(string loggerName)
        {
            var config = new LoggingConfiguration();

            var logfile = new FileTarget("logfile") { FileName = $"Logs/{loggerName}.log" };
            var logconsole = new ConsoleTarget("logconsole");

            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            _loggerName = loggerName;
            _logger = LogManager.GetLogger(_loggerName);

            _logger.Factory.Configuration = config;
        }

        /// <summary>
        /// Записать сообщение с уровнем важности Debug
        /// </summary>
        /// <param name="message">Сообщение</param>
        public void LogDebug(string message)
        {
            Log(LogEventInfo.Create(LogLevel.Debug, _loggerName, message));
        }

        /// <summary>
        /// Записать сообщение (на основе строки форматирования) с уровнем важности Debug
        /// </summary>
        /// <param name="format">Строка форматирования</param>
        /// <param name="args">Аргументы для форматирования</param>
        public void LogDebug(string format, params object[] args)
        {
            LogDebug(string.Format(format, args));
        }

        /// <summary>
        /// Записать сообщение с уровнем важности Info
        /// </summary>
        /// <param name="message">Сообщение</param>
        public void LogInfo(string message)
        {
            Log(LogEventInfo.Create(LogLevel.Info, _loggerName, message));
        }

        /// <summary>
        /// Записать сообщение (на основе строки форматирования) с уровнем важности Info
        /// </summary>
        /// <param name="format">Строка форматирования</param>
        /// <param name="args">Аргументы для форматирования</param>
        public void LogInfo(string format, params object[] args)
        {
            LogInfo(string.Format(format, args));
        }

        /// <summary>
        /// Записать сообщение с уровнем важности Warning
        /// </summary>
        /// <param name="message">Сообщение</param>
        public void LogWarning(string message)
        {
            Log(LogEventInfo.Create(LogLevel.Warn, _loggerName, message));
        }

        /// <summary>
        /// Записать сообщение (на основе строки форматирования) с уровнем важности Warning
        /// </summary>
        /// <param name="format">Строка форматирования</param>
        /// <param name="args">Аргументы для форматирования</param>
        public void LogWarning(string format, params object[] args)
        {
            LogWarning(string.Format(format, args));
        }

        /// <summary>
        /// Записать сообщение с уровнем важности Error
        /// </summary>
        /// <param name="message">Сообщение</param>
        public void LogError(string message)
        {
            Log(LogEventInfo.Create(LogLevel.Error, _loggerName, message));
        }

        /// <summary>
        /// Записать сообщение (на основе строки форматирования) с уровнем важности Error
        /// </summary>
        /// <param name="format">Строка форматирования</param>
        /// <param name="args">Аргументы для форматирования</param>
        public void LogError(string format, params object[] args)
        {
            LogError(string.Format(format, args));
        }

        /// <summary>
        /// Записать сообщение (на основе исключения) с уровнем важности Error
        /// </summary>
        /// <param name="ex">Исключение</param>
        public void LogError(Exception ex)
        {
            Log(LogEventInfo.Create(LogLevel.Error, _loggerName, ex.Message, ex));
            Log(LogEventInfo.Create(LogLevel.Error, _loggerName, ex.StackTrace, ex));
        }

        /// <summary>
        /// Записать сообщение (на основе исключения и сообщения) с уровнем важности Error
        /// </summary>
        /// <param name="ex">Исключение</param>
        /// <param name="message">Сообщение</param>
        public void LogError(Exception ex, string message)
        {
            Log(LogEventInfo.Create(LogLevel.Error, _loggerName, message, ex));
            Log(LogEventInfo.Create(LogLevel.Error, _loggerName, ex?.StackTrace, ex));

        }

        /// <summary>
        /// Записать сообщение (на основе исключения и строки форматирования) с уровнем важности Error
        /// </summary>
        /// <param name="ex">Исключение</param>
        /// <param name="format">Строка форматирования</param>
        /// <param name="args">Аргументы для форматирования</param>
        public void LogError(Exception ex, string format, params object[] args)
        {
            LogError(ex, string.Format(format, args));
        }

        /// <summary>
        /// Записать сообщение с уровнем важности Fatal
        /// </summary>
        /// <param name="message">Сообщение</param>
        public void LogFatal(string message)
        {
            Log(LogEventInfo.Create(LogLevel.Fatal, _loggerName, message));
        }

        /// <summary>
        /// Записать сообщение (на основе строки форматирования) с уровнем важности Fatal
        /// </summary>
        /// <param name="format">Строка форматирования</param>
        /// <param name="args">Аргументы для форматирования</param>
        public void LogFatal(string format, params object[] args)
        {
            LogFatal(string.Format(format, args));
        }

        /// <summary>
        /// Записать сообщение (на основе исключения) с уровнем важности Fatal
        /// </summary>
        /// <param name="ex">Исключение</param>
        public void LogFatal(Exception ex)
        {
            Log(LogEventInfo.Create(LogLevel.Fatal, _loggerName, ex.Message, ex));
        }

        /// <summary>
        /// Записать сообщение (на основе исключения и сообщения) с уровнем важности Fatal
        /// </summary>
        /// <param name="ex">Исключение</param>
        /// <param name="message">Сообщение</param>
        public void LogFatal(Exception ex, string message)
        {
            Log(LogEventInfo.Create(LogLevel.Error, _loggerName, message, ex));
        }

        /// <summary>
        /// Записать сообщение (на основе исключения и строки форматирования) с уровнем важности Fatal
        /// </summary>
        /// <param name="ex">Исключение</param>
        /// <param name="format">Строка форматирования</param>
        /// <param name="args">Аргументы для форматирования</param>
        public void LogFatal(Exception ex, string format, params object[] args)
        {
            LogFatal(ex, string.Format(format, args));
        }

        #endregion
    }
}