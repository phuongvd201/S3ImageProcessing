using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using S3ImageProcessing.Services.Interfaces;

namespace S3ImageProcessing
{
    public class S3ImageProcessingApp
    {
        private int processedCount = 0;
        private static readonly object LockObject = new object();

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

        public async Task Start()
        {
            var sw = Stopwatch.StartNew();

            _logger.LogInformation("Started app...");

            _logger.LogInformation("Start delete existing data.");
            _parsedImageStore.DeleteExistingData();
            _logger.LogInformation("Finish delete existing data.");

            _logger.LogInformation("Start scan S3 images.");
            var s3Images = await _imageStorageProvider.GetJpgImageFilesAsync();
            _logger.LogInformation("Finish scan S3 images.");

            _logger.LogInformation($"S3 bucket has {s3Images.Length} jpg images.");

            Parallel.ForEach(
                s3Images,
                s3Image =>
                {
                    try
                    {
                        _logger.LogInformation($"Start process {s3Image.FileName}...");

                        _parsedImageStore.SaveImageFile(s3Image);

                        var imageData = _imageStorageProvider.GetImageFileDataAsync(s3Image.FileName).GetAwaiter().GetResult();

                        var histograms = _imageHistogramService.HistogramImage(imageData);

                        _parsedImageStore.SaveImageHistograms(s3Image.FileId, histograms);

                        _logger.LogInformation($"Finish process {s3Image.FileName}.");

                        lock (LockObject)
                        {
                            processedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"ERROR on {s3Image.FileName}: " + ex.Message);
                    }
                });

            _logger.LogInformation($"Finish processed {processedCount} / {s3Images.Length} images in {sw.Elapsed}.");

            Console.ReadKey();
        }
    }
}