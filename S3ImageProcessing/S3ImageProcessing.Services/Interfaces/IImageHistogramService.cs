namespace S3ImageProcessing.Services.Interfaces
{
    public interface IImageHistogramService
    {
        int[] ComputeHistogramImage(byte[] imageData);
    }
}