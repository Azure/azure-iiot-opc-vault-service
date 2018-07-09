// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Api.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class SigningRequestApiModel
    {
        /// <summary>
        /// Initializes a new instance of the SigningRequestApiModel class.
        /// </summary>
        public SigningRequestApiModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the SigningRequestApiModel class.
        /// </summary>
        public SigningRequestApiModel(string applicationURI = default(string), string csr = default(string))
        {
            ApplicationURI = applicationURI;
            Csr = csr;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "ApplicationURI")]
        public string ApplicationURI { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Csr")]
        public string Csr { get; set; }

    }
}