using MongoDB.Bson;
using Phase.Interfaces;
using Phase.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Phase.Domains
{
    public class AggregateRoot : MarshalByRefObject, IVolatileState
    {      
    }
}