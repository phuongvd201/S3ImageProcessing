using S3ImageProcessing.Services.Entities;

namespace S3ImageProcessing.Services.Helpers
{
    public static class PixelConvertHelper
    {
        public static byte To8Bit(this Pixel value)
        {
            return (byte)(((byte)(value.R / 32) << 5) + ((byte)(value.G / 32) << 2) + (byte)(value.B / 64));
        }
    }
}