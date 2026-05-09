using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<IDroneService> channelFactory = new ChannelFactory<IDroneService>("DroneServiceEndpoint");
            IDroneService proxy = channelFactory.CreateChannel();
            proxy.StartSession(new SessionData (new string[] { "LinearAccelerationX", "LinearAccelerationY", "LinearAccelerationZ", "WindSpeed", "WindAngle", "Time" } ));

            List<DroneSample> samples = new List<DroneSample>();
            using (SampleReader sampleReader = new SampleReader("278.csv", "278_errors.csv"))
            {
                samples = sampleReader.ReadSamples(5);
            }

            foreach (var sample in samples)
            {
                var response = proxy.PushSample(sample);
                Console.WriteLine($"{response.SessionStatus} : {response.ResponseStatus} : {response.Message}");
            }

            Console.ReadKey();
        }
    }
}
