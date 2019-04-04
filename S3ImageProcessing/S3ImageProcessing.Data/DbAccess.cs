using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using Microsoft.Extensions.Options;

namespace S3ImageProcessing.Data
{
    public class DbAccess
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

        public IEnumerable<T> Read<T>(string sql, Func<IDataReader, T> make, params object[] parms)
        {
            using (var connection = CreateAndOpenConnection())
            {
                using (var command = CreateCommand(sql, connection, parms))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return make(reader);
                        }
                    }
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