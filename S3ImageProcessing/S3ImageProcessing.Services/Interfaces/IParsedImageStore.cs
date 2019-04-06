using S3ImageProcessing.Services.Entities;

namespace S3ImageProcessing.Services.Interfaces
{
    public interface IParsedImageStore
    {
        void DeleteExistingData();

        void Save(ImageFile imageFile, int[] histograms);
    }
}