using System.Data.Common;

using Microsoft.Extensions.Options;

namespace S3ImageProcessing.Data
{
    public class DbAccess : IDbAccess
    {
        private readonly DbProviderFactory _dbFactory;

        private string ConnectionString => CreateConnectionString();

        private DatabaseOption _databaseOption { get; }

        public DbAccess(IOptions<DatabaseOption> option)
        {
            _databaseOption = option.Value;
            _dbFactory = DbProviderFactories.GetFactory(_databaseOption.ProviderName);
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

        private string CreateConnectionString()
        {
            var builder = _dbFactory.CreateConnectionStringBuilder();

            builder.Add("Server", _databaseOption.ServerName);
            builder.Add("Database", _databaseOption.DatabaseName);
            builder.Add("UserID", _databaseOption.UserID);
            builder.Add("Password", _databaseOption.Password);

            return builder.ConnectionString;
        }
    }
}