using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StocksApp.Serializers.Json;
using StocksApp.Serializers.Ñsv;
using StocksApp.StocksApiClients;
using StocksApp.StocksApiClients.Tiingo;
using StocksApp.StocksApiClients.Tiingo.Configs;
using StocksApp.StocksApiClients.YahooFinance;
using StocksApp.StocksApiClients.YahooFinance.Configs;
using StocksApp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StocksApp
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.AddJsonFile("appsettings.local.json", optional: false);
                })
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(context.Configuration, services);
                })
                .Build();

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(host.Services.GetRequiredService<MainForm>());
        }

        /// <summary>
        /// Configure dependencies
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="services"></param>
        private static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            services.AddSingleton<MainForm>();
            services.Configure<TiingoConfiguration>(configuration.GetSection("StockApiServicesSettings:Tiingo"));
            services.Configure<YahooFinanceConfiguration>(configuration.GetSection("StockApiServicesSettings:YahooFinance"));
            services.AddSingleton<ITiingoApiClient, TiingoApiClient>();
            services.AddSingleton<IYahooFinanceApiClient, YahooFinanceApiClient>();
            services.AddSingleton<ICsvSerializer, CsvSerializer>();
            services.AddSingleton<IJsonSerializer, JsonSerializer>();
            services.AddSingleton<IDateTimeUtility, DateTimeUtility>();
            services.AddSingleton<IHtmlGenerator, HtmlGenerator>();
            services.AddSingleton<IHtmlToPdfConverter, HtmlToPdfConverter>();
        }
    }
}
