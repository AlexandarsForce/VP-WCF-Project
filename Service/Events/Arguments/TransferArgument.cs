using Common.Contracts;
using Common.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Events.Arguments
{
    public class TransferArgument : EventArgs
    {
        public string Message { get; set; }
        public LogStatusType LogStatus { get; set; }
    }
}
