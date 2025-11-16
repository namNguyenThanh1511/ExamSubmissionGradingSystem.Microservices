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
            try
            {
                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = $"submissions/{Guid.NewGuid()}_{fileName}",
                    InputStream = fileStream,
                    ContentType = contentType,
                    CannedACL = S3CannedACL.Private
                };

                await _s3Client.PutObjectAsync(putRequest);

                return $"https://{_bucketName}.s3.{_s3Client.Config.RegionEndpoint.SystemName}.amazonaws.com/{putRequest.Key}";
            }
            catch (Exception ex)
            {
                throw new Exception($"S3 upload failed: {ex.Message}");
            }
        }
    }
}
