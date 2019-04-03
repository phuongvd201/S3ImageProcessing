using System;
using System.Linq;
using System.Threading.Tasks;

using S3ImageProcessing.S3Bucket;
using S3ImageProcessing.Services.Entities;
using S3ImageProcessing.Services.Interfaces;

namespace S3ImageProcessing.Services.Implementations
{
    public class S3ImageStorageProvider : IImageStorageProvider
    {
        private readonly S3CBucketClient _s3Client;

        public S3ImageStorageProvider(S3CBucketClient s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task<ImageFile[]> GetImageFilesAsync()
        {
            var s3Objects = await _s3Client.ListAllObjectsAsync();

            return s3Objects
                .Where(x => x.Key.ToUpper().EndsWith(".jpg", StringComparison.Ordinal))
                .Select(
                    x => new ImageFile
                    {
                        FileName = x.Key,
                        FileSize = x.Size,
                    })
                .ToArray();
        }
    }
}