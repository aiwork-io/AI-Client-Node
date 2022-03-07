using System.Collections.Generic;
using Newtonsoft.Json;

namespace Application.Common.Models
{
    public class JobResponsePayload
    {
        [JsonProperty("jobId")]
        public string JobId { get; set; }

        [JsonProperty("data")]
        public IList<JobResponseData> Data { get; set; }
    }
}