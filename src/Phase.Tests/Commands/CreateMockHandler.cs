using Phase.Domains;
using Phase.Tests.Events;
using Phase.Tests.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Tests.Commands
{
    public class CreateMockHandler : CommandHandler<CreateMock>
    {
        public override async Task Execute(CreateMock command, CancellationToken cancellationToken)
        {
            // perform validation first
            if (string.IsNullOrEmpty(command.MockName))
                throw new ArgumentException("Mock name required");

            await RaiseEventAsync<MockAggregate>(TenantId, new MockCreated(command.MockName), cancellationToken);
        }
    }
}