using System;

namespace Phase.Domains
{
    public delegate object AggregateFactory(Type messageType);
}