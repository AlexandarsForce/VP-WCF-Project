using System.Runtime.Serialization;

namespace Common.Contracts
{
    [DataContract]
    public class DroneSample
    {
        [DataMember]
        public double LinearAccelerationX { get; set; }

        [DataMember]
        public double LinearAccelerationY { get; set; }

        [DataMember]
        public double LinearAccelerationZ { get; set; }

        [DataMember]
        public double WindSpeed { get; set; }

        [DataMember]
        public double WindAngle { get; set; }

        [DataMember]
        public double Time { get; set; }

        public DroneSample() { }
        
        public DroneSample(double linearAccelerationX, double linearAccelerationY, double linearAccelerationZ, double windSpeed, double windAngle, double time)
        {
            LinearAccelerationX = linearAccelerationX;
            LinearAccelerationY = linearAccelerationY;
            LinearAccelerationZ = linearAccelerationZ;
            WindSpeed = windSpeed;
            WindAngle = windAngle;
            Time = time;
        }
    }
}
