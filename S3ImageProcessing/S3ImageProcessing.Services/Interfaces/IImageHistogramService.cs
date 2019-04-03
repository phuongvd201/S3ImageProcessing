namespace S3ImageProcessing.Services.Interfaces
{
    public interface IImageHistogramService
    {
        int[] HistogramImage(byte[] imageData);
    }
}