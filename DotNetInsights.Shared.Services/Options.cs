using System;
using DotNetInsights.Shared.Contracts;

namespace DotNetInsights.Shared.Services
{
    public class Options<TOptions> : IOptions<TOptions> where TOptions : class
    {
        private readonly Action<TOptions> _setupAction;
        private readonly TOptions _options;

        public Options(Action<TOptions> setupAction)
        {
            _options = Activator.CreateInstance<TOptions>();
            _setupAction = setupAction;
        }

        public TOptions Setup()
        {
            _setupAction(_options);
            return _options;
        }
    }
}