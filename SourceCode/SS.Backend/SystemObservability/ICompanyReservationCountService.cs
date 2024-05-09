using SS.Backend.SharedNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS.Backend.SystemObservability
{
    public interface ICompanyReservationCountService
    {
        Task<IEnumerable<CompanyReservationCount>> GetTop3CompaniesWithMostReservations(string username, string timeSpan);
    }
}
