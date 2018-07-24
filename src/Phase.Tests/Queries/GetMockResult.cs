using System;

namespace Phase.Tests.Queries
{
    public class GetMockResult
    {
        public GetMockResult(string mockName) => MockName = mockName;

        public string MockName { get; }
    }
}