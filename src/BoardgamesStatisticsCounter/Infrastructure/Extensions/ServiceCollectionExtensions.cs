using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System;
using System.Collections.Generic;
using Telegram.Bot;
using System;
using System.Linq;

using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using BoardgamesStatisticsCounter.Data.Migrations;

namespace BoardgamesStatisticsCounter.Infrastructure.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddSerilogLogging(this IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(new RenderedCompactJsonFormatter())
                .CreateLogger();

            services.AddSingleton<ILogger>(Log.Logger);
            return services;
        }

        internal static IServiceCollection AddTelegramBotClient(this IServiceCollection services)
        {
            var accessToken = Environment.GetEnvironmentVariable("ACCESS_TOKEN");
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new KeyNotFoundException("ACCESS_TOKEN not found");
            }

            return services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(accessToken));
        }

        internal static IServiceCollection AddFluentMigrator(this IServiceCollection services)
        {
            services.AddFluentMigratorCore()
                .ConfigureRunner(rb => 
                    rb.AddPostgres()
                    .WithGlobalConnectionString("Data Source=test.db")
                    .ScanIn(typeof(AddBaseDatabaseStructure).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false);
        }
    }
}