using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviours
{
    public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly Stopwatch _timer;
        private readonly ILogger<TRequest> _logger;
        private const int WARNING_BENCHMARK_IN_MILLISECONDS = 4000;

        public PerformanceBehaviour(ILogger<TRequest> logger)
        {
            _timer = new Stopwatch();

            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            if (elapsedMilliseconds <= WARNING_BENCHMARK_IN_MILLISECONDS)
            {
                return response;
            }

            var requestName = typeof(TRequest).Name;

            _logger.LogWarning("Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds)", requestName, elapsedMilliseconds);

            return response;
        }
    }
}
