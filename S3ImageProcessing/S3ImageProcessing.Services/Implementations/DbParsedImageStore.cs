using S3ImageProcessing.Data;
using S3ImageProcessing.Services.Entities;
using S3ImageProcessing.Services.Interfaces;

namespace S3ImageProcessing.Services.Implementations
{
    public class DbParsedImageStore : IParsedImageStore
    {
        private readonly Db _db;

        public DbParsedImageStore(Db db)
        {
            _db = db;
        }

        public void SaveImageFile(ImageFile file)
        {
            string sql = @"INSERT INTO ImageFile (FileName, FileSize) VALUES (@FileName, @FileSize)";

            file.FileId = _db.Insert(
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

            using (var connection = _db.CreateConnection())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    for (int i = 0; i <= histograms.Length - 1; i++)
                    {
                        using (var command = _db.CreateCommand(
                            sql,
                            connection,
                            new object[]
                            {
                                "@FileID", fileId,
                                "@BandNumber", i,
                                "@Value", histograms[i],
                            })
                        )
                        {
                            command.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
            }
        }

        public void DeleteExistingData()
        {
            DeleteHistograms();
            DeleteImageFiles();
        }

        private void DeleteImageFiles()
        {
            // Truncate to reseed PK identity to 1
            var sql = @"TRUNCATE TABLE ImageFile";

            _db.Delete(sql);
        }

        private void DeleteHistograms()
        {
            var sql = @"DELETE From Histogram";

            _db.Delete(sql);
        }
    }
}