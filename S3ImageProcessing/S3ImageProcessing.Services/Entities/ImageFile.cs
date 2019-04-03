namespace S3ImageProcessing.Services.Entities
{
    public class ImageFile
    {
        public int FileId { get; set; }

        public string FileName { get; internal set; }

        public long FileSize { get; internal set; }
    }
}