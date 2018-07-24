using System;

namespace Phase.Tests.Models
{
    public class AccountEntity
    {
        public AccountEntity(Guid id, string number, string name)
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