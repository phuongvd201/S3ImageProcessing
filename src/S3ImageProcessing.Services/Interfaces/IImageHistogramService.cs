namespace S3ImageProcessing.Services.Interfaces
{
    public interface IImageHistogramService
    {
        int[] ComputeImageHistograms(byte[] imageData);
    }
}