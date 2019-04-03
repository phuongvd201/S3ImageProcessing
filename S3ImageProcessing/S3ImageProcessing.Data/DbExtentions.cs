using System;
using System.Data;
using System.Data.Common;

namespace S3ImageProcessing.Data
{
    public static class DbExtentions
    {
        public static DbCommand WithSqlParameter(this DbCommand cmd, string paramName, object paramValue)
        {
            if (string.IsNullOrEmpty(cmd.CommandText) && cmd.CommandType != CommandType.StoredProcedure)
            {
                throw new InvalidOperationException();
            }

            var param = cmd.CreateParameter();

            param.ParameterName = paramName;
            param.Value = paramValue ?? DBNull.Value;
            cmd.Parameters.Add(param);

            return cmd;
        }

        public static void AddParameters(this DbCommand command, object[] parms)
        {
            if (parms != null && parms.Length > 0)
            {
                for (int i = 0; i < parms.Length; i += 2)
                {
                    string name = parms[i].ToString();

                    if (parms[i + 1] is string && (string)parms[i + 1] == "")
                        parms[i + 1] = null;

                    object value = parms[i + 1] ?? DBNull.Value;

                    var dbParameter = command.CreateParameter();
                    dbParameter.ParameterName = name;
                    dbParameter.Value = value;

                    command.Parameters.Add(dbParameter);
                }
            }
        }
    }
}