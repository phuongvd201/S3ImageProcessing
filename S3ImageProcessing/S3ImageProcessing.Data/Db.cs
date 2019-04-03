using System.Data.Common;

using Microsoft.Extensions.Options;

using MySql.Data.MySqlClient;

namespace S3ImageProcessing.Data
{
    public class Db
    {
        private static readonly DbProviderFactory DbFactory = MySqlClientFactory.Instance;

        private string ConnectionString => CreateConnectionString();

        public DatabaseOption DatabaseOption { get; set; }

        public Db()
        {
        }

        public Db(IOptions<DatabaseOption> option)
        {
            DatabaseOption = option.Value;
        }

        public int Insert(string sql, params object[] parms)
        {
            using (var connection = CreateConnection())
            {
                using (var command = CreateCommand(sql + ";SELECT LAST_INSERT_ID();;", connection, parms))
                {
                    return int.Parse(command.ExecuteScalar().ToString());
                }
            }
        }

        public int Update(string sql, params object[] parms)
        {
            using (var connection = CreateConnection())
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

        private DbConnection CreateConnection()
        {
            var connection = DbFactory.CreateConnection();

            connection.ConnectionString = ConnectionString;
            connection.Open();

            return connection;
        }

        private static DbCommand CreateCommand(string sql, DbConnection conn, params object[] parms)
        {
            var command = DbFactory.CreateCommand();

            command.Connection = conn;
            command.CommandText = sql;

            command.AddParameters(parms);

            return command;
        }

        private string CreateConnectionString()
        {
            var builder = DbFactory.CreateConnectionStringBuilder();

            builder.Add("Server", DatabaseOption.ServerName);
            builder.Add("Database", DatabaseOption.DatabaseName);
            builder.Add("UserID", DatabaseOption.UserID);
            builder.Add("Password", DatabaseOption.Password);

            return builder.ConnectionString;
        }
    }
}