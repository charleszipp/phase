using Phase.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Phase.Publishers
{
    //This should only be used in the context of unit testing so excluding from code coverage
    [ExcludeFromCodeCoverage]
    internal class IdemptotentTestingEventPublisher : EventPublisher
    {
        private readonly EventPublisher _publisher;

        internal IdemptotentTestingEventPublisher(EventPublisher publisher, DependencyResolver resolver)
            :base(resolver)
        {
            _publisher = publisher;
        }

        internal override void Publish(IEvent @event, CancellationToken cancellationToken)
        {
            _publisher.Publish(@event, cancellationToken);
            _publisher.Publish(@event, cancellationToken);
        }
    }
}