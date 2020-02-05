using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BoardgamesStatisticsCounter.Infrastructure;
using BoardgamesStatisticsCounter.Infrastructure.Results;
using Dapper;
using MediatR;
using Microsoft.Extensions.Options;
using Npgsql;
using Serilog;
using Sprache;

namespace BoardgamesStatisticsCounter.GameUpdatesImport
{
    public class TextMessageHandler : IRequestHandler<TextMessage, OperationResult<int>>
    {
        private readonly AppSettings _clientConfig;

        private readonly ILogger _logger;

        public TextMessageHandler(IOptions<AppSettings> clientConfig, ILogger logger)
        {
            _clientConfig = clientConfig.Value;
            _logger = logger;
        }
        
        public async Task<OperationResult<int>> Handle(TextMessage request, CancellationToken cancellationToken)
        {
            var parsedMessage = TextMessageGrammar.MessageParser.TryParse(request.Message);
            if (!parsedMessage.WasSuccessful)
            {
                _logger.Warning($"TextMessageHandler.Handle Failed to parse message {request.Message}");
                return new InvalidMessageFormatErrorResult();
            }

            var gameName = parsedMessage.Value.SelectMany(s => s).First();
            using var connection = new NpgsqlConnection(_clientConfig.ConnectionString);

            var insertUserAndGetIdSql = @"INSERT INTO user_chats (chat_id)
                        VALUES(@ChatId)
                        ON CONFLICT (chat_id) DO UPDATE 
                        SET chat_id=EXCLUDED.chat_id
                        RETURNING id";
            var userId = await connection.QueryFirstAsync<int>(insertUserAndGetIdSql, new { request.ChatId });

            var insertGameAndGetIdSql = @"INSERT INTO games (name)
                        VALUES(@Name)
                        ON CONFLICT (name) DO UPDATE 
                        SET name=EXCLUDED.name
                        RETURNING id";
            var gameId = await connection.QueryFirstAsync<int>(insertGameAndGetIdSql, new { Name = gameName });

            var insertUserGameSql = @"INSERT INTO user_games (game_id, user_id, game_datetime, players, score, winner)
                        VALUES(@GameId, @UserId, @GameDateTime, @Players, @Score, @Winner) RETURNING id";
            var userGameId = await connection.QueryFirstAsync<int>(
                insertUserGameSql,
                new 
                {
                    GameId = gameId,
                    UserId = userId,
                    GameDateTime = request.MessageDateTime,
                    Players = string.Empty,
                    Score = string.Empty,
                    Winner = string.Empty,
                });

            return userGameId;
        }
    }
}