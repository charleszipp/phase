using Phase.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Phase.Publishers
{
    //This should only be used in the context of unit testing so excluding from code coverage
    [ExcludeFromCodeCoverage]
    internal class IdemptotentTestingEventPublisher : EventPublisher
    {
        internal IdemptotentTestingEventPublisher(DependencyResolver resolver)
            :base(resolver)
        {
        }

        internal override void Publish(IEvent @event, CancellationToken cancellationToken)
        {
            base.Publish(@event, cancellationToken);
            base.Publish(@event, cancellationToken);
        }
    }
}