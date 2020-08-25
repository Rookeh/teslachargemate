using System;
using TeslaChargeMate.Data;
using TeslaChargeMate.Helpers;
using Xunit;

namespace TeslaChargeMate.Tests
{
    public class DateTimeHelperTests
    {
        [Theory]
        [InlineData("04:00:00", "18:00:00", "13:00:00", "18:00:00")]
        [InlineData("04:00:00", "18:00:00", "18:30:00", "04:00:00")]
        [InlineData("04:30:00", "00:30:00", "13:00:00", "00:30:00")]
        [InlineData("04:30:00", "00:30:00", "01:30:00", "04:30:00")]
        public void GetNextRateChange_GivenStartTimes_AndCurrentTime_ReturnsNextRateChange(string dayStartString, string nightStartString, string nowString, string expectedChangeString)
        {
            // Arrange
            var dayStart = TimeSpan.Parse(dayStartString);
            var nightStart = TimeSpan.Parse(nightStartString);
            var now = TimeSpan.Parse(nowString);
            var expectedChange = DateTime.Today.Add(TimeSpan.Parse(expectedChangeString));
            if (expectedChange < DateTime.Today.Add(now))
            {
                expectedChange = expectedChange.AddDays(1);
            }

            // Act
            var nextChange = DateTimeHelper.GetNextRateChange(dayStart, nightStart, now);

            // Assert
            Assert.Equal(expectedChange, nextChange);
        }

        [Theory]
        [InlineData("04:00:00", "18:00:00", "13:00:00", TariffRate.Day)]
        [InlineData("04:00:00", "18:00:00", "18:30:00", TariffRate.Night)]
        [InlineData("04:30:00", "00:30:00", "13:00:00", TariffRate.Day)]
        [InlineData("04:30:00", "00:30:00", "01:30:00", TariffRate.Night)]
        public void GetRate_GivenStartTimes_AndCurrentTime_ReturnsRate(string dayStartString, string nightStartString, string nowString, TariffRate expectedRate)
        {
            // Arrange
            var dayStart = TimeSpan.Parse(dayStartString);
            var nightStart = TimeSpan.Parse(nightStartString);
            var now = TimeSpan.Parse(nowString);

            // Act
            var rate = DateTimeHelper.GetRate(dayStart, nightStart, now);

            // Assert
            Assert.Equal(expectedRate, rate);
        }
    }
}
