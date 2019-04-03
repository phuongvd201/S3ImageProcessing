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
        private readonly IParsedImageStore _parsedImageStore;

        public S3ImageProcessingApp(
            ILogger<S3ImageProcessingApp> logger,
            IImageStorageProvider imageStorageProvider,
            IParsedImageStore parsedImageStore)
        {
            _logger = logger;
            _imageStorageProvider = imageStorageProvider;
            _parsedImageStore = parsedImageStore;
        }

        public async Task Start()
        {
            try
            {
                _parsedImageStore.DeleteExistingData();

                var s3Images = await _imageStorageProvider.GetJpgImageFilesAsync();

                _parsedImageStore.SaveImageFiles(s3Images);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}