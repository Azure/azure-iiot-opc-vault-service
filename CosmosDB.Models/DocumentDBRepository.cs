namespace Microsoft.Azure.IIoT.OpcUa.Services.Gds.CosmosDB.Models
{
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using System;
    using System.Security;
    using System.Threading.Tasks;

    public class DocumentDBRepository : IDocumentDBRepository
    {
        public DocumentClient Client { get; }
        public string DatabaseId { get { return "GDS"; } }

        public DocumentDBRepository(string endpoint, string authKeyOrResourceToken)
        {
            this.Client = new DocumentClient(new Uri(endpoint), authKeyOrResourceToken);
            CreateDatabaseIfNotExistsAsync().Wait();
        }

        public DocumentDBRepository(string endpoint, SecureString authKeyOrResourceToken)
        {
            this.Client = new DocumentClient(new Uri(endpoint), authKeyOrResourceToken);
            CreateDatabaseIfNotExistsAsync().Wait();
        }

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await Client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await Client.CreateDatabaseAsync(new Database { Id = DatabaseId });
                }
                else
                {
                    throw;
                }
            }
        }
    }
}