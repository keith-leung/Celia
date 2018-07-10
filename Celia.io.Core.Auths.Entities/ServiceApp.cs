using Newtonsoft.Json;
using System;

namespace Celia.io.Core.Auths.Entities
{
    public class ServiceApp
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string ServiceName { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public ServiceStatus Status { get; set; }
    }
}
