using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Job.DomainEvents;
using Domain;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Infrastructure.Services
{
    public class JobDequeueHostedService : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IDistributedCache _distributedCache;
        private readonly NewJobReceivedDomainEventHandler _jobReceivedDomainEventHandler;
        private readonly ILogger<JobDequeueHostedService> _logger;

        public JobDequeueHostedService(IConnection connection,
            IDistributedCache distributedCache,
            NewJobReceivedDomainEventHandler jobReceivedDomainEventHandler,
            ILogger<JobDequeueHostedService> logger)
        {
            _connection = connection;

            _connection.CreateModel();
            _distributedCache = distributedCache;
            _jobReceivedDomainEventHandler = jobReceivedDomainEventHandler;
            _logger = logger;
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(
                queue: "task_queue",
                durable: true,
                exclusive: false,
                autoDelete: false);

            _channel.BasicQos(0, 1, true);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Run(() =>
                    {
                        var consumer = new EventingBasicConsumer(_channel);

                        consumer.Received += async (model, ea) =>
                        {

                            var storedToken = await _distributedCache.GetStringAsync("token", stoppingToken);
                            var currentJobJson =
                                await _distributedCache.GetStringAsync(CacheKeys.CURRENT_RUNNING_JOBS, stoppingToken);

                            var consumerObj = (EventingBasicConsumer)model;

                            if (storedToken == null || currentJobJson != null)
                            {
                                _logger.LogInformation("User is not authenticated, retrying...");

                                await Task.Delay(5000, stoppingToken);

                                consumerObj?.Model.BasicNack(ea.DeliveryTag, false, true);

                                return;
                            }

                            // {"images": [{"id": 0, "url": "http://13.229.247.193/wp-content/uploads/CustomDirectory/20d393f65a9807c9.jpg"}, {"id": 1, "url": "http://13.229.247.193/wp-content/uploads/CustomDirectory/ef901640466be2cf.jpg"}, {"id": 2, "url": "http://13.229.247.193/wp-content/uploads/CustomDirectory/610119265566d.jpg"}]}

                            var body = ea.Body.ToArray();
                            var message = Encoding.UTF8.GetString(body);

                            _logger.LogInformation("[Ack] message from queue, {@payload}", message);

                            try
                            {
                                var newJobReceivedDomainEvent = JsonConvert.DeserializeObject<NewJobReceivedDomainEvent>(message);
                                await _jobReceivedDomainEventHandler.Handle(newJobReceivedDomainEvent);
                                consumerObj?.Model.BasicAck(ea.DeliveryTag, false);
                            }
                            catch
                            {
                                // ignored
                            }
                        };

                        _channel.BasicConsume(queue: "task_queue", autoAck: false, consumer: consumer);
                    }, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");
            _channel.Dispose();
            _connection.Dispose();
            await base.StopAsync(stoppingToken);
        }
    }
}