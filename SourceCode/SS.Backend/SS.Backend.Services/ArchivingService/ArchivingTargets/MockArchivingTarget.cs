using System;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using SS.Backend.SharedNamespace;  
using System.Text.Json;


namespace SS.Backend.Services.ArchivingService
{public class MockTargetArchivingDestination : ITargetArchivingDestination
{
    public List<ArchivesModel> UploadedFiles { get; private set; } = new List<ArchivesModel>();
    public bool ThrowException { get; set; } = false;

    public Task<Response> UploadFileAsync(ArchivesModel archiveInfo)
    {
        if (ThrowException)
        {
            throw new Exception("Simulated exception");
        }

        UploadedFiles.Add(archiveInfo);
        return Task.FromResult(new Response { HasError = false });
    }
}


}