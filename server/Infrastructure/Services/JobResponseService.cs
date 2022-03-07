using System;
using System.Net.Http;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Infrastructure.Services
{
    public class JobResponseService : IJobResponseService
    {
        private readonly ILogger<JobResponseService> _logger;
        private readonly HttpClient _httpClient;

        public JobResponseService(ILogger<JobResponseService> logger, HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri("http://13.229.247.193");

            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task Submit(string token, JobResponsePayload payload)
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var formData = new MultipartFormDataContent
            {
                {new StringContent(payload.JobId), "jobId"},
                {new StringContent(JsonConvert.SerializeObject(payload.Data)), "data"}
            };

            var response = await _httpClient.PostAsync("/wp-json/aiwork/v1/submit", formData);

            await response.Content.ReadAsStringAsync();
        }
    }
}