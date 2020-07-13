namespace TeslaChargeMate.Interfaces
{
    public interface IConfigProvider
    {
        T Get<T>() where T : IConfigSection;
    }
}