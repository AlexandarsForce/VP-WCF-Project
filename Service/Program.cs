using Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = null;
            bool isFinished = false;

            try
            {
                using (host = new ServiceHost(typeof(DroneService)))
                {
                    host.Open();
                    Console.WriteLine("Service is running. Press any key to terminate it...");
                    Console.ReadKey();
                }
                isFinished = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while starting the service: " + ex.Message);
            }
            finally
            {
                Console.WriteLine("Service has been terminated.");
                if (host != null)
                {
                    if (!isFinished)
                    { host.Abort(); }
                    host.Close();
                }
            }
        }
    }
}
