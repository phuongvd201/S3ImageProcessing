using System.Data.Common;

using Microsoft.Extensions.Options;

namespace S3ImageProcessing.Data
{
    public class DbAccess : IDbAccess
    {
        private readonly DbProviderFactory _dbFactory;

        private string ConnectionString { get; }

        private DatabaseOption DatabaseOption { get; }

        public DbAccess(IOptions<DatabaseOption> option)
        {
            DatabaseOption = option.Value;
            ConnectionString = DatabaseOption.ConnectionString;
            _dbFactory = DbProviderFactories.GetFactory(DatabaseOption.ProviderName);
        }

        public int Insert(string sql, params object[] parms)
        {
            using (var connection = CreateAndOpenConnection())
            {
                using (var command = CreateCommand(sql + ";SELECT LAST_INSERT_ID();", connection, parms))
                {
                    return int.Parse(command.ExecuteScalar().ToString());
                }
            }
        }

        public int Update(string sql, params object[] parms)
        {
            using (var connection = CreateAndOpenConnection())
            {
                using (var command = CreateCommand(sql, connection, parms))
                {
                    return command.ExecuteNonQuery();
                }
            }
        }

        public int Delete(string sql, params object[] parms)
        {
            return Update(sql, parms);
        }

        public DbConnection CreateAndOpenConnection()
        {
            var connection = _dbFactory.CreateConnection();

            connection.ConnectionString = ConnectionString;
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