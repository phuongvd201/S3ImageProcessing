using System;
using System.Data.Common;

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

        public void Save(ImageFile imageFile, int[] histograms)
        {
            using (var connection = _dbAccess.CreateAndOpenConnection())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        InsertImageFile(imageFile, connection, transaction);

                        InsertHistograms(imageFile.FileId, histograms, connection, transaction);

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

        private void ExecuteTransaction(params Action<DbCommand>[] executeCommands)
        {
            using (var connection = _dbAccess.CreateAndOpenConnection())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;

                        foreach (var executeCommand in executeCommands)
                        {
                            executeCommand(command);
                        }

                        transaction.Commit();
                    }
                }
            }
        }

        private void InsertHistograms(int fileId, int[] histograms, DbConnection connection, DbTransaction transaction)
        {
            string sql = @"INSERT INTO Histogram (FileID, BandNumber, Value) VALUES (@FileID, @BandNumber, @Value);";

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
                    command.Transaction = transaction;
                    command.ExecuteNonQuery();
                }
            }
        }

        private void InsertImageFile(ImageFile imageFile, DbConnection connection, DbTransaction transaction)
        {
            string sql = @"INSERT INTO ImageFile (FileName, FileSize) VALUES (@FileName, @FileSize); SELECT @@IDENTITY;";

            using (var command = _dbAccess.CreateCommand(
                sql,
                connection,
                new object[]
                {
                    "@FileName", imageFile.FileName,
                    "@FileSize", imageFile.FileSize,
                }))
            {
                command.Transaction = transaction;
                imageFile.FileId = command.ExecuteScalar().AsInt();
            }
        }

        public void DeleteExistingData()
        {
            DeleteHistograms();
            DeleteImageFiles();
        }

        private void DeleteImageFiles()
        {
            // delete all data and reseed PK auto increment to 1
            var sql = @"DELETE FROM ImageFile; DBCC CHECKIDENT (ImageFile, RESEED, 0);";

            _dbAccess.ExecuteNonQuery(sql);
        }

        private void DeleteHistograms()
        {
            var sql = @"DELETE From Histogram";

            _dbAccess.ExecuteNonQuery(sql);
        }
    }
}