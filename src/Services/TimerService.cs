using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TeslaChargeMate.Config;
using TeslaChargeMate.Helpers;
using TeslaChargeMate.Interfaces;

namespace TeslaChargeMate.Services
{
    public class TimerService : BackgroundService
    {
        private readonly TimerConfig _config;
        private readonly IDateTimeWrapper _dateTimeWrapper;
        private readonly ILogger<TimerService> _logger;
        private readonly ITariffService _tariffService;        

        private Timer _timer;

        public TimerService(IConfigProvider configProvider, IDateTimeWrapper dateTimeWrapper, ILogger<TimerService> logger, ITariffService tariffService)
        {
            _config = configProvider.Get<TimerConfig>();
            _dateTimeWrapper = dateTimeWrapper;
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
            var rateChangeDate = DateTimeHelper.GetNextRateChange(dayStart, nightStart, _dateTimeWrapper.Now.TimeOfDay);
            _logger.LogInformation($"Next rate change at: {rateChangeDate}");
            return (rateChangeDate - _dateTimeWrapper.Now).Add(new TimeSpan(0, 0, 5));
        }
    }
}