// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace GdsVault.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// A X509 certificate revocation list.
    /// </summary>
    public partial class X509CrlApiModel
    {
        /// <summary>
        /// Initializes a new instance of the X509CrlApiModel class.
        /// </summary>
        public X509CrlApiModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the X509CrlApiModel class.
        /// </summary>
        /// <param name="issuer">The Issuer name of the revocation
        /// list.</param>
        /// <param name="crl">The base64 encoded X509 certificate revocation
        /// list.</param>
        public X509CrlApiModel(string issuer = default(string), string crl = default(string))
        {
            Issuer = issuer;
            Crl = crl;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the Issuer name of the revocation list.
        /// </summary>
        [JsonProperty(PropertyName = "Issuer")]
        public string Issuer { get; set; }

        /// <summary>
        /// Gets or sets the base64 encoded X509 certificate revocation list.
        /// </summary>
        [JsonProperty(PropertyName = "Crl")]
        public string Crl { get; set; }

    }
}
