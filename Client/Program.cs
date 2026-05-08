using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            List<DroneSample> samples = new List<DroneSample>();
            using (SampleReader sampleReader = new SampleReader("278.csv", "278_errors.csv"))
            {
                samples = sampleReader.ReadSamples(100);
            }
        }
    }
}
