using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SS.Backend.SharedNamespace;

namespace SS.Backend.SystemObservability
{
    public interface IViewDurationService
    {
        Task<IEnumerable<ViewDuration>> GetTop3ViewDuration(string username, string timeSpan);

        Task<Response> InsertViewDuration(string username, string viewName, int viewDuration);
    }
}
