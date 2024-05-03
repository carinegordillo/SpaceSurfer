
using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.ArchivingService
{
    public interface ITargetArchivingDestination
    {    
        public Task<Response> UploadFileAsync(ArchivesModel archiveInfo);

    }
}
