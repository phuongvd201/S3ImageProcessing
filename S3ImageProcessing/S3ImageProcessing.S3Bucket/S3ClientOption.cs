namespace S3ImageProcessing.S3Bucket
{
    public class S3ClientOption
    {
        public string Region { get; set; }

        public string Url { get; set; }

        public string User { get; set; }

        public string AccessKeyId { get; set; }

        public string SecretAccessKey { get; set; }

        public string BucketPath { get; set; }
    }
}