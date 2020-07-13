using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TeslaChargeMate.Config;
using TeslaChargeMate.Data;
using TeslaChargeMate.Interfaces;
using TeslaChargeMate.Services;

namespace TeslaChargeMate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddEnvironmentVariables(prefix: "TCM_");
                })                
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(hostContext.Configuration);
                    services.AddSingleton<IConfigProvider, ConfigProvider>();
                    services.AddSingleton<ITeslaMateRepository, TeslaMateRepository>();
                    services.AddSingleton<ITariffService, TariffService>();
                    services.AddHostedService<TimerService>();
                });
    }
}
