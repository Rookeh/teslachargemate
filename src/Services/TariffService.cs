using Microsoft.Extensions.Logging;
using System;
using TeslaChargeMate.Config;
using TeslaChargeMate.Interfaces;

namespace TeslaChargeMate.Services
{
    public class TariffService : ITariffService
    {
        private TariffConfig _config;
        private readonly ILogger<TariffService> _logger;
        private readonly ITeslaMateRepository _repository;

        public TariffService(IConfigProvider configProvider, ILogger<TariffService> logger, ITeslaMateRepository repository)
        {
            _config = configProvider.Get<TariffConfig>();
            _logger = logger;
            _repository = repository;
        }

        public void UpdateRate()
        {
            var rate = GetRate(_config.DayStart, _config.NightStart);
            _logger.LogInformation($"Updating tariff to {rate}");
            _repository.UpdateChargeRate(_config.GeofenceId, rate == TariffRate.Day ? _config.DayRate : _config.NightRate);
        }

        private enum TariffRate
        {
            Day,
            Night
        }

        private TariffRate GetRate(TimeSpan dayStart, TimeSpan nightStart)
        {
            var dayRateTime = DateTime.Today.Add(dayStart);
            var nightRateTime = DateTime.Today.Add(nightStart);
            var now = DateTime.Now;
            if (now >= nightRateTime && now < dayRateTime)
            {
                return TariffRate.Night;
            }
            else
            {
                return TariffRate.Day;
            }
        }
    }
}
