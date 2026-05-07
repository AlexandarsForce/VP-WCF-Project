using Common.Enumerations;
using System.Runtime.Serialization;


namespace Common
{
    [DataContract]
    public class ResponseData
    {
        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public ResponseStatusType ResponseStatus { get; set; }

        [DataMember]
        public SessionStatusType SessionStatus { get; set; }

        public ResponseData(string message, ResponseStatusType responseStatus, SessionStatusType sessionStatus)
        {
            Message = message;
            ResponseStatus = responseStatus;
            SessionStatus = sessionStatus;
        }
    }
}
