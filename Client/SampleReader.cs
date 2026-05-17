using Common;
using Common.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    public class SampleReader : IDisposable
    {
        private StreamReader sampleReader;
        private StreamWriter errorWriter;
        private bool disposed = false;

        public SampleReader(string sampleFileName, string errorFileName)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Dataset", sampleFileName);
            if (!File.Exists(filePath))
            {
                throw new Exception($"The file '{filePath}' does not exist.");
            }

            string errorDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Errors");
            if (!Directory.Exists(errorDirectoryPath))
            {
                Directory.CreateDirectory(errorDirectoryPath);
            }
            string errorFilePath = Path.Combine(errorDirectoryPath, errorFileName);

            sampleReader = new StreamReader(filePath, Encoding.UTF8);
            errorWriter = new StreamWriter(errorFilePath, false, Encoding.UTF8);
        }

        ~SampleReader()
        {
            Dispose(false);
        }

        public List<DroneSample> ReadSamples(int sampleCountLimit)
        {
            List<DroneSample> samples = new List<DroneSample>(sampleCountLimit);

            const int fieldCountLimit = 21;
            sampleReader.ReadLine();
            string sampleLine;
            int sampleIndex = 0;
            int lineIndex = 1;

            while (sampleIndex < sampleCountLimit)
            {
                if (sampleReader == null)
                {
                    break;
                }
                sampleLine = sampleReader.ReadLine();

                string[] sampleParts = sampleLine.Split(',');
                if (sampleParts.Length < fieldCountLimit)
                {
                    errorWriter.WriteLine($"Line {lineIndex}, {sampleLine}, Error message: Invalid field count: {sampleParts.Length} / {fieldCountLimit}");
                }
                else
                {
                    try
                    {
                        //SimulateException(); // - Uncomment this line to test exception handling and resource cleanup
                        DroneSample sample = new DroneSample
                        {
                            LinearAccelerationX = double.Parse(sampleParts[18], CultureInfo.InvariantCulture),
                            LinearAccelerationY = double.Parse(sampleParts[19], CultureInfo.InvariantCulture),
                            LinearAccelerationZ = double.Parse(sampleParts[20], CultureInfo.InvariantCulture),
                            WindAngle = double.Parse(sampleParts[2], CultureInfo.InvariantCulture),
                            WindSpeed = double.Parse(sampleParts[1], CultureInfo.InvariantCulture),
                            Time = double.Parse(sampleParts[0], CultureInfo.InvariantCulture)
                        };
                        samples.Add(sample);
                        sampleIndex++;
                    }
                    catch (ObjectDisposedException dex)
                    {
                        Console.WriteLine($"ObjectDisposedException caught: {dex.Message}");
                    }
                    catch (Exception ex)
                    {
                        errorWriter.WriteLine($"Line {lineIndex}, {sampleLine}, Error message: {ex.Message}");
                    }
                }
                lineIndex++;
            }
            if (sampleReader != null)
            {
                while ((sampleLine = sampleReader.ReadLine()) != null)
                {
                    errorWriter.WriteLine($"Line {lineIndex}, {sampleLine}, Error message: Reached sample count limit!");
                    lineIndex++;
                }
            }
            return samples;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (sampleReader != null)
                    {
                        sampleReader.Dispose();
                        sampleReader = null;
                    }
                    if (errorWriter != null)
                    {
                        errorWriter.Flush();
                        errorWriter.Dispose();
                        errorWriter = null;
                    }
                }

                disposed = true;
            }
        }

        private void SimulateException()
        {
            Console.WriteLine("Simulating an exception for testing purposes...");
            Dispose();
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(SampleReader), "Cannot perform this operation on a disposed SampleReader.");
            }
        }
    }
}