using System;

namespace TeslaChargeMate.Interfaces
{
    public interface IDateTimeWrapper
    {
        DateTime Now { get; }
        DateTime Today { get; }
    }
}