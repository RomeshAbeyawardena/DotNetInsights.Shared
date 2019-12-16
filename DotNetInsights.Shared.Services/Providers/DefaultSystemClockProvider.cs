using Microsoft.Extensions.Internal;
using DotNetInsights.Shared.Contracts.Providers;
using System;

namespace DotNetInsights.Shared.Services.Providers
{
    public class DefaultSystemClockProvider : IClockProvider
    {
        private readonly ISystemClock systemClock;

        public DefaultSystemClockProvider(ISystemClock systemClock)
        {
            this.systemClock = systemClock;
        }

        public DateTimeOffset Now => systemClock.UtcNow;

        public DateTime DateTime => Now.DateTime;
    }
}
