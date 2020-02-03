using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Hosting;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace BoardgamesStatisticsCounter.GameUpdatesImport
{
    public class GameUpdatesImporterHostedService : BackgroundService
    {
        private readonly TimeSpan _defaultDelayBetweenRequests = TimeSpan.FromMinutes(0);
        private readonly TimeSpan _defaultLongPollingTimeout = TimeSpan.FromMinutes(1);

        private readonly ITelegramBotClient _telegramBotClient;
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public GameUpdatesImporterHostedService(
            ITelegramBotClient telegramBotClient,
            ILogger logger,
            IMediator mediator)
        {
            _telegramBotClient = telegramBotClient;
            _logger = logger;
            _mediator = mediator;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var offset = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var updates = await _telegramBotClient.GetUpdatesAsync(
                    offset,
                    0,
                    _defaultLongPollingTimeout.Seconds,
                    null,
                    cancellationToken);

                if (updates == null || updates.Length == 0)
                {
                    await Task.Delay(_defaultDelayBetweenRequests.Milliseconds, cancellationToken);
                    continue;
                }

                foreach (var update in updates)
                {
                    if (update.Message.Type == MessageType.Text)
                    {
                        var textMessage = new TextMessage
                        {
                            Message = update.Message.Text,
                            ChatId = update.Message.Chat.Id.ToString(CultureInfo.InvariantCulture),
                            MessageDateTime = update.Message.Date,
                        };
                        await _mediator.Send(textMessage);
                    }

                    offset = update.Id + 1;
                }
            }
        }
    }
}