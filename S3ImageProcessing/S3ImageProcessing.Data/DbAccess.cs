using System.Data.Common;

using Microsoft.Extensions.Options;

namespace S3ImageProcessing.Data
{
    public class DbAccess : IDbAccess
    {
        private readonly DbProviderFactory _dbFactory;

        private readonly DatabaseOption _databaseOption;

        public DbAccess(IOptions<DatabaseOption> option)
        {
            _databaseOption = option.Value;
            _dbFactory = DbProviderFactories.GetFactory(_databaseOption.ProviderName);
        }

        public int ExecuteScalar(string sql, params object[] parms)
        {
            using (var connection = CreateAndOpenConnection())
            {
                using (var command = CreateCommand(sql + ";SELECT LAST_INSERT_ID();", connection, parms))
                {
                    return int.Parse(command.ExecuteScalar().ToString());
                }
            }
        }

        public int ExecuteNonQuery(string sql, params object[] parms)
        {
            using (var connection = CreateAndOpenConnection())
            {
                using (var command = CreateCommand(sql, connection, parms))
                {
                    return command.ExecuteNonQuery();
                }
            }
        }

        public DbConnection CreateAndOpenConnection()
        {
            var connection = _dbFactory.CreateConnection();

            connection.ConnectionString = _databaseOption.ConnectionString;
            connection.Open();

            return connection;
        }

        public DbCommand CreateCommand(string sql, DbConnection conn, params object[] parms)
        {
            var command = _dbFactory.CreateCommand();

            command.Connection = conn;
            command.CommandText = sql;

            command.AddParameters(parms);

            return command;
        }
    }
}