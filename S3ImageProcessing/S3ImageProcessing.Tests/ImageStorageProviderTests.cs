using System.Linq;

using Microsoft.Extensions.Options;

using S3ImageProcessing.S3Bucket;
using S3ImageProcessing.Services.Implementations;
using S3ImageProcessing.Services.Interfaces;

using Xunit;

namespace S3ImageProcessing.Tests
{
    public class ImageStorageProviderTests
    {
        private readonly IImageStorageProvider _imageStorageProvider;

        public ImageStorageProviderTests()
        {
            _imageStorageProvider = new S3ImageStorageProvider(
                new S3CBucketClient(
                    Options.Create(
                        new S3ClientOption()
                        {
                            AccessKeyId = "AKIAWQMANXGNLZIT73WZ",
                            SecretAccessKey = "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",
                            BucketName = "axon-demo",
                            Region = "ap-southeast-2",
                        })));
        }

        [Theory]
        [InlineData("image_0001.jpg", 20354)]
        [InlineData("image_0019.jpg", 8470)]
        public void Get_Jpg_Images_S3(string filename, int fileSize)
        {
            var images = _imageStorageProvider.GetJpgImageFilesAsync().GetAwaiter().GetResult();

            Assert.Equal(35, images.Length);
            Assert.Equal(fileSize, images.FirstOrDefault(x => x.FileName == filename).FileSize);
        }

        [Theory]
        [InlineData("image_0001.jpg", 20354)]
        [InlineData("image_0019.jpg", 8470)]
        public void Get_Image_Data_S3(string filename, int fileSize)
        {
            var images = _imageStorageProvider.GetImageFileDataAsync(filename).GetAwaiter().GetResult();

            Assert.Equal(fileSize, images.Length);
        }
    }
}