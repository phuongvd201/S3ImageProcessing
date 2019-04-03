using System.Data.Common;

namespace S3ImageProcessing.Data
{
    public interface IDbAccess
    {
        int ExecuteScalar(string sql, params object[] parms);

        int ExecuteNonQuery(string sql, params object[] parms);

        DbConnection CreateAndOpenConnection();

        DbCommand CreateCommand(string sql, DbConnection conn, params object[] parms);
    }
}