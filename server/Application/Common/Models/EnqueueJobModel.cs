using System.Collections.Generic;
using Newtonsoft.Json;

namespace Application.Common.Models
{
    public class EnqueueJobModel
    {
        public EnqueueJobModel(string source, JobContext jobContext, IList<string> next)
        {
            Source = source;
            JobContext = jobContext;
            Next = next;
        }

        [JsonProperty("source")]
        public string Source { get; }

        [JsonProperty("context")]
        public JobContext JobContext { get; }

        [JsonProperty("next")]
        public IList<string> Next { get; }

        [JsonProperty("data")]
        public JobInputData Data { get; } = new JobInputData();

        [JsonProperty("prev")]
        public IList<string> Prev { get; } = new List<string>();

        [JsonProperty("action")]
        public string Action { get; } = "process";
    }

    public class JobInputData
    {

    }
}