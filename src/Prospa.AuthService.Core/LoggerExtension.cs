using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;

namespace Prospa.AuthService.Core
{
    public static class ProspaLoggerExtension
    {
        public static Action<HostBuilderContext, LoggerConfiguration> ConfigureLogger =>
            (hostingContext, loggerConfiguration) =>
            {
                var env = hostingContext.HostingEnvironment;

                loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration)
                    .MinimumLevel.Information()
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails()
                    .Enrich.WithProperty("Service", hostingContext.HostingEnvironment.ApplicationName)
                    .WriteTo.Console();

                // Configure Sentry
                var sentryDsn = hostingContext.Configuration.GetSection("Sentry:Dsn").Value;
                if (!string.IsNullOrEmpty(sentryDsn))
                {
                    loggerConfiguration.WriteTo.Sentry(o =>
                    {
                        o.Dsn = sentryDsn; 
                        // When configuring for the first time, to see what the SDK is doing:
                       // o.Debug = true;
                        // Set TracesSampleRate to 1.0 to capture 100% of transactions for performance monitoring.
                        // We recommend adjusting this value in production.
                        o.TracesSampleRate = 1.0;
                        // Add additional Sentry configuration here if needed
                    });
                }

                // Other configurations...

            };
    }
}
