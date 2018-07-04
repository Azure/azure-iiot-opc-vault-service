using System;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Gds.App.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}