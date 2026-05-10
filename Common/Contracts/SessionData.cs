using System.Runtime.Serialization;

namespace Common.Contracts
{
    [DataContract]
    public class SessionData
    {
        [DataMember]
        public string[] SampleHeader { get; set; }

        public SessionData(string[] sampleHeader)
        {
            SampleHeader = sampleHeader;
        }
    }
}
