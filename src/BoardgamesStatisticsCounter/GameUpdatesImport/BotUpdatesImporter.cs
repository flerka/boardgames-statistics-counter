using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BoardgamesStatisticsCounter.Infrastructure;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BoardgamesStatisticsCounter.GameUpdatesImport
{
    public class BotUpdatesImporter : BackgroundService
    {
        private readonly TimeSpan _defaultDelayBetweenRequests = TimeSpan.FromMinutes(0);
        private readonly TimeSpan _defaultLongPollingTimeout = TimeSpan.FromMinutes(1);

        private readonly ITelegramBotClient _telegramBotClient;
        private readonly List<string> _allowedChatIds;
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public BotUpdatesImporter(
            ITelegramBotClient telegramBotClient,
            ILogger logger,
            IMediator mediator,
            IOptions<AppSettings> clientConfig)
        {
            _allowedChatIds = string.IsNullOrEmpty(clientConfig.Value.AllowedChatIds)
                ? new List<string>()
                : clientConfig.Value.AllowedChatIds.Split(',').ToList();

            _telegramBotClient = telegramBotClient;
            _logger = logger;
            _mediator = mediator;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
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

                    (int unsuccessfulProcessed, int successfulProcessed) = await ProcessUpdate(updates);

                    _logger.Information($"BotUpdatesImporter.ExecuteAsync {successfulProcessed} messages processed successfully, failed to process {unsuccessfulProcessed} messages");
                    if (successfulProcessed == 0 || unsuccessfulProcessed > 0)
                    {
                        await Task.Delay(_defaultDelayBetweenRequests.Milliseconds, cancellationToken);
                        continue;
                    }

                    offset += updates.Length;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "BotUpdatesImporter.ExecuteAsync failed");
            }
        }

        private async Task<(int, int)> ProcessUpdate(Update[] updates)
        {
            var unsuccessfulProcessed = 0;
            var successfulProcessed = 0;

            foreach (var update in updates)
            {
                if (!_allowedChatIds.Contains(update.Message.Chat.Id.ToString(CultureInfo.InvariantCulture)))
                {
                    _logger.Warning($"BotUpdatesImporter.ProcessUpdate message from unrecognized user - {update.Message.Chat.Id}");
                    unsuccessfulProcessed++;
                    continue;
                }

                if (update.Message.Type != MessageType.Text)
                {
                    _logger.Warning($"BotUpdatesImporter.ProcessUpdate unrecognized message type - {update.Message.Chat.Id}");
                    unsuccessfulProcessed++;
                    continue;
                }

                await _mediator.Send(new TextMessage
                {
                    Message = update.Message.Text,
                    ChatId = update.Message.Chat.Id.ToString(CultureInfo.InvariantCulture),
                    MessageDateTime = update.Message.Date,
                });
                successfulProcessed++;
            }

            return (unsuccessfulProcessed, successfulProcessed);
        }
    }
}