using System.Data.Common;
using System.Linq;

using Microsoft.Extensions.Options;

using MySql.Data.MySqlClient;

using S3ImageProcessing.Data;
using S3ImageProcessing.Services.Entities;
using S3ImageProcessing.Services.Implementations;
using S3ImageProcessing.Services.Interfaces;

using Xunit;

namespace S3ImageProcessing.Tests
{
    public class ParsedImageStoreTests
    {
        private readonly IParsedImageStore _parsedImageStore;

        public ParsedImageStoreTests()
        {
            DbProviderFactories.RegisterFactory("MySql.Data.MySqlClient", MySqlClientFactory.Instance);

            _parsedImageStore = new ParsedImageStore(
                new DbAccess(
                    Options.Create(
                        new DatabaseOption()
                        {
                            ProviderName = "MySql.Data.MySqlClient",
                            ConnectionString = "Server=rds-dev-technical-assignment.csrlmopsktab.ap-southeast-2.rds.amazonaws.com;Database=dbPhuong;Uid=phuong;Pwd=d@nan9nic3;",
                        }))
            );
        }

        [Theory]
        [InlineData("image_0001.jpg", 20354)]
        [InlineData("image_0019.jpg", 8470)]
        public void Save_Image_File(string filename, int fileSize)
        {
            _parsedImageStore.DeleteExistingData();

            var imageFile = new ImageFile
            {
                FileName = filename,
                FileSize = fileSize
            };

            _parsedImageStore.SaveImageFile(imageFile);

            var images = _parsedImageStore.GetImageFiles();

            Assert.Equal(filename, images.FirstOrDefault(x => x.FileId == imageFile.FileId).FileName);
            Assert.Equal(fileSize, images.FirstOrDefault(x => x.FileId == imageFile.FileId).FileSize);
        }
    }
}