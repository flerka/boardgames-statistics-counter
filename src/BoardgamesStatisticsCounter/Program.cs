using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace BoardgamesStatisticsCounter
{
    /// <summary>
    /// App entrypoing.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// App entrypoint.
        /// </summary>
        /// <param name="args">CLI arguments/flags.</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Create and configure a builder for webhost
        /// see https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0.
        /// </summary>
        /// <param name="args">CLI arguments/flags.</param>
        /// <returns>
        /// Returns builder that creates host - object that encapsulates an app's resources, such as DI,
        /// logging, configuration.
        /// </returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseSerilog().UseStartup<Startup>();
                });
    }
}
