using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ParsingLib.Contract;
using ParsingLib.Engines;
using ParsingLib.Managers;
using System.Collections.Generic;
using System.Linq;

namespace ParsingLib
{
    /// <summary>
    /// The purpose of this extension is to 'bootstrap' the application from the driver application.
    /// </summary>
    public static class BootstrapExtensions
    {
        /// <summary>
        /// This will bootstrap the configuration and services graph for dependency injection to use this application, including logging
        /// </summary>
        /// <param name="explicitConfig"></param>
        /// <returns>IServiceCollection</returns>
        public static IHostBuilder BoostrapApplication(this string[] args, IDictionary<string, string> explicitConfig = null) =>
            Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                {
                    services.BoostrapServices(explicitConfig);
                })
                .ConfigureLogging((_, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddSimpleConsole(opt => opt.IncludeScopes = true);
                });
        
        /// <summary>
        /// This will bootstrap the configuration and services graph for dependency injection to use this application.
        /// </summary>
        /// <param name="explicitConfig"></param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection BoostrapServices(this IServiceCollection services, IDictionary<string, string> explicitConfig = null)
        {
            //Ensure we can read windows encoded files
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            if (!explicitConfig?.Keys?.Any() ?? true)
                explicitConfig = new Dictionary<string, string>();

            if (services == null)
                services = new ServiceCollection();

            services.AddSingleton<IConfiguration>(explicitConfig.BootstrapConfiguration())
                    .AddScoped<IRawTextLineParsingEngine, RawTextLineParsingEngine>()
                    .AddScoped<IExcelParsingEngine, ExcelParsingEngine>()
                    .AddScoped<IPdfFileTextParsingEngine, PdfFileTextParsingEngine>()
                    .AddScoped<ITextParseManager, TextParseManager>();

            return services;
        }

        #region Private Extensions

        /// <summary>
        /// Bootstrap the configuration from environment variables and explicit key value pairs
        /// </summary>
        /// <param name="explicitConfig"></param>
        /// <returns>IConfiguration instance</returns>
        private static IConfiguration BootstrapConfiguration(this IDictionary<string, string> explicitConfig)
            => new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddInMemoryCollection(explicitConfig)
                .Build();

        #endregion
    }
}
