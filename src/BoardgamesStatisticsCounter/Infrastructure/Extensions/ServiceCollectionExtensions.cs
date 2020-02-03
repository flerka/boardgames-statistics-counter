using System;
using System.Collections.Generic;
using BoardgamesStatisticsCounter.Data.Migrations;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Telegram.Bot;

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
            var serviceProvider = services.BuildServiceProvider(false);
            using var scope = serviceProvider.CreateScope();
            var clientConfig = serviceProvider.GetRequiredService<IOptions<DbClientConfig>>();
            
            return services.AddFluentMigratorCore()
                .ConfigureRunner(rb =>
                    rb.AddPostgres()
                        .WithGlobalConnectionString(clientConfig.Value.ConnectionString)
                        .ScanIn(typeof(Startup).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole());
        }

        internal static IServiceCollection ApplyMigrations(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider(false);
            using var scope = serviceProvider.CreateScope();
            
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();

            return services;
        }
    }
}