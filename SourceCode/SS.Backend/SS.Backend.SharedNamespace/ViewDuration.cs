using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS.Backend.SharedNamespace
{
    public class ViewDuration : IViewDuration
    {
        public string ViewName { get; set; }
        public int DurationInSeconds { get; set; }
    }
}
