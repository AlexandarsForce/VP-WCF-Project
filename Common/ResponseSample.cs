using Common.Enumerations;
using System.Runtime.Serialization;


namespace Common
{
    [DataContract]
    public class ResponseSample
    {
        [DataMember]
        public string ResponseMessage { get; set; }

        [DataMember]
        public ResponseStatus Status { get; set; }

        [DataMember]
        public SessionStatus SessionStatus { get; set; }

        public ResponseSample(string responseMessage, ResponseStatus status, SessionStatus sessionStatus)
        {
            ResponseMessage = responseMessage;
            Status = status;
            SessionStatus = sessionStatus;
        }
    }
}
