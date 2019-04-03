using System;

namespace S3ImageProcessing.Services.Helpers
{
    public static class PixelConvertHelper
    {
        public static byte To8BitColorByte(byte red, byte green, byte blue)
        {
            var value8Bit = (byte)(((byte)(red / 32) << 5) + ((byte)(green / 32) << 2) + (byte)(blue / 64));

            if (value8Bit > 255)
            {
                Console.WriteLine(value8Bit);
                return 0;
            }

            return value8Bit;
        }
    }
}