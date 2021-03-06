﻿using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Threading;
using TeslaChargeMate.Config;
using TeslaChargeMate.Data;
using TeslaChargeMate.Helpers;
using TeslaChargeMate.Interfaces;

namespace TeslaChargeMate.Services
{
    public class TariffService : ITariffService
    {
        private TariffConfig _tariffConfig;
        private DatabaseConfig _dbConfig;
        private readonly IDateTimeWrapper _dateTimeWrapper;
        private readonly ILogger<TariffService> _logger;
        private readonly ITeslaMateRepository _repository;

        public TariffService(IConfigProvider configProvider, IDateTimeWrapper dateTimeWrapper, ILogger<TariffService> logger, ITeslaMateRepository repository)
        {
            _dbConfig = configProvider.Get<DatabaseConfig>();
            _tariffConfig = configProvider.Get<TariffConfig>();
            _dateTimeWrapper = dateTimeWrapper;
            _logger = logger;
            _repository = repository;
        }

        public void UpdateRate(int attempts = 1)
        {
            var rate = DateTimeHelper.GetRate(_tariffConfig.DayStart, _tariffConfig.NightStart, _dateTimeWrapper.Now.TimeOfDay);

            _logger.LogInformation($"Updating tariff to {rate}");

            try
            {
                _repository.UpdateChargeRate(_tariffConfig.GeofenceId, rate == TariffRate.Day ? _tariffConfig.DayRate : _tariffConfig.NightRate);
            }
            catch (NpgsqlException ex)
            {
                _logger.LogWarning($"Failed to update tariff: {ex.Message}. Retries remaining: {_dbConfig.DatabaseRetries - attempts}");                
                if (_dbConfig.DatabaseRetries - attempts > 0)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(_dbConfig.DatabaseWait));
                    UpdateRate(attempts + 1);
                }
                else
                {
                    _logger.LogError($"Failed to update tariff after {_dbConfig.DatabaseRetries} attempts. No further attempts will be made.");
                }
            }
        }        
    }
}