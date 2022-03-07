using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Application.Common.Models;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using Application.Common.Interfaces;
using Domain;
using Microsoft.Extensions.Caching.Distributed;
using RabbitMQ.Client;

namespace Services.Controllers
{
    public class JobController : ApiController
    {
        private readonly IModel _channel;
        private readonly IDistributedCache _distributedCache;
        private readonly IJobResponseService _jobResponseService;

        public JobController(IDistributedCache distributedCache,
            IConnection connection,
            IJobResponseService jobResponseService)
        {
            _distributedCache = distributedCache;
            _jobResponseService = jobResponseService;

            connection.CreateModel();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(
                queue: "task_queue",
                durable: true,
                exclusive: false,
                autoDelete: false);
        }

        [HttpPost("completed")]
        public async Task<IActionResult> JobCompleted([FromBody] JobResponse jobResponse)
        {
            var token = await _distributedCache.GetStringAsync("token");

            if (token == null)
            {
                throw new Exception("Token not found");
            }

            Console.WriteLine("============");

            if (jobResponse.Data != null)
            {
                await _distributedCache.SetStringAsync(
                    jobResponse.JobContext.ProjectId,
                    JsonConvert.SerializeObject(jobResponse)
                );

                foreach (var box in jobResponse.Data?.Object.Take(1).OrderByDescending(box => box.Score))
                {
                    Console.WriteLine($"- {box.Category} detected with score of {box.Score} [{box.BBox.Select(item => item.ToString(CultureInfo.InvariantCulture)).Aggregate((prev, next) => $"{prev}, {next}")}]");
                }
            }

            var currentJobJson = await _distributedCache.GetStringAsync(CacheKeys.CURRENT_RUNNING_JOBS);

            var currentJobId = await _distributedCache.GetStringAsync(CacheKeys.CURRENT_JOB_ID);

            var allJobIds = JsonConvert.DeserializeObject<IList<string>>(currentJobJson);

            var jobIdToResponse = new Dictionary<string, string>();

            foreach (var jobId in allJobIds)
            {
                var responseData = await _distributedCache.GetStringAsync(jobId);

                jobIdToResponse.Add(jobId, responseData);
            }

            if (jobIdToResponse.Values.Any(response => response == null))
            {
                return Ok();
            }

            await _distributedCache.RemoveAsync(CacheKeys.CURRENT_RUNNING_JOBS);

            await Task.WhenAll(allJobIds.Select(id => _distributedCache.RemoveAsync(id)));

            // all jobs are completed
            // resume the bus
            // send response to rest api
            var jobResponsePayload = new JobResponsePayload
            {
                JobId = currentJobId,
                Data = jobIdToResponse.Select(kvp => new JobResponseData
                {
                    Id = kvp.Key,
                    Response = kvp.Value
                }).ToList()
            };

            await _jobResponseService.Submit(token, jobResponsePayload);

            return Ok();
        }
    }
}