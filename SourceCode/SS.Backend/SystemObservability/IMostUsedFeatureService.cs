using SS.Backend.SharedNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS.Backend.SystemObservability
{
    public interface IMostUsedFeatureService
    {
        Task<IEnumerable<MostUsedFeature>> GetMostUsedFeatures(string username, string timeSpan);

        Task<Response> InsertUsedFeature(string username, string featureName);
    }
}
