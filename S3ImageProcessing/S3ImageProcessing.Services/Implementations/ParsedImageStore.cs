using System;
using System.Data;
using System.Linq;

using S3ImageProcessing.Data;
using S3ImageProcessing.Services.Entities;
using S3ImageProcessing.Services.Interfaces;

namespace S3ImageProcessing.Services.Implementations
{
    public class ParsedImageStore : IParsedImageStore
    {
        private readonly DbAccess _dbAccess;

        public ParsedImageStore(DbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public void SaveImageFile(ImageFile file)
        {
            string sql = @"INSERT INTO ImageFile (FileName, FileSize) VALUES (@FileName, @FileSize)";

            file.FileId = _dbAccess.ExecuteScalar(
                sql,
                new object[]
                {
                    "@FileName", file.FileName,
                    "@FileSize", file.FileSize,
                });
        }

        public void SaveImageHistograms(int fileId, int[] histograms)
        {
            string sql = @"INSERT INTO Histogram (FileID, BandNumber, Value) VALUES (@FileID, @BandNumber, @Value)";

            using (var connection = _dbAccess.CreateAndOpenConnection())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        for (int i = 0; i < histograms.Length; i++)
                        {
                            var parms = new object[]
                            {
                                "@FileID", fileId,
                                "@BandNumber", i,
                                "@Value", histograms[i],
                            };

                            using (var command = _dbAccess.CreateCommand(sql, connection, parms))
                            {
                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public ImageFile[] GetImageFiles()
        {
            string sql = @"SELECT FileID, FileName, FileSize FROM ImageFile";

            return _dbAccess.Read(sql, Make).ToArray();
        }

        public void DeleteExistingData()
        {
            DeleteHistograms();
            DeleteImageFiles();
        }

        private void DeleteImageFiles()
        {
            // delete all data and reseed PK auto increment to 1
            var sql = @"DELETE FROM ImageFile; ALTER TABLE ImageFile AUTO_INCREMENT = 1;";

            _dbAccess.ExecuteNonQuery(sql);
        }

        private void DeleteHistograms()
        {
            var sql = @"DELETE From Histogram";

            _dbAccess.ExecuteNonQuery(sql);
        }

        private static readonly Func<IDataReader, ImageFile> Make = reader =>
            new ImageFile
            {
                FileId = reader["FileID"].AsInt(),
                FileName = reader["FileName"].AsString(),
                FileSize = reader["FileSize"].AsInt()
            };
    }
}