using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using S3ImageProcessing.Services.Interfaces;

namespace S3ImageProcessing
{
    public class S3ImageProcessingApp
    {
        private int processedCount = 0;

        private readonly ILogger _logger;
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

        public void Start()
        {
            var sw = Stopwatch.StartNew();

            try
            {
                _logger.LogInformation("Started app...");

                _logger.LogInformation("Starting delete existing data...");
                _parsedImageStore.DeleteExistingData();
                _logger.LogInformation("Finishing delete existing data.");

                _logger.LogInformation("Starting scan S3 images...");
                var s3Images = _imageStorageProvider.GetJpgImageFilesAsync().GetAwaiter().GetResult();
                _logger.LogInformation("Finishing scan S3 images.");

                _logger.LogInformation($"S3 bucket has {s3Images.Length} jpg images.");

                Parallel.ForEach(
                    s3Images,
                    s3Image =>
                    {
                        try
                        {
                            _logger.LogInformation($"Starting process {s3Image.FileName}...");

                            _parsedImageStore.SaveImageFile(s3Image);

                            var imageData = _imageStorageProvider.GetImageFileDataAsync(s3Image.FileName).GetAwaiter().GetResult();

                            var histograms = _imageHistogramService.ComputeImageHistograms(imageData);

                            _parsedImageStore.SaveImageHistograms(s3Image.FileId, histograms);

                            _logger.LogInformation($"Finishing process {s3Image.FileName}.");

                            Interlocked.Increment(ref processedCount);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"ERROR on {s3Image.FileName}: " + ex.Message);
                        }
                    });

                _logger.LogInformation($"Finish processed {processedCount} / {s3Images.Length} images in {sw.Elapsed}.");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message);
            }

            Console.ReadKey();
        }
    }
}