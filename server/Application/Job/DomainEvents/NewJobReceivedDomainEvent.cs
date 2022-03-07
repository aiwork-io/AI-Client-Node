using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Application.Job.DomainEvents
{
    public class NewJobReceivedDomainEvent
    {
        [JsonProperty("jobId")]
        public string JobId { get; set; }

        [JsonProperty("images")]
        public IList<NewJob> Data { get; set; }
    }

    public class NewJob
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class NewJobReceivedDomainEventHandler
    {
        private readonly IFileLoader _fileLoader;
        private readonly IJobService _jobService;
        private const string DIRECTORY = "/workspace/images";
        private readonly ILogger<NewJobReceivedDomainEventHandler> _logger;
        private readonly IDistributedCache _distributedCache;

        public NewJobReceivedDomainEventHandler(IFileLoader fileLoader,
            IJobService jobService,
            ILogger<NewJobReceivedDomainEventHandler> logger,
            IDistributedCache distributedCache)
        {
            _fileLoader = fileLoader;
            _jobService = jobService;
            _logger = logger;
            _distributedCache = distributedCache;
        }

        private static string GetFileName(string hrefLink)
        {
            var u = new Uri(hrefLink);
            return Path.GetFileName(u.AbsolutePath);
        }

        public async Task Handle(NewJobReceivedDomainEvent message)
        {
            _logger.LogCritical("Processing {@message}", message.Data.Select(datum => datum.Id));

            var token = await _distributedCache.GetStringAsync("token");

            if (token == null)
            {
                throw new Exception("Token not found");
            }

            await _distributedCache.SetStringAsync(CacheKeys.CURRENT_JOB_ID, message.JobId);

            await _distributedCache.SetStringAsync(CacheKeys.CURRENT_RUNNING_JOBS, JsonConvert.SerializeObject(message.Data.Select(datum => datum.Id)));

            var files = new Dictionary<string, string>();

            foreach (var newJob in message.Data)
            {
                var fileNameWithPath = $"{DIRECTORY}/{GetFileName(newJob.Url)}";

                files.Add(newJob.Id, fileNameWithPath);

                await _fileLoader.Download(newJob.Url, fileNameWithPath);
            }

            foreach (var (key, value) in files)
            {
                await _jobService.EnqueueJobAsync(
                    new EnqueueJobModel(
                        $"file:{value}",
                        new JobContext(
                            key,
                            "timecode",
                            null,
                            null
                        ),
                        new List<string> { "post:http://job_services/api/job/completed" }));
            }

            await Task.Delay(10000);
        }
    }
}