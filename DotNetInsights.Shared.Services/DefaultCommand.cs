using DotNetInsights.Shared.Contracts;
using DotNetInsights.Shared.Contracts.Builders;
using DotNetInsights.Shared.Services.Builders;
using System;
using System.Collections.Generic;

namespace DotNetInsights.Shared.Services
{
    public static class DefaultCommand
    {
        public static ICommand Create<T>(string name, IDictionary<string, object> parameters)
        {
            return new DefaultCommand<T>(name, parameters);
        }

        public static ICommand Create<T>(string name, Action<IDictionaryBuilder<string, object>> commandParameters)
        {
            if(commandParameters == null)
                throw new ArgumentNullException(nameof(commandParameters));

            var parameters = DictionaryBuilder.Create<string, object>();
            commandParameters(parameters);
            return new DefaultCommand<T>(name, parameters.ToDictionary());
        }
    }
    public class DefaultCommand<T> : ICommand
    {
        public DefaultCommand(string name, IDictionary<string, object> parameters)
        {
            Name = name;
            Parameters = parameters;
        }

        public string Name { get; set; }
        public IDictionary<string, object> Parameters { get; }
    }
}
