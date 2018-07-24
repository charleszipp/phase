using Phase.Interfaces;
using System;

namespace Phase.Tests.Commands
{
    public class CreateMock : ICommand
    {
        public CreateMock(string mockName)
        {
            MockName = mockName;
        }

        public string MockName { get; }
    }
}