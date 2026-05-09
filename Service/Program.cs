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
            using (ServiceHost host = new ServiceHost(typeof(DroneService)))
            {
                host.Open();
                Console.WriteLine("Service is running. Press any key to terminate it...");
                Console.ReadKey();
                host.Close();
            }
            Console.WriteLine("Service has been terminated");
            Console.ReadKey();
        }
    }
}
