using System;

namespace Phase.Domains
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PhaseAggregateAttribute : Attribute
    {
        public PhaseAggregateAttribute(string aggregateName)
        {
            AggregateName = aggregateName;
        }

        public string AggregateName { get; }
    }
}