using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using S3ImageProcessing.Services.Entities;
using S3ImageProcessing.Services.Helpers;
using S3ImageProcessing.Services.Interfaces;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

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
                _logger.LogInformation("Started app...");

                _logger.LogInformation("Start delete existing data.");

                _parsedImageStore.DeleteExistingData();

                _logger.LogInformation("Finish delete existing data.");

                var s3Images = await _imageStorageProvider.GetJpgImageFilesAsync();

                _parsedImageStore.SaveImageFiles(s3Images);

                foreach (var s3Image in s3Images)
                {
                    var imageData = await _imageStorageProvider.GetImageFileDataAsync(s3Image.FileName);

                    var bitmap = Image.Load<Rgb24>(imageData);

                    var histograms = new int[256];

                    //MemoryMarshal.AsBytes(bitmap.GetPixelSpan()).ToArray();

                    var pixelArray = bitmap.GetPixelSpan()
                        .ToArray()
                        .Select(
                            x => new Pixel
                            {
                                R = x.R,
                                G = x.G,
                                B = x.B,
                            }.To8Bit())
                        .ToArray();

                    for (int i = 0; i < pixelArray.Count(); i++)
                    {
                        histograms[pixelArray[i]]++;
                    }

                    _parsedImageStore.SaveImageHistograms(s3Image.FileId, histograms);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}