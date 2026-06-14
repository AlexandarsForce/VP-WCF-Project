using Common.Enumerations;
using System;

namespace Service.Events.Arguments
{
    public class WindArgument : EventArgs
    {
        public string Message { get; set; }
        public double WindEffect { get; set; }
        public AnalysisStatusType AnalysisStatus { get; set; }
    }
}
