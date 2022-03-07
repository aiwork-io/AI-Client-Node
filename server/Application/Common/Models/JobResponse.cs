using Newtonsoft.Json;

namespace Application.Common.Models
{
    public class JobResponse
    {
        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("context")]
        public JobContext JobContext { get; set; }

        [JsonProperty("data")]
        public JobData Data { get; set; }
    }
}