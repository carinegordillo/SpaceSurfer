using SS.Backend.SharedNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS.Backend.SystemObservability
{
    public interface ILoginCountService
    {
        Task<IEnumerable<LogCount>> GetLoginCount(string username, string timeSpan);
    }
}
