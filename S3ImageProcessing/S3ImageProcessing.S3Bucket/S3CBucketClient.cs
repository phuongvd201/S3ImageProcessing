using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

using Microsoft.Extensions.Options;

namespace S3ImageProcessing.S3Bucket
{
    public class S3CBucketClient : IDisposable
    {
        private static readonly RegionEndpoint DefaultEndPoint = RegionEndpoint.APNortheast1;

        public AmazonS3Config S3Config { get; }

        public AmazonS3Client Client { get; }

        public S3ClientOption Option { get; }

        public S3CBucketClient(IOptions<S3ClientOption> option)
        {
            Option = option.Value;
            S3Config = new AmazonS3Config
            {
                RegionEndpoint = !string.IsNullOrEmpty(Option.Region)
                    ? RegionEndpoint.GetBySystemName(Option.Region)
                    : DefaultEndPoint,
            };
            Client = new AmazonS3Client(Option.AccessKeyId, Option.SecretAccessKey, S3Config);
        }

        public async Task<List<S3Object>> ListAllObjectsAsync()
        {
            var result = new List<S3Object>();

            var request = new ListObjectsV2Request
            {
                BucketName = Option.BucketName,
                MaxKeys = int.MaxValue,
                Delimiter = "/" // listing on root
            };

            ListObjectsV2Response response;

            do
            {
                try
                {
                    response = Client.ListObjectsV2Async(request).Result;

                    if (response != null)
                    {
                        result.AddRange(response.S3Objects);
                    }

                    request.ContinuationToken = response.NextContinuationToken;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }
            }
            while (!string.IsNullOrEmpty(response.NextContinuationToken));

            return result;
        }

        public async Task<GetObjectResponse> GetObjectsAsync(string bucketName, string keyName)
        {
            var res = await Client.GetObjectAsync(bucketName, keyName);
            return res;
        }

        public void Dispose()
        {
            Client?.Dispose();
        }
    }
}