using System;

namespace DotNetInsights.Shared.Contracts.Providers
{
    public interface IClockProvider
    {
        DateTimeOffset Now {get;}
        DateTime DateTime {get;}
    }
}
