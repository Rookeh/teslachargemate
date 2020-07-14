using TeslaChargeMate.Interfaces;

namespace TeslaChargeMate.Config
{
    public class DatabaseConfig : IConfigSection
    {
        [EnvironmentVariableName("DATABASE_NAME")]
        public virtual string DatabaseName { get; set; }

        [EnvironmentVariableName("DATABASE_HOST")]
        public virtual string DatabaseHost { get; set; }

        [EnvironmentVariableName("DATABASE_PORT")]
        public virtual int DatabasePort { get; set; }

        [EnvironmentVariableName("DATABASE_USER")]
        public virtual string DatabaseUser { get; set; }

        [EnvironmentVariableName("DATABASE_PASSWORD")]
        public virtual string DatabasePassword { get; set; }

        [EnvironmentVariableName("DATABASE_WAIT")]
        public virtual int DatabaseWait { get; set; }

        [EnvironmentVariableName("DATABASE_RETRIES")]
        public virtual int DatabaseRetries { get; set; }
    }
}