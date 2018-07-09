// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Api.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class ServerOnNetworkApiModel
    {
        /// <summary>
        /// Initializes a new instance of the ServerOnNetworkApiModel class.
        /// </summary>
        public ServerOnNetworkApiModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ServerOnNetworkApiModel class.
        /// </summary>
        public ServerOnNetworkApiModel(int? recordId = default(int?), string serverName = default(string), string discoveryUrl = default(string), string serverCapabilities = default(string))
        {
            RecordId = recordId;
            ServerName = serverName;
            DiscoveryUrl = discoveryUrl;
            ServerCapabilities = serverCapabilities;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "RecordId")]
        public int? RecordId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "ServerName")]
        public string ServerName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "DiscoveryUrl")]
        public string DiscoveryUrl { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "ServerCapabilities")]
        public string ServerCapabilities { get; set; }

    }
}
