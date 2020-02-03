using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BoardgamesStatisticsCounter.Infrastructure;
using Dapper;
using MediatR;
using Microsoft.Extensions.Options;
using Sprache;

namespace BoardgamesStatisticsCounter.GameUpdatesImport
{
    public class TextMessageHandler : IRequestHandler<TextMessage, OperationResult>
    {
        private readonly AppSettings _clientConfig;

        public TextMessageHandler(IOptions<AppSettings> clientConfig)
        {
            _clientConfig = clientConfig.Value;
        }
        
        public async Task<OperationResult> Handle(TextMessage request, CancellationToken cancellationToken)
        {
            var gameName = TextMessageGrammar.MessageParser.TryParse(request.Message).Value.SelectMany(s => s).First();
            using var connection = new SqlConnection(_clientConfig.ConnectionString);

            var insertUserAndGetIdSql = @"INSERT INTO user_chats (chat_id)
                        VALUES(@ChatId),
                        ON CONFLICT (chat_id) DO UPDATE 
                        SET chat_id=EXCLUDED.chat_id
                        RETURNING id";
            var userId = await connection.QueryFirstAsync<int>(insertUserAndGetIdSql, new { request.ChatId });

            var insertGameAndGetIdSql = @"INSERT INTO games (name)
                        VALUES(@Name),
                        ON CONFLICT (name) DO UPDATE 
                        SET name=EXCLUDED.name
                        RETURNING id";
            var gameId = await connection.QueryFirstAsync<int>(insertGameAndGetIdSql, new { Name = gameName });

            var insertUserGameSql = @"INSERT INTO user_games (game_id, user_id, game_datetime, players, score, winner)
                        VALUES(@GameId, @UserId, @GameDateTime, @Players, @Score, @Winner)";
            await connection.ExecuteAsync(
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

            return new OperationResult();
        }
    }
}