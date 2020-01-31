using Microsoft.AspNetCore.Builder;

namespace BoardgamesStatisticsCounter.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
