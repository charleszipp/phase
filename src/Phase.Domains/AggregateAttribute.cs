using System;

namespace Phase.Domains
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AggregateAttribute : Attribute
    {
        public AggregateAttribute(string aggregateName)
        {
            AggregateName = aggregateName;
        }

        public string AggregateName { get; }
    }
}