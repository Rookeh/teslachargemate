using System;
using TeslaChargeMate.Data;

namespace TeslaChargeMate.Helpers
{
    public static class DateTimeHelper
    {
        public static TariffRate GetRate(TimeSpan dayStart, TimeSpan nightStart, TimeSpan now)
        {
            if (dayStart <= nightStart)
            {
                if (now >= dayStart && now < nightStart)
                {
                    return TariffRate.Day;
                }

                return TariffRate.Night;
            }
            else
            {
                if (now >= dayStart || now < nightStart)
                {
                    return TariffRate.Day;
                }

                return TariffRate.Night;
            }
        }

        public static DateTime GetNextRateChange(TimeSpan dayStart, TimeSpan nightStart, TimeSpan now)
        {
            DateTime nextDayStart = DateTime.Today.Add(dayStart);
            DateTime nextNightStart = DateTime.Today.Add(nightStart);

            if (nextDayStart < DateTime.Today.Add(now))
            {
                nextDayStart = nextDayStart.AddDays(1);
            }

            if (nextNightStart < DateTime.Today.Add(now))
            {
                nextNightStart = nextNightStart.AddDays(1);
            }

            if (dayStart <= nightStart)
            {
                if (now >= dayStart && now < nightStart)
                {
                    return nextNightStart;
                }

                return nextDayStart;
            }
            else
            {
                if (now >= dayStart || now < nightStart)
                {
                    return nextNightStart;
                }

                return nextDayStart;
            }
        }
    }
}
