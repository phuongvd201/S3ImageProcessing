﻿using System;

using S3ImageProcessing.Data;
using S3ImageProcessing.Services.Entities;
using S3ImageProcessing.Services.Interfaces;

namespace S3ImageProcessing.Services.Implementations
{
    public class ParsedImageStore : IParsedImageStore
    {
        private readonly IDbAccess _dbAccess;

        public ParsedImageStore(IDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public void SaveImageFile(ImageFile file)
        {
            string sql = @"INSERT INTO ImageFile (FileName, FileSize) VALUES (@FileName, @FileSize)";

            file.FileId = _dbAccess.Insert(
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
                            using (var command = _dbAccess.CreateCommand(
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
                    catch (Exception)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch
                        {
                            throw;
                        }

                        throw;
                    }
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

            _dbAccess.Delete(sql);
        }

        private void DeleteHistograms()
        {
            var sql = @"DELETE From Histogram";

            _dbAccess.Delete(sql);
        }
    }
}