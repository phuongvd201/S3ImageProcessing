using System.Data.Common;

namespace S3ImageProcessing.Data
{
    public class Db
    {
        private static readonly DbProviderFactory DbFactory = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");

        public string ConnectionString { get; set; }

        public int Insert(string sql, params object[] parms)
        {
            using (var connection = CreateConnection())
            {
                using (var command = CreateCommand(sql + ";SELECT SCOPE_IDENTITY();", connection, parms))
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
    }
}