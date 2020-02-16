using System.Reflection;
using BoardgamesStatisticsCounter.GameUpdatesImport;
using BoardgamesStatisticsCounter.Infrastructure;
using BoardgamesStatisticsCounter.Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace BoardgamesStatisticsCounter
{
    /// <summary>
    /// Startup allows registering app dependencies and configure middleware.
    /// </summary>
    public sealed class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Middleware builder.</param>
        public static void Configure(IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseCors();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940.
        /// </summary>
        /// <param name="services">Service collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSerilogLogging();
            services.Configure<AppSettings>(_configuration.GetSection("AS"));

            services.AddAppHealthChecks();
            services.AddControllers();
            services.AddCors();
            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);
            services.AddFluentMigrator();
            services.AddTelegramBotClient();
            services.AddHostedService<BotUpdatesImporter>();

            services.ApplyMigrations();
        }
    }
}
