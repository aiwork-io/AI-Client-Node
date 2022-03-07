using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public abstract class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
        private Task _executingTask;
        private Timer _timer;

        protected TimedHostedService(ILogger<TimedHostedService> logger, int timerInSeconds = 5)
        {
            _logger = logger;
            TimerInSeconds = timerInSeconds;
        }

        private int TimerInSeconds { get; }

        public void Dispose()
        {
            _stoppingCts.Cancel();
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(ExecuteTask, null, TimeSpan.FromSeconds(TimerInSeconds), TimeSpan.FromMilliseconds(-1));

            return Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            if (_executingTask == null)
            {
                return;
            }

            try
            {
                _stoppingCts.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        private void ExecuteTask(object state)
        {
            _timer?.Change(Timeout.Infinite, 0);
            _executingTask = ExecuteTaskAsync(_stoppingCts.Token);
        }

        private async Task ExecuteTaskAsync(CancellationToken stoppingToken)
        {
            try
            {
                await RunJobAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            _timer.Change(TimeSpan.FromSeconds(TimerInSeconds), TimeSpan.FromMilliseconds(-1));
        }

        protected abstract Task RunJobAsync(CancellationToken stoppingToken);
    }
}