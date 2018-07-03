namespace Microsoft.Azure.IIoT.OpcUa.Services.Gds.CosmosDB.Models
{
    using Microsoft.Azure.Documents.Client;

    public interface IDocumentDBRepository
    {
        DocumentClient Client { get; }
        string DatabaseId { get; }
    }
}