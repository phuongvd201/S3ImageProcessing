using S3ImageProcessing.Services.Helpers;
using S3ImageProcessing.Services.Interfaces;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

namespace S3ImageProcessing.Services.Implementations
{
    public class ImageHistogramService : IImageHistogramService
    {
        public int[] ComputeHistogramImage(byte[] imageData)
        {
            using (var bitmap = Image.Load<Rgb24>(imageData))
            {
                int[] histograms = new int[256];

                var pixels = bitmap.GetPixelSpan().ToArray();

                foreach (Rgb24 pixel in pixels)
                {
                    var color8Bit = ColorConvertHelper.To8BitColorByte(pixel.R, pixel.G, pixel.B);

                    histograms[color8Bit]++;
                }

                return histograms;
            }
        }
    }
}