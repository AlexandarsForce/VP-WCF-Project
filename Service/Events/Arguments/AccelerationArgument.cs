using Common.Enumerations;
using System;

namespace Service.Events.Arguments
{
    public class AccelerationArgument : EventArgs
    {
        public string Message { get; set; }
        public double Anorm { get; set; }
        public double Aprevious { get; set; }
        public double Difference { get; set; }
        public AnalysisStatusType AnalysisStatus { get; set; }

    }
}
