using System.Threading.Tasks;

namespace S3ImageProcessing.Services
{
    public interface IParsedImageStore
    {
        Task SaveImageFileAsync();
    }
}