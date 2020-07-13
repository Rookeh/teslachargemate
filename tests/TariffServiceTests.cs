using Microsoft.Extensions.Logging;
using Moq;
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
        private Mock<ILogger<TariffService>> _mockLogger;
        private Mock<ITeslaMateRepository> _mockRepository;
        private Mock<TariffConfig> _mockConfig;

        private TariffService _service;

        public TariffServiceTests()
        {
            _mockConfig = new Mock<TariffConfig>();
            _mockConfigProvider = new Mock<IConfigProvider>();
            _mockLogger = new Mock<ILogger<TariffService>>();
            _mockRepository = new Mock<ITeslaMateRepository>();            

            _mockConfigProvider.Setup(x => x.Get<TariffConfig>())
                .Returns(_mockConfig.Object);

            _service = new TariffService(_mockConfigProvider.Object, _mockLogger.Object, _mockRepository.Object);
        }

        [Fact]
        public void UpdateRate_SetsCorrectRate_BasedOnTimeOfDay()
        {
            // Arrange
            var dayRate = (float)0.14;
            var nightRate = (float)0.05;
            var dayStart = new TimeSpan(04, 30, 00);
            var nightStart = new TimeSpan(00, 30, 00);
            var currentTime = DateTime.Now - DateTime.Today;
            var nightExpected = currentTime >= nightStart && currentTime < dayStart;
            var geoFenceId = 1;

            _mockConfig.Setup(x => x.DayRate).Returns(dayRate);
            _mockConfig.Setup(x => x.NightRate).Returns(nightRate);
            _mockConfig.Setup(x => x.DayStart).Returns(dayStart);
            _mockConfig.Setup(x => x.NightStart).Returns(nightStart);
            _mockConfig.Setup(x => x.GeofenceId).Returns(geoFenceId);

            // Act
            _service.UpdateRate();

            // Assert
            _mockRepository.Verify(x => x.UpdateChargeRate(geoFenceId, nightExpected ? nightRate : dayRate), Times.Once);
        }
    }
}
