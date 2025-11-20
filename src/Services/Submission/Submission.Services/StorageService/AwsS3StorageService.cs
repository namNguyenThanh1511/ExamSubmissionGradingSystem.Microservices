using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using Submission.Services.DTOs;
using Submission.Services.UploadService;

namespace Submission.Services.StorageService
{
    public class AwsS3StorageService : IStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public AwsS3StorageService(IOptions<AwsConfig> awsOptions)
        {
            var config = awsOptions.Value ?? throw new ArgumentNullException(nameof(awsOptions));

            _bucketName = config.BucketName ?? throw new ArgumentNullException("AWS:BucketName");
            var region = RegionEndpoint.GetBySystemName(config.Region ?? throw new ArgumentNullException("AWS:Region"));

            _s3Client = new AmazonS3Client(
                config.AccessKey,
                config.SecretKey,
                region
            );
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            var key = $"submissions/{Guid.NewGuid()}_{fileName}";

            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
                InputStream = fileStream,
                ContentType = contentType,
                // GIỮ PRIVATE – KHÔNG dùng ACL
                CannedACL = S3CannedACL.Private
            };

            await _s3Client.PutObjectAsync(putRequest);

            // Trả về key để controller generate presigned URL
            return key;
        }
        public string GeneratePresignedUrl(string key, int expireMinutes = 60)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = key,
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                Verb = HttpVerb.GET
            };

            return _s3Client.GetPreSignedURL(request);
        }
    }
}
