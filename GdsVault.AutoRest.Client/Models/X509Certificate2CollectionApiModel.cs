// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace GdsVault.Client.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class X509Certificate2CollectionApiModel
    {
        /// <summary>
        /// Initializes a new instance of the
        /// X509Certificate2CollectionApiModel class.
        /// </summary>
        public X509Certificate2CollectionApiModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// X509Certificate2CollectionApiModel class.
        /// </summary>
        public X509Certificate2CollectionApiModel(IList<X509Certificate2ApiModel> chain = default(IList<X509Certificate2ApiModel>))
        {
            Chain = chain;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Chain")]
        public IList<X509Certificate2ApiModel> Chain { get; set; }

    }
}
