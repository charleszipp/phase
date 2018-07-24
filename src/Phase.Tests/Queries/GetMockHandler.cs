using Phase.Interfaces;
using Phase.Tests.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Tests.Queries
{
    public class GetMockHandler : IHandleQuery<GetMock, GetMockResult>
    {
        private readonly MockReadModel _db;

        public GetMockHandler(MockReadModel db)
        {
            this._db = db;
        }

        public Task<GetMockResult> Execute(GetMock query, CancellationToken cancellationToken)
        {
            return Task.FromResult(new GetMockResult(_db.Name));
        }
    }
}