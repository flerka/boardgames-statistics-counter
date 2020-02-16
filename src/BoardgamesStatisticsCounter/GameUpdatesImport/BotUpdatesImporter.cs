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
        private readonly TimeSpan _defaultDelayBetweenRequests = TimeSpan.FromMinutes(10);
        private readonly TimeSpan _defaultLongPollingTimeout = TimeSpan.FromMinutes(1);

        private readonly IHostApplicationLifetime _hostApplicationLifetime; 
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly List<string>? _allowedChatIds;
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public BotUpdatesImporter(
            ITelegramBotClient telegramBotClient,
            IHostApplicationLifetime hostApplicationLifetime,
            ILogger logger,
            IMediator mediator,
            IOptions<AppSettings> clientConfig)
        {
            _allowedChatIds = string.IsNullOrEmpty(clientConfig.Value.AllowedChatIds)
                ? null
                : clientConfig.Value.AllowedChatIds.Split(',').ToList();

            _hostApplicationLifetime = hostApplicationLifetime;
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
                    var updates = await _telegramBotClient.GetUpdatesAsync(
                        offset,
                        100,
                        _defaultLongPollingTimeout.Seconds,
                        null,
                        cancellationToken);

                    var (unsuccessfulProcessed, successfulProcessed) = await ProcessUpdate(updates, cancellationToken);

                    _logger.Information($"BotUpdatesImporter.ExecuteAsync {successfulProcessed} messages processed successfully, failed to process {unsuccessfulProcessed} messages");
                    if (successfulProcessed == 0 || unsuccessfulProcessed > 0)
                    {
                        await Task.Delay(_defaultDelayBetweenRequests.Milliseconds, cancellationToken);
                        continue;
                    }

                    offset += updates.Length;
                }
                
                cancellationToken.ThrowIfCancellationRequested();
            }
            catch (Exception e)
            {
                _logger.Error(e, "BotUpdatesImporter.ExecuteAsync Some unhandled error occurred, stopping application.");
                _hostApplicationLifetime.StopApplication();
            }
        }

        private async Task<(int, int)> ProcessUpdate(Update[] updates, CancellationToken cancellationToken)
        {
            var total = updates.Length;
            var successfulProcessed = 0;

            foreach (var update in updates)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                if (!_allowedChatIds.Contains(update.Message.Chat.Id.ToString(CultureInfo.InvariantCulture)))
                {
                    _logger.Warning($"BotUpdatesImporter.ProcessUpdate message from unrecognized user - {update.Message.Chat.Id}");
                    continue;
                }

                if (update.Message.Type != MessageType.Text)
                {
                    _logger.Warning($"BotUpdatesImporter.ProcessUpdate unrecognized message type - {update.Message.Chat.Id}");
                    continue;
                }

                var result = await _mediator.Send(new TextMessage
                {
                    Message = update.Message.Text,
                    ChatId = update.Message.Chat.Id.ToString(CultureInfo.InvariantCulture),
                    MessageDateTime = update.Message.Date,
                });

                if (result.IsSuccesfull)
                {
                    successfulProcessed++;
                }
            }

            return (total - successfulProcessed, successfulProcessed);
        }
    }
}