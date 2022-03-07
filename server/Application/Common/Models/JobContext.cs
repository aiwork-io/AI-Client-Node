using System.Collections.Generic;
using Newtonsoft.Json;

namespace Application.Common.Models
{
    public class JobContext
    {
        public JobContext(string projectId, string timeCode, string interestRegion, IList<string> objectFilter)
        {
            ProjectId = projectId;
            TimeCode = timeCode;
            InterestRegion = interestRegion;
            ObjectFilter = objectFilter;
        }

        [JsonProperty("project_id")]
        public string ProjectId { get; }

        [JsonProperty("timecode")]
        public string TimeCode { get; }

        [JsonProperty("debug")] public bool Debug { get; } = true;

        [JsonProperty("interest_region")]
        public string InterestRegion { get; }

        [JsonProperty("object_filter")]
        public IList<string> ObjectFilter { get; }
    }
}