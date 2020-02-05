using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using BoardgamesStatisticsCounter.Infrastructure.Results;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace BoardgamesStatisticsCounter.Infrastructure
{
    public class ExceptionMiddleware
    {
        private const string DefaultErrorMessage = "Internal Server Error occurred.";

        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Something went wrong");
                await HandleExceptionAsync(httpContext);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var errorDetails = new ErrorResult("internal_error", DefaultErrorMessage);
            var body = context.Response.Body;
            await JsonSerializer.SerializeAsync(body, errorDetails, errorDetails.GetType());
            await body.FlushAsync();
        }
    }
}