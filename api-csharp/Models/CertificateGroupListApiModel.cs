// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Api.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class CertificateGroupListApiModel
    {
        /// <summary>
        /// Initializes a new instance of the CertificateGroupListApiModel
        /// class.
        /// </summary>
        public CertificateGroupListApiModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the CertificateGroupListApiModel
        /// class.
        /// </summary>
        public CertificateGroupListApiModel(IList<string> groups = default(IList<string>))
        {
            Groups = groups;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Groups")]
        public IList<string> Groups { get; set; }

    }
}
