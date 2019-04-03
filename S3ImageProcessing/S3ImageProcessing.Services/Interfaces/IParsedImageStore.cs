using S3ImageProcessing.Services.Entities;

namespace S3ImageProcessing.Services.Interfaces
{
    public interface IParsedImageStore
    {
        void DeleteExistingData();

        void SaveImageFiles(ImageFile[] files);

        void SaveImageHistograms(int fileId, int[] histograms);
    }
}