using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotNetInsights.Shared.Library.Extensions
{
    public static class LoggingBuilderExtensions 
    {
        public static ILoggingBuilder AddProvider<T>(this ILoggingBuilder builder)
        where T: class, ILoggerProvider
        {
            builder.Services.AddTransient<ILoggerProvider, T>();
            return builder;
        }
    }
}