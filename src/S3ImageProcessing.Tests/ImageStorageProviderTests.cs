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

        [Fact]
        public void Get_JpgImages_ReturnImageCount()
        {
            var images = _imageStorageProvider.GetJpgImageFilesAsync().GetAwaiter().GetResult();

            var actualImagesCount = images.Length;

            Assert.Equal(35, actualImagesCount);
        }

        [Theory]
        [InlineData("image_0001.jpg", 20354)]
        [InlineData("image_0019.jpg", 8470)]
        public void Get_JpgImage_ReturnFileSize(string filename, int expected)
        {
            var images = _imageStorageProvider.GetJpgImageFilesAsync().GetAwaiter().GetResult();

            var actualImageFileSize = images.FirstOrDefault(x => x.FileName == filename)?.FileSize;

            Assert.Equal(expected, actualImageFileSize);
        }
    }
}