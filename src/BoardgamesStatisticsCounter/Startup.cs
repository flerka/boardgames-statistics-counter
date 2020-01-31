using System;
using System.Collections.Generic;

using BoardgamesStatisticsCounter.Infrastructure;
using BoardgamesStatisticsCounter.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace BoardgamesStatisticsCounter
{
    /// <summary>
    /// Startup allows to register app dependencies and configure middleware.
    /// </summary>
    public sealed class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940.
        /// </summary>
        /// <param name="services">Service collection.</param>
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSerilogLogging();
            services.AddControllers();
            services.AddCors();
            services.AddFluentMigrator();
        } 

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Middleware builder.</param>
        public static void Configure(IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging();
            app.UseCustomExceptionMiddleware();
            app.UseCors();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
