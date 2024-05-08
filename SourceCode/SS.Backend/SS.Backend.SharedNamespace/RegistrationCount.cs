using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS.Backend.SharedNamespace
{
    public class RegistrationCount : IRegistrationCount
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int FailedRegistrations { get; set; }
        public int SuccessfulRegistrations { get; set; }
    }
}
