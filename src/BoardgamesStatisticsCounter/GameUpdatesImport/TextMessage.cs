using System;
using BoardgamesStatisticsCounter.Infrastructure;
using MediatR;

namespace BoardgamesStatisticsCounter.GameUpdatesImport
{
    public class TextMessage : IRequest<OperationResult>
    {
        public string Message { get; set; } = default!;
        
        public string ChatId { get; set; } = default!;
        
        public DateTime MessageDateTime { get; set; }
    }
}