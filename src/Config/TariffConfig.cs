using System;
using TeslaChargeMate.Interfaces;

namespace TeslaChargeMate.Config
{
    public class TariffConfig : IConfigSection
    {
        [EnvironmentVariableName("GEOFENCE_ID")]
        public virtual int GeofenceId { get; set; }

        [EnvironmentVariableName("DAY_RATE")]
        public virtual float DayRate { get; set; }

        [EnvironmentVariableName("NIGHT_RATE")]
        public virtual float NightRate { get; set; }

        [EnvironmentVariableName("DAY_START")]
        public virtual TimeSpan DayStart { get; set; }

        [EnvironmentVariableName("NIGHT_START")]
        public virtual TimeSpan NightStart { get; set; }
    }
}