namespace S3ImageProcessing.Services.Helpers
{
    public static class ColorConvertHelper
    {
        public static byte To8BitColorByte(byte red, byte green, byte blue)
        {
            var color8bit = (byte)(((byte)(red / 32) << 5) + ((byte)(green / 32) << 2) + (byte)(blue / 64));

            return color8bit;
        }
    }
}