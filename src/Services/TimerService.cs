using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TeslaChargeMate.Config;
using TeslaChargeMate.Interfaces;

namespace TeslaChargeMate.Services
{
    public class TimerService : BackgroundService
    {
        private readonly TimerConfig _config;
        private readonly ILogger<TimerService> _logger;
        private readonly ITariffService _tariffService;        

        private Timer _timer;

        public TimerService(IConfigProvider configProvider, ILogger<TimerService> logger, ITariffService tariffService)
        {
            _config = configProvider.Get<TimerConfig>();
            _logger = logger;
            _tariffService = tariffService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _tariffService.UpdateRate();
            _timer = new Timer(UpdateRate, null, GetNextTime(_config.DayStart, _config.NightStart), new TimeSpan(0, 0, 0, 0, -1));            
        }

        private void UpdateRate(object state)
        {
            _tariffService.UpdateRate();
            _timer.Change(GetNextTime(_config.DayStart, _config.NightStart), new TimeSpan(0, 0, 0, 0, -1));
        }

        private TimeSpan GetNextTime(TimeSpan dayStart, TimeSpan nightStart)
        {
            var dayStartTime = DateTime.Today.Add(dayStart);
            var nightStartTime = DateTime.Today.Add(nightStart);

            if (dayStartTime < DateTime.Now)
            {
                dayStartTime = dayStartTime.AddDays(1);
            }

            if (nightStartTime < DateTime.Now)
            {
                nightStartTime = nightStartTime.AddDays(1);
            }

            var rateChangeDate = new[] { dayStartTime, nightStartTime }.Min();
            _logger.LogInformation($"Next rate change at: {rateChangeDate}");
            return rateChangeDate - DateTime.Now;
        }
    }
}