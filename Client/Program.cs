using Common;
using Common.Contracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<IDroneService> channelFactory = new ChannelFactory<IDroneService>("DroneServiceEndpoint");
            IDroneService proxy = channelFactory.CreateChannel();
            ResponseData sessionResponse;

            string inputFilePath = ConfigurationManager.AppSettings["inputFilePath"];
            string errorFilePath = ConfigurationManager.AppSettings["errorFilePath"];
            int sampleCount = 100;
            bool isFinished = false;
            try
            {
                sessionResponse = proxy.StartSession(new SessionData(new string[] { "LinearAccelerationX", "LinearAccelerationY", "LinearAccelerationZ", "WindSpeed", "WindAngle", "Time" }));
                Console.WriteLine("=----------------------------------------------------------------------------------------------------------------------=");
                Console.WriteLine($" {"SESSION",-15} | {"RESPONSE",-10} | {"MESSAGE",20}");
                Console.WriteLine("=----------------------------------------------------------------------------------------------------------------------=");
                Console.WriteLine($" {sessionResponse.SessionStatus,-15} | {sessionResponse.ResponseStatus,-10} | {sessionResponse.Message}");

                List<DroneSample> samples = new List<DroneSample>();
                using (SampleReader sampleReader = new SampleReader(inputFilePath, errorFilePath))
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
                isFinished = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                if (proxy != null)
                {
                    if (!isFinished)
                    { ((IClientChannel)proxy).Abort(); }
                    ((IClientChannel)proxy).Close();
                }
                if (channelFactory != null)
                {
                    if (!isFinished)
                    { channelFactory.Abort(); }
                }
                channelFactory.Close();
            }

            Console.ReadKey();
        }
    }
}
