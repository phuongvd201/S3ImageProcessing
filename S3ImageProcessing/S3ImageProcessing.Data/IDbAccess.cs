using System.Data.Common;

namespace S3ImageProcessing.Data
{
    public interface IDbAccess
    {
        int Insert(string sql, params object[] parms);

        int Update(string sql, params object[] parms);

        int Delete(string sql, params object[] parms);

        DbConnection CreateAndOpenConnection();

        DbCommand CreateCommand(string sql, DbConnection conn, params object[] parms);
    }
}