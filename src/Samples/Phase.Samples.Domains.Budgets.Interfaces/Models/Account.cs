using System;

namespace Phase.Samples.Domains.Budgets.Interfaces.Models
{
    public class Account
    {
        public Account(Guid id, string number, string name)
        {
            Id = id;
            Number = number;
            Name = name;
        }

        public Guid Id { get; }

        public string Name { get; }

        public string Number { get; }
    }
}