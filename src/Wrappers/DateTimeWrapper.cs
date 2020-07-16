using System;
using TeslaChargeMate.Interfaces;

namespace TeslaChargeMate.Wrappers
{
    public class DateTimeWrapper : IDateTimeWrapper
    {
        public DateTime Now => DateTime.Now;

        public DateTime Today => DateTime.Today;
    }
}