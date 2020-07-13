using Dapper;
using Npgsql;
using System.Data;
using TeslaChargeMate.Config;
using TeslaChargeMate.Interfaces;

namespace TeslaChargeMate.Data
{
    public class TeslaMateRepository : ITeslaMateRepository
    {
        private readonly DatabaseConfig _config;

        public TeslaMateRepository(IConfigProvider configProvider)
        {
            _config = configProvider.Get<DatabaseConfig>();
        }

        internal IDbConnection Connection
        {
            get
            {
                return new NpgsqlConnection($"User ID={_config.DatabaseUser};Password={_config.DatabasePassword};Host={_config.DatabaseHost};Port={_config.DatabasePort};Database={_config.DatabaseName};Pooling=true;");
            }
        }

        public void UpdateChargeRate(int geoFenceId, float newRate)
        {
            using (IDbConnection dbConnection = Connection)
            {
                const string sql = @"UPDATE geofences
                                     SET cost_per_kwh = @newRate
                                     WHERE id = @geoFenceId";

                dbConnection.Open();
                dbConnection.Execute(sql, new { newRate, geoFenceId });
            }
        }
    }
}