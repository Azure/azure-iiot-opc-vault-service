using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Gds.CosmosDB.Models
{
    [Serializable]
    public class ApplicationName
    {
        public string Locale { get; set; }
        public string Text { get; set; }
    }
    [Serializable]
    public class Application
    {
        [JsonProperty(PropertyName = "id")]
        public Guid ApplicationId { get; set; }
        public uint ID { get; set; }
        public string ApplicationUri { get; set; }
        public string ApplicationName { get; set; }
        public int ApplicationType { get; set; }
        public string ProductUri { get; set; }
        public string ServerCapabilities { get; set; }
        public byte[] Certificate { get; set; }
        public byte[] HttpsCertificate { get; set; }
        public Guid? TrustListId { get; set; }
        public Guid? HttpsTrustListId { get; set; }
        public ApplicationName[] ApplicationNames { get; set; }
        public string[] DiscoveryUrls { get; set; }
    }
}
