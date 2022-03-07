using System;
using System.Net.Http;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Infrastructure.Services
{
    public class JobService : IJobService
    {
        private readonly HttpClient _httpClient;

        public JobService(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri("http://inference-engine");

            _httpClient = httpClient;
        }

        public async Task EnqueueJobAsync(EnqueueJobModel model)
        {
            await _httpClient.PostAsync("/receive", new StringContent(JsonConvert.SerializeObject(model, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            })));
        }
    }
}