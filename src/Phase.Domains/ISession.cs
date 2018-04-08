using Phase.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Phase.Domains
{
    public interface ISession
    {
        Task AddAsync<T>(T actor) where T : AggregateRoot;

        Task<T> GetAsync<T>(Guid id) where T : AggregateRoot;

        IEnumerable<IEvent> Flush();
    }
}