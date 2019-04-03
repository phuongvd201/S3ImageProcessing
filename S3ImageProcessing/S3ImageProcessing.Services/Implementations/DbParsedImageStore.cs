using System;

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

        private void SaveImageFile(ImageFile file)
        {
            string sql = @"INSERT INTO ImageFile (FileName, FileSize) VALUES (@FileName, @FileSize)";

            file.FileId = _db.Insert(sql, Take(file));
        }

        public void SaveImageFiles(ImageFile[] files)
        {
            foreach (var imageFile in files)
            {
                SaveImageFile(imageFile);
            }
        }

        public void SaveImageHistograms(int fileId, int[] histograms)
        {
            string sql = @"INSERT INTO Histogram (FileID, BandNumber, Value) VALUES (@FileID, @BandNumber, @Value)";

            using (var connection = _db.CreateConnection())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    for (int i = 0; i <= 255; i++)
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

        private void SaveImageHistogram(int fileId, byte bandNumber, int value)
        {
            string sql = @"INSERT INTO Histogram (FileID, BandNumber, Value) VALUES (@FileID, @BandNumber, @Value)";

            _db.Insert(
                sql,
                new object[]
                {
                    "@FileID", fileId,
                    "@BandNumber", bandNumber,
                    "@Value", value,
                });
        }

        public void DeleteExistingData()
        {
            DeleteHistogram();
            TruncateImageFile();
        }

        private void TruncateImageFile()
        {
            // Truncate to reseed PK identity to 1
            var sql = @"TRUNCATE TABLE ImageFile";

            _db.Delete(sql);
        }

        private void DeleteHistogram()
        {
            // Truncate to reseed PK identity to 1
            var sql = @"DELETE From Histogram";

            _db.Delete(sql);
        }

        private static object[] Take(ImageFile file)
        {
            return new object[]
            {
                "@FileName", file.FileName,
                "@FileSize", file.FileSize,
            };
        }
    }
}