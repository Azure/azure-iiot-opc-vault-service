namespace Microsoft.Azure.IoTSolutions.GdsVault.CosmosDB.Models
{
    using Microsoft.Azure.Documents.Client;

    public interface IDocumentDBRepository
    {
        DocumentClient Client { get; }
        string DatabaseId { get; }
    }
}