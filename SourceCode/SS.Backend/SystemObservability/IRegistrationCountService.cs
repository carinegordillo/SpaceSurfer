using SS.Backend.SharedNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS.Backend.SystemObservability
{
    public interface IRegistrationCountService
    {
        Task<IEnumerable<RegistrationCount>> GetRegistrationCount(string username, string timeSpan);
    }
}
