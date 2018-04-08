namespace Phase.Domains
{
    public class AggregateDescriptor
    {
        public AggregateRoot AggregateRoot { get; set; }

        public int Version { get; set; }
    }
}