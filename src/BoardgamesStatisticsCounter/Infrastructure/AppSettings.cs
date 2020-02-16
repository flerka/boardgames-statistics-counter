using System.Collections.Generic;

namespace BoardgamesStatisticsCounter.Infrastructure
{
    public class AppSettings
    {
        public string ConnectionString { get; set; } = default!;

        public string? AllowedChatIds { get; set; }

        public string AccessToken { get; set; } = default!;
    }
}