using System;
using TeslaChargeMate.Interfaces;

namespace TeslaChargeMate.Config
{
    public class TimerConfig : IConfigSection
    {
        [EnvironmentVariableName("DAY_START")]
        public TimeSpan DayStart { get; set; }

        [EnvironmentVariableName("NIGHT_START")]
        public TimeSpan NightStart { get; set; }
    }
}