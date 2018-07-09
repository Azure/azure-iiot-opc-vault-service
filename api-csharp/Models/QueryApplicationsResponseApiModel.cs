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

    public partial class QueryApplicationsResponseApiModel
    {
        /// <summary>
        /// Initializes a new instance of the QueryApplicationsResponseApiModel
        /// class.
        /// </summary>
        public QueryApplicationsResponseApiModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the QueryApplicationsResponseApiModel
        /// class.
        /// </summary>
        public QueryApplicationsResponseApiModel(IList<ApplicationDescriptionApiModel> applications = default(IList<ApplicationDescriptionApiModel>), System.DateTime? lastCounterResetTime = default(System.DateTime?), int? nextRecordId = default(int?))
        {
            Applications = applications;
            LastCounterResetTime = lastCounterResetTime;
            NextRecordId = nextRecordId;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Applications")]
        public IList<ApplicationDescriptionApiModel> Applications { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "LastCounterResetTime")]
        public System.DateTime? LastCounterResetTime { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "NextRecordId")]
        public int? NextRecordId { get; set; }

    }
}
