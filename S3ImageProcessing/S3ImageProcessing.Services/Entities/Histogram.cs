namespace S3ImageProcessing.Services.Entities
{
    public class Histogram
    {
        public long FileID { get; set; }

        public byte BandNumber { get; set; }

        public int Value { get; set; }
    }
}