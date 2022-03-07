using System;
using System.Security.Authentication;
using Application.Common.Interfaces;
using Application.Job.DomainEvents;
using Domain;
using Infrastructure.Filters;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RabbitMQ.Client;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<NewJobReceivedDomainEventHandler>();
            services.AddDistributedMemoryCache();

            if (configuration.GetValue<bool>("UseInMemoryMessageBroker"))
            {
            }
            else
            {
                services.AddSingleton<IConnectionFactory>(_ => new ConnectionFactory
                {
                    Uri = new Uri(configuration["BrokerSettings:Host"]),
                    UserName = configuration["BrokerSettings:UserName"],
                    Password = configuration["BrokerSettings:Password"],
                    AmqpUriSslProtocols = SslProtocols.Tls12
                });

                services.AddTransient(sp => sp.GetRequiredService<IConnectionFactory>().CreateConnection());
            }

            services
                .AddControllers(options =>
                {
                    options.Filters.Add(new ApiExceptionFilter());
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.Formatting = Formatting.None;
                });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddHttpClient<IJobService, JobService>().SetHandlerLifetime(TimeSpan.FromMinutes(5));
            services.AddHttpClient<IJobResponseService, JobResponseService>().SetHandlerLifetime(TimeSpan.FromMinutes(5));
            services.AddTransient<IFileLoader, WebFileLoader>();

            services.AddHostedService<JobDequeueHostedService>();

            return services;
        }
    }
}
