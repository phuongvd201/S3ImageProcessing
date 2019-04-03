using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using S3ImageProcessing.Services.Entities;
using S3ImageProcessing.Services.Helpers;
using S3ImageProcessing.Services.Interfaces;

using SixLabors.ImageSharp;
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
                _parsedImageStore.DeleteExistingData();

                var s3Images = await _imageStorageProvider.GetJpgImageFilesAsync();

                _parsedImageStore.SaveImageFiles(s3Images);

                foreach (var s3Image in s3Images)
                {
                    var imageData = _imageStorageProvider.GetImageFileDataAsync(s3Image.FileName).GetAwaiter().GetResult();

                    var bitmap = Image.Load<Rgb24>(imageData);

                    var histograms = new byte[256];

                    for (int i = 0; i < bitmap.Height; i++)
                    {
                        for (int j = 0; j < bitmap.Width; j++)
                        {
                            var pixel = new Pixel
                            {
                                R = bitmap[i, j].R,
                                G = bitmap[i, j].G,
                                B = bitmap[i, j].B,
                            };

                            histograms[pixel.To8Bit()]++;
                        }
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