using System.Threading.Tasks;

using S3ImageProcessing.Services.Entities;

namespace S3ImageProcessing.Services.Interfaces
{
    public interface IImageStorageProvider
    {
        Task<ImageFile[]> GetImageFilesAsync();
    }
}