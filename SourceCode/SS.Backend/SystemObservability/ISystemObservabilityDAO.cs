using SS.Backend.SharedNamespace;

namespace SS.Backend.SystemObservability
{
    public interface ISystemObservabilityDAO
    {
        Task<Response> InsertViewDuration(string username, string viewName, int durationInSeconds);

        Task<Response> RetrieveTop3ViewDurations(string username, string timeSpan);

        Task<Response> RetrieveLoginsCount(string username, string timeSpan);

        Task<Response> RetrieveRegistrationsCount(string username, string timeSpan);

        Task<Response> RetrieveCompanyReservationsCount(string username, string timeSpan);

        Task<Response> RetrieveCompanySpaceCount(string username, string timeSpan);

        Task<Response> InsertUsedFeature(string username, string featureName);

        Task<Response> RetrieveMostUsedFeatures(string username, string timeSpan);
    }
}
