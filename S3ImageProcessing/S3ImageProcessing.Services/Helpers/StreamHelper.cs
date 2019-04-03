using System.IO;

namespace S3ImageProcessing.Services.Helpers
{
    public static class StreamHelper
    {
        public static byte[] AsByte(this Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                byte[] buffer = new byte[2048];

                int readBytes;
                while ((readBytes = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, readBytes);
                }

                return ms.ToArray();
            }
        }
    }
}