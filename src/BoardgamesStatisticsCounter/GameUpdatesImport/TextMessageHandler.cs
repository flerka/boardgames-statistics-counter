using System.Threading;
using System.Threading.Tasks;
using BoardgamesStatisticsCounter.Infrastructure;
using MediatR;

namespace BoardgamesStatisticsCounter.GameUpdatesImport
{
    public class TextMessageHandler : IRequestHandler<TextMessage, OperationResult>
    {
        public Task<OperationResult> Handle(TextMessage request, CancellationToken cancellationToken)
        {
            return null;
        }
    }
}