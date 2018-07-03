using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Gds.CosmosDB.Models
{
    public interface IDocumentDBCollection<T> where T : class
    {
        DocumentCollection Collection { get; }
        Task<Document> CreateAsync(T item);
        Task DeleteAsync(Guid id);
        Task<T> GetAsync(Guid id);
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetAsync(string predicate);
        Task<Document> UpdateAsync(Guid id, T item);
    }
}