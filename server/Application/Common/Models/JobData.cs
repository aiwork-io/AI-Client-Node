using System.Collections.Generic;
using Newtonsoft.Json;

namespace Application.Common.Models
{
    public class JobData
    {
        [JsonProperty("object")]
        public IList<JobObject> Object { get; set; }
    }
}