using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using S3ImageProcessing.Services.Interfaces;

namespace S3ImageProcessing
{
    public class S3ImageProcessingApp
    {
        private readonly ILogger<S3ImageProcessingApp> _logger;

        private readonly IImageStorageProvider _imageStorageProvider;

        public S3ImageProcessingApp(
            ILogger<S3ImageProcessingApp> logger,
            IImageStorageProvider imageStorageProvider)
        {
            _logger = logger;
            _imageStorageProvider = imageStorageProvider;
        }

        public async Task Start()
        {
            try
            {
                var s3Images = await _imageStorageProvider.GetImageFilesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}