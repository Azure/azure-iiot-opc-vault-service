using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.IoTSolutions.GdsVault.CosmosDB.Models
{
    [Serializable]
    public class CertificateStore
    {
        [JsonProperty(PropertyName = "id")]
        public Guid TrustListId { get; private set; }
        public string Path { get; set; }
        public string AuthorityId { get; set; }
    }

}
