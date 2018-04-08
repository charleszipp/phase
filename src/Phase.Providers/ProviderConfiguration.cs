using System;
using System.Collections.Generic;

namespace Phase.Providers
{
    public class ProviderConfiguration
    {
        public ProviderConfiguration(string serviceTypeName, string executingDirectory, string providerName, Func<string, KeyValuePair<string, object>> partitionKeyFactory, Func<string, IDictionary<string, string>> serviceInstanceKeysFactory)
        {
            ServiceTypeName = serviceTypeName;
            ExecutingDirectory = executingDirectory;
            ProviderName = providerName;
            PartitionKeyFactory = partitionKeyFactory;
            ServiceInstanceKeysFactory = (serviceInstanceName) =>
            {
                var rvalue = serviceInstanceKeysFactory(serviceInstanceName);
                var partitionKey = partitionKeyFactory(serviceInstanceName);
                rvalue[partitionKey.Key] = partitionKey.Value?.ToString();
                return rvalue;
            };            
        }

        public string ServiceTypeName { get; }

        public string ExecutingDirectory { get; }

        public string ProviderName { get; }

        public Func<string, KeyValuePair<string, object>> PartitionKeyFactory { get; }

        public Func<string, IDictionary<string, string>> ServiceInstanceKeysFactory { get; }
    }
}