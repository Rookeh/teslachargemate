using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;
using System;
using TeslaChargeMate.Config;
using TeslaChargeMate.Interfaces;
using TeslaChargeMate.Services;
using Xunit;

namespace TeslaChargeMate.Tests
{
    public class TariffServiceTests
    {
        private Mock<IConfigProvider> _mockConfigProvider;
        private Mock<IDateTimeWrapper> _mockDateTimeWrapper;
        private Mock<ILogger<TariffService>> _mockLogger;
        private Mock<ITeslaMateRepository> _mockRepository;
        private Mock<TariffConfig> _mockTariffConfig;
        private Mock<DatabaseConfig> _mockDbConfig;

        private TariffService _service;

        public TariffServiceTests()
        {
            _mockDateTimeWrapper = new Mock<IDateTimeWrapper>();
            _mockDbConfig = new Mock<DatabaseConfig>();
            _mockTariffConfig = new Mock<TariffConfig>();
            _mockConfigProvider = new Mock<IConfigProvider>();
            _mockLogger = new Mock<ILogger<TariffService>>();
            _mockRepository = new Mock<ITeslaMateRepository>();

            _mockConfigProvider.Setup(x => x.Get<DatabaseConfig>())
                .Returns(_mockDbConfig.Object);

            _mockConfigProvider.Setup(x => x.Get<TariffConfig>())
                .Returns(_mockTariffConfig.Object);

            _service = new TariffService(_mockConfigProvider.Object, _mockDateTimeWrapper.Object,
                _mockLogger.Object, _mockRepository.Object);
        }

        [Theory]
        [InlineData(18, 00, false)]
        [InlineData(00, 00, false)]
        [InlineData(00, 30, true)]
        [InlineData(03, 00, true)]
        [InlineData(04, 30, false)]
        [InlineData(08, 00, false)]
        public void UpdateRate_SetsCorrectRate_BasedOnTimeOfDay(int hour, int minute, bool nightExpected)
        {
            // Arrange
            var dayRate = (float)0.14;
            var nightRate = (float)0.05;
            var dayStart = new TimeSpan(04, 30, 00);
            var nightStart = new TimeSpan(00, 30, 00);
            var currentTime = DateTime.Now - DateTime.Today;
            var geoFenceId = 1;

            _mockDateTimeWrapper.Setup(x => x.Now)
                .Returns(new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, hour, minute, 0))
                .Verifiable();

            _mockDateTimeWrapper.Setup(x => x.Today)
                .Returns(new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day))
                .Verifiable();

            _mockTariffConfig.Setup(x => x.DayRate).Returns(dayRate);
            _mockTariffConfig.Setup(x => x.NightRate).Returns(nightRate);
            _mockTariffConfig.Setup(x => x.DayStart).Returns(dayStart);
            _mockTariffConfig.Setup(x => x.NightStart).Returns(nightStart);
            _mockTariffConfig.Setup(x => x.GeofenceId).Returns(geoFenceId);

            // Act
            _service.UpdateRate();

            // Assert
            _mockRepository.Verify(x => x.UpdateChargeRate(geoFenceId, nightExpected ? nightRate : dayRate), Times.Once);
        }

        [Fact]
        public void UpdateRate_IfDatabaseThrowsException_RetriesUntilSuccess()
        {
            // Arrange
            var dayRate = (float)0.14;
            var nightRate = (float)0.05;
            var dayStart = new TimeSpan(04, 30, 00);
            var nightStart = new TimeSpan(00, 30, 00);
            var currentTime = DateTime.Now - DateTime.Today;
            var nightExpected = currentTime >= nightStart && currentTime < dayStart;
            var dbWait = 1;
            var dbRetries = 2;
            var geoFenceId = 1;

            _mockDateTimeWrapper.Setup(x => x.Now).Returns(DateTime.Now);
            _mockDateTimeWrapper.Setup(x => x.Today).Returns(DateTime.Today);
            _mockDbConfig.Setup(x => x.DatabaseWait).Returns(dbWait);
            _mockDbConfig.Setup(x => x.DatabaseRetries).Returns(dbRetries);
            _mockTariffConfig.Setup(x => x.DayRate).Returns(dayRate);
            _mockTariffConfig.Setup(x => x.NightRate).Returns(nightRate);
            _mockTariffConfig.Setup(x => x.DayStart).Returns(dayStart);
            _mockTariffConfig.Setup(x => x.NightStart).Returns(nightStart);
            _mockTariffConfig.Setup(x => x.GeofenceId).Returns(geoFenceId);
            _mockRepository.SetupSequence(x => x.UpdateChargeRate(It.IsAny<int>(), It.IsAny<float>()))
                .Throws(new NpgsqlException())
                .Pass();

            // Act
            _service.UpdateRate();

            // Assert
            _mockRepository.Verify(x => x.UpdateChargeRate(It.IsAny<int>(), It.IsAny<float>()), Times.Exactly(2));
        }

        [Fact]
        public void UpdateRate_IfDatabaseThrowsException_DoesNotExceedMaxRetries()
        {
            // Arrange
            var dayRate = (float)0.14;
            var nightRate = (float)0.05;
            var dayStart = new TimeSpan(04, 30, 00);
            var nightStart = new TimeSpan(00, 30, 00);
            var currentTime = DateTime.Now - DateTime.Today;
            var nightExpected = currentTime >= nightStart && currentTime < dayStart;
            var dbWait = 1;
            var dbRetries = 5;
            var geoFenceId = 1;

            _mockDateTimeWrapper.Setup(x => x.Now).Returns(DateTime.Now);
            _mockDateTimeWrapper.Setup(x => x.Today).Returns(DateTime.Today);
            _mockDbConfig.Setup(x => x.DatabaseWait).Returns(dbWait);
            _mockDbConfig.Setup(x => x.DatabaseRetries).Returns(dbRetries);
            _mockTariffConfig.Setup(x => x.DayRate).Returns(dayRate);
            _mockTariffConfig.Setup(x => x.NightRate).Returns(nightRate);
            _mockTariffConfig.Setup(x => x.DayStart).Returns(dayStart);
            _mockTariffConfig.Setup(x => x.NightStart).Returns(nightStart);
            _mockTariffConfig.Setup(x => x.GeofenceId).Returns(geoFenceId);
            _mockRepository.SetupSequence(x => x.UpdateChargeRate(It.IsAny<int>(), It.IsAny<float>()))
                .Throws(new NpgsqlException())
                .Throws(new NpgsqlException())
                .Throws(new NpgsqlException())
                .Throws(new NpgsqlException())
                .Throws(new NpgsqlException())
                .Throws(new NpgsqlException());

            // Act
            _service.UpdateRate();

            // Assert
            _mockRepository.Verify(x => x.UpdateChargeRate(It.IsAny<int>(), It.IsAny<float>()), Times.Exactly(5));
        }
    }
}
