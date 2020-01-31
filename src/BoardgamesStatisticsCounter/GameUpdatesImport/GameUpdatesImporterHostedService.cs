using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace BoardgamesStatisticsCounter.GameUpdatesImport
{
    public class GameUpdatesImporterHostedService : BackgroundService
    {
        private TimeSpan DefaultDelayMs = 50 * 60000;
        private TimeSpan DefaultLongPollingTimeoutSeconds = 360;

        private readonly ITelegramBotClient _telegramBotClient;
        private readonly ILogger _logger;

        public GameUpdatesImporterHostedService(
            ITelegramBotClient telegramBotClient,
            ILogger logger)
        {
            _telegramBotClient = telegramBotClient;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var offset = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var updates = await _telegramBotClient.GetUpdatesAsync(offset, 0, DefaultLongPollingTimeoutSeconds, null, cancellationToken);
                    if (updates == null || updates.Length == 0)
                    {
                        await Task.Delay(DefaultDelayMs, cancellationToken);
                        continue;
                    }

                    foreach (var update in updates)
                    {
                        if (update.Message.Type == MessageType.Text)
                        {
                            // process text
                        }

                        offset = update.Id + 1;
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e, "An unhandled exception occured in GameUpdatesImporterHostedService");
                    await Task.Delay(DefaultDelayMs, cancellationToken);
                }
            }
        }
    }
}
