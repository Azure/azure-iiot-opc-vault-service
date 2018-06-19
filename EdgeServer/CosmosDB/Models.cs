using Newtonsoft.Json;
using System;

namespace Opc.Ua.Gds.Server.Database.CosmosDB.Models
{
    [Serializable]
    class ApplicationName
    {
        public string Locale { get; set; }
        public string Text { get; set; }
    }
    [Serializable]
    class Application
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
    [Serializable]
    class CertificateRequest
    {
        [JsonProperty(PropertyName = "id")]
        public Guid RequestId { get; set; }
        public Guid ApplicationId { get; set; }
        public int State { get; set; }
        public NodeId CertificateGroupId { get; set; }
        public NodeId CertificateTypeId { get; set; }
        public byte[] CertificateSigningRequest { get; set; }
        public string SubjectName { get; set; }
        public string[] DomainNames { get; set; }
        public string PrivateKeyFormat { get; set; }
        public string PrivateKeyPassword { get; set; }
        public string AuthorityId { get; set; }
        public byte[] Certificate { get; set; }
        public DateTime RequestTime { get; set; }
        public DateTime ApproveRejectTime { get; set; }
        public DateTime AcceptTime { get; set; }
    }

    [Serializable]
    class CertificateStore
    {
        [JsonProperty(PropertyName = "id")]
        public Guid TrustListId { get; private set; }
        public string Path { get; set; }
        public string AuthorityId { get; set; }
    }

}
