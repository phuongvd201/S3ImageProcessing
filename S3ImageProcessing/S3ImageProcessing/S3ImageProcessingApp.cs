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
        private readonly IImageHistogramService _imageHistogramService;

        public S3ImageProcessingApp(
            ILogger<S3ImageProcessingApp> logger,
            IImageStorageProvider imageStorageProvider,
            IParsedImageStore parsedImageStore,
            IImageHistogramService imageHistogramService)
        {
            _logger = logger;
            _imageStorageProvider = imageStorageProvider;
            _parsedImageStore = parsedImageStore;
            _imageHistogramService = imageHistogramService;
        }

        public async Task Start()
        {
            try
            {
                _logger.LogInformation("Started app...");

                _logger.LogInformation("Start delete existing data.");
                _parsedImageStore.DeleteExistingData();
                _logger.LogInformation("Finish delete existing data.");

                _logger.LogInformation("Start scan S3 images.");
                var s3Images = await _imageStorageProvider.GetJpgImageFilesAsync();
                _logger.LogInformation("Finish scan S3 images.");

                _logger.LogInformation($"S3 bucket has {s3Images.Length} jpg images.");

                foreach (var s3Image in s3Images)
                {
                    _logger.LogInformation($"Start insert {s3Image.FileName} to table ImageFile.");
                    _parsedImageStore.SaveImageFile(s3Image);
                    _logger.LogInformation($"Start insert {s3Image.FileName} to table ImageFile.");

                    _logger.LogInformation($"Start download {s3Image.FileName}.");
                    var imageData = await _imageStorageProvider.GetImageFileDataAsync(s3Image.FileName);
                    _logger.LogInformation($"Finish download {s3Image.FileName}.");

                    _logger.LogInformation($"Start histogram {s3Image.FileName}.");
                    var histograms = _imageHistogramService.HistogramImage(imageData);
                    _logger.LogInformation($"Finish histogram {s3Image.FileName}.");

                    _logger.LogInformation($"Start save histogram {s3Image.FileName} to database.");
                    _parsedImageStore.SaveImageHistograms(s3Image.FileId, histograms);
                    _logger.LogInformation($"Finish save histogram {s3Image.FileName} to database.");
                }

                _logger.LogInformation($"Finish process s3 images.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                Console.ReadKey();
            }
        }
    }
}