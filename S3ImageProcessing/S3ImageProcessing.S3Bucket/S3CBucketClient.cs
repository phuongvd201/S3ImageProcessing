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

        private readonly AmazonS3Config _s3Config;

        private readonly AmazonS3Client _s3client;

        private readonly S3ClientOption _s3Option;

        public S3CBucketClient(IOptions<S3ClientOption> option)
        {
            _s3Option = option.Value;
            _s3Config = new AmazonS3Config
            {
                RegionEndpoint = !string.IsNullOrWhiteSpace(_s3Option.Region)
                    ? RegionEndpoint.GetBySystemName(_s3Option.Region)
                    : DefaultEndPoint,
            };
            _s3client = new AmazonS3Client(_s3Option.AccessKeyId, _s3Option.SecretAccessKey, _s3Config);
        }

        public async Task<List<S3Object>> ListAllObjectsAsync()
        {
            var result = new List<S3Object>();

            var request = new ListObjectsV2Request
            {
                BucketName = _s3Option.BucketName,
                MaxKeys = int.MaxValue,
                Delimiter = "/" // only listing in root
            };

            ListObjectsV2Response response;

            do
            {
                response = await _s3client.ListObjectsV2Async(request);

                if (response != null)
                {
                    result.AddRange(response.S3Objects);
                }

                request.ContinuationToken = response.NextContinuationToken;
            }
            while (!string.IsNullOrWhiteSpace(response.NextContinuationToken));

            return result;
        }

        public async Task<GetObjectResponse> GetObjectsAsync(string keyName)
        {
            var res = await _s3client.GetObjectAsync(_s3Option.BucketName, keyName);

            return res;
        }

        public void Dispose()
        {
            _s3client?.Dispose();
        }
    }
}