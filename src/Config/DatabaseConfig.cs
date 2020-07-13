using TeslaChargeMate.Interfaces;

namespace TeslaChargeMate.Config
{
    public class DatabaseConfig : IConfigSection
    {
        [EnvironmentVariableName("DATABASE_NAME")]
        public string DatabaseName { get; set; }

        [EnvironmentVariableName("DATABASE_HOST")]
        public string DatabaseHost { get; set; }

        [EnvironmentVariableName("DATABASE_PORT")]
        public int DatabasePort { get; set; }

        [EnvironmentVariableName("DATABASE_USER")]
        public string DatabaseUser { get; set; }

        [EnvironmentVariableName("DATABASE_PASSWORD")]
        public string DatabasePassword { get; set; }
    }
}