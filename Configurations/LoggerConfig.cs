using Serilog;

public static class LoggerConfig
{
    public static void ConfigureLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()

            // ✅ APPLICATION LOG
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(e => e.Properties.ContainsKey("LogType") &&
                                             e.Properties["LogType"].ToString().Contains("APP"))
                .WriteTo.File(LogPathHelper.GetPath("application"),
                    rollingInterval: RollingInterval.Infinite,
                    fileSizeLimitBytes: 10 * 1024 * 1024,
                    rollOnFileSizeLimit: true,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} | {Message}{NewLine}")
            )

            // ✅ REQUEST LOG
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(e => e.Properties.ContainsKey("LogType") &&
                                             e.Properties["LogType"].ToString().Contains("REQUEST"))
                .WriteTo.File(LogPathHelper.GetPath("request"),
                    rollingInterval: RollingInterval.Infinite,
                    fileSizeLimitBytes: 10 * 1024 * 1024,
                    rollOnFileSizeLimit: true,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} | {Message}{NewLine}")
            )

            // ✅ RESPONSE LOG
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(e => e.Properties.ContainsKey("LogType") &&
                                             e.Properties["LogType"].ToString().Contains("RESPONSE"))
                .WriteTo.File(LogPathHelper.GetPath("response"),
                    rollingInterval: RollingInterval.Infinite,
                    fileSizeLimitBytes: 10 * 1024 * 1024,
                    rollOnFileSizeLimit: true,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} | {Message}{NewLine}")
            )

            // ✅ ERROR LOG
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(e => e.Properties.ContainsKey("LogType") &&
                                             e.Properties["LogType"].ToString().Contains("ERROR"))
                .WriteTo.File(LogPathHelper.GetPath("error"),
                    rollingInterval: RollingInterval.Infinite,
                    fileSizeLimitBytes: 10 * 1024 * 1024,
                    rollOnFileSizeLimit: true,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} | {Message}{NewLine}{Exception}")
            )

            .CreateLogger();
    }
}