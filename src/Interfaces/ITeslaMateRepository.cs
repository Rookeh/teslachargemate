namespace TeslaChargeMate.Interfaces
{
    public interface ITeslaMateRepository
    {
        void UpdateChargeRate(int geoFenceId, float newRate);
    }
}