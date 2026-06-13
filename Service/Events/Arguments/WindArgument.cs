using Common.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Service.Events.Arguments
{
    public class WindArgument
    {
        public string Message { get; set; }
        public double WindEffect { get; set; }
        public AnalysisStatusType AnalysisStatus { get; set; }
    }
}
