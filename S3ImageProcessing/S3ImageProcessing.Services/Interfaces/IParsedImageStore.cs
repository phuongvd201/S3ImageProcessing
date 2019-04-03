using S3ImageProcessing.Services.Entities;

namespace S3ImageProcessing.Services.Interfaces
{
    public interface IParsedImageStore
    {
        void SaveImageFiles(ImageFile[] files);

        void DeleteExistingData();
    }
}