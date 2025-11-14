using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Submission.Services.UploadService;

namespace Submission.Services.StorageService
{
    public class AwsS3StorageService : IStorageService
    {

        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public AwsS3StorageService(IConfiguration configuration)
        {
            var awsConfig = configuration.GetSection("AWS");

            _bucketName = awsConfig["BucketName"] ?? throw new ArgumentNullException("AWS:BucketName");
            var region = RegionEndpoint.GetBySystemName(awsConfig["Region"]);

            _s3Client = new AmazonS3Client(
                awsConfig["AccessKey"],
                awsConfig["SecretKey"],
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

                string fileUrl = $"https://{_bucketName}.s3.amazonaws.com/{putRequest.Key}";
                return fileUrl;
            }
            catch (Exception ex)
            {
                throw new Exception($"S3 upload failed: {ex.Message}");
            }
        }
    }
}
