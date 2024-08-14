using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors
{
    public class RequestGenericExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
        where TException : Exception
    {
        private readonly ILogger<TRequest> _logger;

        public RequestGenericExceptionHandler(ILogger<TRequest> logger)
        {
            _logger = logger;
        }

        public async Task Handle(TRequest request, TException exception, 
            RequestExceptionHandlerState<TResponse> state, CancellationToken cancellationToken)
        {
            string name = typeof(TRequest).Name;

            _logger.LogError("Request Exception {@name} {@message} {@request}", name, exception.Message, request);
        }
    }
}
