using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS.Backend.SharedNamespace
{
    public interface IMostUsedFeature
    {
        public string FeatureName { get; set; }

        public int FeatureCount { get; set; }

    }
}
