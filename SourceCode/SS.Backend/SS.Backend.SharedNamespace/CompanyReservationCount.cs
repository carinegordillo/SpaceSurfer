using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS.Backend.SharedNamespace
{
    public class CompanyReservationCount : ICompanyReservationCount
    {
        public string CompanyName { get; set; }

        public int ReservationCount { get; set; }
    }
}
