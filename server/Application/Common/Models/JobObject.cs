using Newtonsoft.Json;

namespace Application.Common.Models
{
    public class JobObject
    {
        [JsonProperty("bbox")]
        public double[] BBox { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }
    }
}