using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS.Backend.SharedNamespace
{
    public class LogCount : ILogCount
    {
        public int Month {  get; set; }
        public int Year { get; set; }
        public int FailedLogins { get; set; }
        public int SuccessfulLogins { get; set; }
    }
}
