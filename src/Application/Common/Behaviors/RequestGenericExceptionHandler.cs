using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var name = typeof(TRequest).Name;

            _logger.LogError("Request Exception {@Request}", name, exception.Message, request);
        }
    }
}
