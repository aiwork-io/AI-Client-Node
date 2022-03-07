using Newtonsoft.Json;

namespace Application.Common.Models
{
    public class JobResponseData
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("response")]
        public string Response { get; set; }
    }
}