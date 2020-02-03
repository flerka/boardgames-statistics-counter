using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BoardgamesStatisticsCounter.Infrastructure;
using Dapper;
using Dapper.Contrib.Extensions;
using MediatR;
using Microsoft.Extensions.Options;

namespace BoardgamesStatisticsCounter.GameUpdatesImport
{
    public class TextMessageHandler : IRequestHandler<TextMessage, OperationResult>
    {
        private readonly DbClientConfig _clientConfig;

        public TextMessageHandler(IOptions<DbClientConfig> clientConfig)
        {
            _clientConfig = clientConfig.Value;
        }
        
        public async Task<OperationResult> Handle(TextMessage request, CancellationToken cancellationToken)
        {
            using var connection = new SqlConnection(_clientConfig.ConnectionString);
            var getGameIdSql = @"INSERT INTO user_chats (chat_id)
                        VALUES(@ChatId),
                        ON CONFLICT (chat_id) DO UPDATE 
                        SET chat_id=EXCLUDED.chat_id
                        RETURNING id";
            var userId = (await connection.QueryAsync<int>(getGameIdSql, new { request.ChatId })).First();
            throw new NotImplementedException();
        }
    }
}