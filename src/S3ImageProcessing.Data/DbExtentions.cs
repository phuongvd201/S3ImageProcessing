using System;
using System.Data.Common;

namespace S3ImageProcessing.Data
{
    public static class DbExtentions
    {
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

        public static int AsInt(this object item, int defaultInt = default(int))
        {
            if (item == null)
                return defaultInt;

            if (!int.TryParse(item.ToString(), out int result))
                return defaultInt;

            return result;
        }

        public static string AsString(this object item, string defaultString = default(string))
        {
            if (item == null || item.Equals(DBNull.Value))
                return defaultString;

            return item.ToString().Trim();
        }
    }
}