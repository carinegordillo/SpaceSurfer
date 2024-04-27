using System;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using SS.Backend.SharedNamespace;  
namespace SS.Backend.Services.ArchivingService
{
    public class S3ArchivingDestination : ITargetArchivingDestination
    {
               private string accessKey = "AKIAZI2LD4MHH4SYIMWE";
        private string secretKey = "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";

        public async Task<Response> UploadFileAsync(ArchivesModel archiveInfo)
        {
            Response result = new Response();
            string bucketName = archiveInfo.destination;
            string filePath = archiveInfo.filePath;
            string keyName = archiveInfo.fileName;

            // Properly initialize the AWS credentials and client
            var awsCredentials = new BasicAWSCredentials(accessKey, secretKey);
            var region = Amazon.RegionEndpoint.USWest1;  // Ensure this matches the region of your S3 bucket
            using (var s3Client = new AmazonS3Client(awsCredentials, region))
            {
                try
                {
                    var transferUtility = new TransferUtility(s3Client);
                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        BucketName = bucketName,
                        FilePath = filePath,
                        Key = keyName
                    };

                    Console.WriteLine("Uploading file to S3 bucket " + bucketName);
                    await transferUtility.UploadAsync(uploadRequest);
                    Console.WriteLine("File uploaded successfully to bucket " + bucketName);
                }
                catch (AmazonS3Exception e)
                {
                    Console.WriteLine($"AWS Error encountered on server. Message: '{e.Message}'");
                    Console.WriteLine($"AWS Error Code: {e.ErrorCode}, Error Type: {e.ErrorType}, Request ID: {e.RequestId}");
                    result.HasError = true;
                    result.ErrorMessage = e.Message;
                    return result;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unknown error encountered. Message: '{e.Message}'");
                    result.HasError = true;
                    result.ErrorMessage = e.Message;
                    return result;
                }
            }

            return result;
        }
    }
}
