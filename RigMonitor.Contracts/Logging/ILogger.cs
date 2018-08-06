using System;
using RigMonitor.Common;

namespace RigMonitor.Contracts.Logging
{
    public interface ILogger
    {
        LoggerConfiguration Configuration { get; }

        /// <summary>
        /// Записать сообщение с уровнем важности Debug
        /// </summary>
        /// <param name="message">Сообщение</param>
        void LogDebug(string message);

        /// <summary>
        /// Записать сообщение (на основе строки форматирования) с уровнем важности Debug
        /// </summary>
        /// <param name="format">Строка форматирования</param>
        /// <param name="args">Аргументы для форматирования</param>
        void LogDebug(string format, params object[] args);

        /// <summary>
        /// Записать сообщение с уровнем важности Info
        /// </summary>
        /// <param name="message">Сообщение</param>
        void LogInfo(string message);

        /// <summary>
        /// Записать сообщение (на основе строки форматирования) с уровнем важности Info
        /// </summary>
        /// <param name="format">Строка форматирования</param>
        /// <param name="args">Аргументы для форматирования</param>
        void LogInfo(string format, params object[] args);

        /// <summary>
        /// Записать сообщение с уровнем важности Warning
        /// </summary>
        /// <param name="message">Сообщение</param>
        void LogWarning(string message);

        /// <summary>
        /// Записать сообщение (на основе строки форматирования) с уровнем важности Warning
        /// </summary>
        /// <param name="format">Строка форматирования</param>
        /// <param name="args">Аргументы для форматирования</param>
        void LogWarning(string format, params object[] args);

        /// <summary>
        /// Записать сообщение с уровнем важности Error
        /// </summary>
        /// <param name="message">Сообщение</param>
        void LogError(string message);

        /// <summary>
        /// Записать сообщение (на основе строки форматирования) с уровнем важности Error
        /// </summary>
        /// <param name="format">Строка форматирования</param>
        /// <param name="args">Аргументы для форматирования</param>
        void LogError(string format, params object[] args);

        /// <summary>
        /// Записать сообщение (на основе исключения) с уровнем важности Error
        /// </summary>
        /// <param name="ex">Исключение</param>
        void LogError(Exception ex);

        /// <summary>
        /// Записать сообщение (на основе исключения и сообщения) с уровнем важности Error
        /// </summary>
        /// <param name="ex">Исключение</param>
        /// <param name="message">Сообщение</param>
        void LogError(Exception ex, string message);

        /// <summary>
        /// Записать сообщение (на основе исключения и строки форматирования) с уровнем важности Error
        /// </summary>
        /// <param name="ex">Исключение</param>
        /// <param name="format">Строка форматирования</param>
        /// <param name="args">Аргументы для форматирования</param>
        void LogError(Exception ex, string format, params object[] args);

        /// <summary>
        /// Записать сообщение с уровнем важности Fatal
        /// </summary>
        /// <param name="message">Сообщение</param>
        void LogFatal(string message);

        /// <summary>
        /// Записать сообщение (на основе строки форматирования) с уровнем важности Fatal
        /// </summary>
        /// <param name="format">Строка форматирования</param>
        /// <param name="args">Аргументы для форматирования</param>
        void LogFatal(string format, params object[] args);

        /// <summary>
        /// Записать сообщение (на основе исключения) с уровнем важности Fatal
        /// </summary>
        /// <param name="ex">Исключение</param>
        void LogFatal(Exception ex);

        /// <summary>
        /// Записать сообщение (на основе исключения и сообщения) с уровнем важности Fatal
        /// </summary>
        /// <param name="ex">Исключение</param>
        /// <param name="message">Сообщение</param>
        void LogFatal(Exception ex, string message);

        /// <summary>
        /// Записать сообщение (на основе исключения и строки форматирования) с уровнем важности Fatal
        /// </summary>
        /// <param name="ex">Исключение</param>
        /// <param name="format">Строка форматирования</param>
        /// <param name="args">Аргументы для форматирования</param>
        void LogFatal(Exception ex, string format, params object[] args);
    }
}
