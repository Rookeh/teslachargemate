using System;
using TeslaChargeMate.Interfaces;

namespace TeslaChargeMate.Config
{
    public class TimerConfig : IConfigSection
    {
        [EnvironmentVariableName("DAY_START")]
        public virtual TimeSpan DayStart { get; set; }

        [EnvironmentVariableName("NIGHT_START")]
        public virtual TimeSpan NightStart { get; set; }
    }
}