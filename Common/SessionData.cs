using System.Runtime.Serialization;

namespace Common
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
