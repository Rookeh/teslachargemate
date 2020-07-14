namespace TeslaChargeMate.Interfaces
{
    public interface ITariffService
    {
        void UpdateRate(int attempts = 1);
    }
}