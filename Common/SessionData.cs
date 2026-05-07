using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
    public class SessionData
    {
        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public int SampleCount { get; set; }

        [DataMember]
        public string[] SampleHeader { get; set; }
    }
}
