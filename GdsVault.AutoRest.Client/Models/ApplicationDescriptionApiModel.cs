// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Azure.IIoT.OpcUa.Services.Gds.Client.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class ApplicationDescriptionApiModel
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationDescriptionApiModel
        /// class.
        /// </summary>
        public ApplicationDescriptionApiModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ApplicationDescriptionApiModel
        /// class.
        /// </summary>
        public ApplicationDescriptionApiModel(string applicationUri = default(string), string productUri = default(string), ApplicationNameApiModel applicationName = default(ApplicationNameApiModel), int? applicationType = default(int?), string gatewayServerUri = default(string), IList<string> discoveryProfileUri = default(IList<string>), IList<string> discoveryUrls = default(IList<string>))
        {
            ApplicationUri = applicationUri;
            ProductUri = productUri;
            ApplicationName = applicationName;
            ApplicationType = applicationType;
            GatewayServerUri = gatewayServerUri;
            DiscoveryProfileUri = discoveryProfileUri;
            DiscoveryUrls = discoveryUrls;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "ApplicationUri")]
        public string ApplicationUri { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "ProductUri")]
        public string ProductUri { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "ApplicationName")]
        public ApplicationNameApiModel ApplicationName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "ApplicationType")]
        public int? ApplicationType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "GatewayServerUri")]
        public string GatewayServerUri { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "DiscoveryProfileUri")]
        public IList<string> DiscoveryProfileUri { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "DiscoveryUrls")]
        public IList<string> DiscoveryUrls { get; set; }

    }
}
