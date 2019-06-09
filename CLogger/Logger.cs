using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace CLogger
{
    public class Logger : ILogger
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        //.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build();

        public void LogInformation(LogLevels eventLevel, string information, Exception exInfo = null, params object[] values)
        {
            var connectionStringLogginDb = Configuration.GetSection("ConnectionStrings:LoggingDatabase");
            var customLoggingLevel = Configuration.GetSection("CustomLoggingLevel");
            var loggingTable = Configuration.GetSection("LoggingTable");

            ValidateMandatoryEntries(connectionStringLogginDb, customLoggingLevel, loggingTable);

            var columnOptions = GetColumnOptions();

            //Excluding redundant data
            //columnOptions.Properties.ExcludeAdditionalProperties = true;

            //Log in console to trace any Serilog issues
            Serilog.Debugging.SelfLog.Enable(msg => System.Diagnostics.Debug.WriteLine(msg));

            var levelSwitch = new Serilog.Core.LoggingLevelSwitch();
            levelSwitch.MinimumLevel = getLogEventLevel(customLoggingLevel.Value);
            var logLevel = getLogEventLevel(eventLevel.ToString());

            using (var log = GetLog(connectionStringLogginDb.Value, loggingTable.Value, columnOptions, levelSwitch))
            {
                if (log.IsEnabled(logLevel))
                {
                    if (exInfo != null)
                    {
                        log.Write(logLevel, exInfo, information, values);
                    }
                    else
                    {
                        log.Write(logLevel, information, values);
                    }
                }
                else
                {
                    var result = $"Logger: eventLevel:{nameof(logLevel)} is not enabled";
                    System.Diagnostics.Debug.WriteLine(result);
                }
            }
        }

        /// <summary>
        /// Gets a custom logger configuration
        /// </summary>
        /// <param name="connectionStringLoggingDb">connection string</param>
        /// <param name="loggingTable">name of the table</param>
        /// <param name="columnOptions">configuration of columns (excluded, additional, customized) in the table</param>
        /// <param name="levelSwitch">minimun level to log</param>
        /// <returns>custom logger configuration object</returns>
        private static Serilog.Core.Logger GetLog(string connectionStringLoggingDb,
            string loggingTable, Serilog.Sinks.MSSqlServer.ColumnOptions columnOptions, Serilog.Core.LoggingLevelSwitch levelSwitch)
        {
            return new LoggerConfiguration()
                           //.MinimumLevel.Information()
                           .MinimumLevel.ControlledBy(levelSwitch)
                            //.MinimumLevel.Override("SerilogDemo", Serilog.Events.LogEventLevel.Information)
                            .WriteTo.MSSqlServer(connectionStringLoggingDb, loggingTable,
                                    columnOptions: columnOptions,
                                    autoCreateSqlTable: false
                                    )
                           .CreateLogger();
        }

        /// <summary>
        /// Validates Mandatory config entries
        /// </summary>
        /// <param name="connectionStringLogginDb">Logging db Connection string</param>
        /// <param name="customLoggingLevel">custom logging level</param>
        /// <param name="loggingTable">Name of the log table</param>
        private void ValidateMandatoryEntries(IConfigurationSection connectionStringLoggingDb, IConfigurationSection customLoggingLevel, IConfigurationSection loggingTable)
        {
            if (string.IsNullOrEmpty(connectionStringLoggingDb.Value))
            {
                throw new ArgumentNullException("ConnectionStrings:LoggingDatabase null or empty");
            }

            if (string.IsNullOrEmpty(customLoggingLevel.Value))
            {
                throw new ArgumentNullException("CustomLoggingLevel null or empty");
            }

            if (string.IsNullOrEmpty(loggingTable.Value))
            {
                throw new ArgumentNullException("LoggingTable null or empty");
            }
        }

        /// <summary>
        /// Gets correspondant Serilog Log Event Level
        /// </summary>
        /// <param name="logLevel">name of the log level</param>
        /// <returns>LogEventLevel</returns>
        private Serilog.Events.LogEventLevel getLogEventLevel(string logLevel)
        {
            var level = Serilog.Events.LogEventLevel.Debug;
            try
            {
                level = (Serilog.Events.LogEventLevel)Enum.Parse(typeof(Serilog.Events.LogEventLevel), logLevel);
            }
            catch (Exception ex)
            {
                var result = $"CLogger.Logger.getLogEventLevel(): exception getting customlogging level, defaulted to Debug Level. logLevel:{logLevel}, exception:{ex}";
                System.Diagnostics.Debug.WriteLine(result);
            }
            return level;
        }

        /// <summary>
        /// Gets Custom Options (excluded, additional, customized)
        /// </summary>
        /// <returns>Column Options object</returns>
        private Serilog.Sinks.MSSqlServer.ColumnOptions GetColumnOptions()
        {
            return new Serilog.Sinks.MSSqlServer.ColumnOptions()
            {
                AdditionalDataColumns = new List<DataColumn>
                {
                    new DataColumn { ColumnName="Source", DataType = System.Type.GetType("System.String")}
                }
            };
        }


    }
}
