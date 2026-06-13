using Common.Enumerations;
using System;

namespace Service.Events.Arguments
{
    public class DeviationArgument : EventArgs
    {
        public string Message { get; set; }
        public double Anorm { get; set; }
        public double Amean { get; set; }
        public AnalysisStatusType AnalysisStatus { get; set; }
    }
}
