using System;

namespace Phase.Samples.Domains.Budgets.Models
{
    internal class AccountEntity
    {
        internal AccountEntity(Guid id, string number, string name)
        {
            Id = id;
            Number = number;
            Name = name;
        }

        internal Guid Id { get; }

        internal string Name { get; }

        internal string Number { get; }
    }
}