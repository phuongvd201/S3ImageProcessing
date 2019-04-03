using System;

using S3ImageProcessing.Services.Entities;

namespace S3ImageProcessing.Services.Helpers
{
    public static class PixelConvertHelper
    {
        public static byte To8Bit(this Pixel value)
        {
            var value8Bit = (byte)(((byte)(value.R / 32) << 5) + ((byte)(value.G / 32) << 2) + (byte)(value.B / 64));

            if (value8Bit > 255)
            {
                Console.WriteLine(value8Bit);
                return 0;
            }

            return value8Bit;
        }
    }
}