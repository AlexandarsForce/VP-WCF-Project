using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Common.Contracts;

namespace Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<IDroneService> channelFactory = new ChannelFactory<IDroneService>("DroneServiceEndpoint");
            IDroneService proxy = channelFactory.CreateChannel();
            int sampleCount = 100;

            ResponseData sessionResponse = proxy.StartSession(new SessionData (new string[] { "LinearAccelerationX", "LinearAccelerationY", "LinearAccelerationZ", "WindSpeed", "WindAngle", "Time" } ));
            Console.WriteLine("=----------------------------------------------------------------------------------------------------------------------=");
            Console.WriteLine($" {"SESSION",-15} | {"RESPONSE",-10} | {"MESSAGE",20}");
            Console.WriteLine("=----------------------------------------------------------------------------------------------------------------------=");
            Console.WriteLine($" {sessionResponse.SessionStatus,-15} | {sessionResponse.ResponseStatus,-10} | {sessionResponse.Message}");

            List<DroneSample> samples = new List<DroneSample>();
            using (SampleReader sampleReader = new SampleReader("278.csv", "278_errors.csv"))
            {
                samples = sampleReader.ReadSamples(sampleCount);
            }

            foreach (DroneSample sample in samples)
            {
                ResponseData response = proxy.PushSample(sample);
                Console.WriteLine($" {response.SessionStatus,-15} | {response.ResponseStatus,-10} | {response.Message}");
            }

            sessionResponse = proxy.EndSession();
            Console.WriteLine($" {sessionResponse.SessionStatus,-15} | {sessionResponse.ResponseStatus,-10} | {sessionResponse.Message}");

            Console.ReadKey();
        }
    }
}
