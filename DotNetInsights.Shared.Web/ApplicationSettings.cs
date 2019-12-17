using Microsoft.Extensions.Configuration;

namespace DotNetInsights.Shared.WebApp
{
    public class ApplicationSettings
    {
        public ApplicationSettings(IConfiguration configuration)
        {
            configuration.Bind(this);
        }
        public string ConnectionString { get; set; }
    }
}