using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Application.Serializer
{
    public static class JsonSettings
    {
        public static JsonSerializerSettings DefaultSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.None
        };
    }
}