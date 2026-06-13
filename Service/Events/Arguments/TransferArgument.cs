using Common.Enumerations;
using System;

namespace Service.Events.Arguments
{
    public class TransferArgument : EventArgs
    {
        public string Message { get; set; }
        public LogStatusType LogStatus { get; set; }
    }
}
