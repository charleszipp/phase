using Phase.Domains;
using System;
using System.Threading.Tasks;

namespace Phase.Domains
{
    public interface IRepository
    {
        Task<T> Get<T>(string aggregateId) where T : AggregateRoot;
    }
}