using Newtonsoft.Json;
using Opc.Ua;
using System;

namespace Microsoft.Azure.IoTSolutions.GdsVault.CosmosDB.Models
{
    [Serializable]
    public class CertificateRequest
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
}
